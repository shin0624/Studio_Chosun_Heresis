using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class SanityManager : MonoBehaviour
{
    // 약 먹는 사운드
    public AudioClip eatingSound;

    // 심장소리 사운드
    public AudioClip heartbeat;

    // filledImage 참조
    public Image filledMedicine;
    public Image filledSanity;

    // TMP UI 텍스트 참조
    public TextMeshProUGUI sanityText; // 정신력 수치
    public TextMeshProUGUI medicineText; // 회복약 수치

    public int sanity; // 정신력 수치
    public int maxSanity; // 최대 정신력
    public int medicine; // 회복약 개수

    public string difficulty = "Normal"; // 난이도

    private bool gameOverTriggered = false; // 게임오버 상태를 추적
    private bool isRestoringSanity = false; // 정신력 회복 중인지 추적
    private float countdownTime = 10f; // 정신력 0일 때 게임오버 시간
    private float lastMedicineTime = -Mathf.Infinity; // 마지막으로 약을 먹은 시간

    private IEnumerator countdownCoroutine; // 카운트다운 코루틴 참조 저장

    void Start()
    {
        // 난이도에 따라 초기 정신력 값 설정
        SetDifficulty();
        ResetSanity();
    }

    void Update()
    {
        // 테스트를 위해 Q 키를 누르면 정신력 감소
        if (Input.GetKeyDown(KeyCode.Q))
        {
            DecreaseSanity();
        }

        // 테스트를 위해 R 키를 누르면 정신력 회복
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestoreSanity();
        }

        // 테스트를 위해 M 키를 누르면 회복약 획득
        if (Input.GetKeyDown(KeyCode.M))
        {
            AddMedicine();
        }

        //빌드 시 회복 키 제외 모두 삭제
    }

    // 난이도에 따라 최대 정신력 설정
    private void SetDifficulty()
    {
        switch (difficulty)
        {
            case "Easy":
                maxSanity = 5;
                break;
            case "Normal":
                maxSanity = 3;
                break;
            default:
                maxSanity = 3; // 기본값은 Normal
                break;
        }
    }

    public void DecreaseSanity()
    {
        // 정신력이 1 이상일 때만 감소
        if (sanity > 0)
        {
            sanity--;

            // UI 텍스트 업데이트
            sanityText.text = sanity.ToString();

            // 정신력이 0 이하로 떨어지면 게임오버를 지연시키는 코루틴 호출
            if (sanity <= 0 && !gameOverTriggered)
            {
                sanity = 0;
                sanityText.color = new Color32(230, 25, 5, 255); // 0일 시 붉은 색
                gameOverTriggered = true; // 게임오버가 트리거되었음을 표시
                AudioManager.Instance.PlaySoundLoop(heartbeat);
                if (countdownCoroutine != null) StopCoroutine(countdownCoroutine); // 기존 카운트다운 중지
                countdownCoroutine = GameOverCountdown(countdownTime);
                StartCoroutine(countdownCoroutine); // 카운트다운 시작
            }

            // 정신력 감소 시 체력바 업데이트
            UpdateSanityBar();
        }
    }

    public void RestoreSanity()
    {
        // 현재 시간과 마지막으로 약을 먹은 시간 비교 및 회복 중인지 확인
        if (!isRestoringSanity && Time.time - lastMedicineTime >= 5f) // 5초가 지났는지 확인
        {
            // 정신력이 최대치 이하일 때만 회복
            if (sanity < maxSanity)
            {
                if (medicine > 0)
                {
                    isRestoringSanity = true; // 회복 시작
                    StartCoroutine(DelayedRestoreSanity()); // 5초 후 정신력 회복
                    medicine--;
                    medicineText.text = medicine.ToString();

                    AudioManager.Instance.PlaySound(eatingSound); // AudioManager를 통해 사운드 재생

                    lastMedicineTime = Time.time; // 현재 시간을 마지막 약 먹은 시간으로 기록
                }
            }
        }
        else
        {
            //Debug.Log("약을 다시 먹기 위해서는 5초를 기다려야 합니다.");
        }
    }

    // 약을 먹은 후 5초 뒤에 정신력을 회복하는 코루틴
    private IEnumerator DelayedRestoreSanity()
    {
        // 회복에 걸리는 시간 (5초) // 약 먹는 소리 사운드 길이
        float duration = 5f;
        // 경과 시간 추적 변수
        float elapsedTime = 0f;

        filledMedicine.fillAmount = 1f;

        // 회복 과정이 진행되는 동안 반복문 실행
        while (elapsedTime < duration)
        {
            // 경과 시간을 업데이트 (Time.deltaTime은 프레임 간 경과 시간)
            elapsedTime += Time.deltaTime;
            // 경과된 시간 비율에 따라 fillAmount를 1에서 0으로 감소시키며 시각적 효과를 구현
            filledMedicine.fillAmount = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            // 다음 프레임까지 대기
            yield return null;
        }

        sanity++;
        sanityText.text = sanity.ToString(); // UI 텍스트 업데이트

        // 정신력 회복 시 게임오버 상태 초기화
        if (gameOverTriggered && sanity > 0)
        {
            if (countdownCoroutine != null) StopCoroutine(countdownCoroutine); // 카운트다운 중지
            sanityText.color = new Color32(25, 25, 5, 255); // 회복 시 기본 색상으로
            sanityText.text = sanity.ToString(); // 정신력 UI 업데이트
            gameOverTriggered = false; // 게임오버 트리거 상태 초기화
            AudioManager.Instance.StopSoundLoop();
        }

        isRestoringSanity = false; // 회복 완료

        // 정신력 회복 후 체력바 업데이트
        UpdateSanityBar();
    }

    private void UpdateSanityBar()
    {
        // 최대 정신력이 0보다 큰 경우에만 업데이트 수행
        if (maxSanity > 0)
        {
            // 현재 정신력 비율을 계산 (0에서 1 사이)
            float targetFillAmount = 1 - ((float)sanity / maxSanity);
            // fillAmount 업데이트
            StartCoroutine(SmoothUpdateSanityBar(filledSanity.fillAmount, targetFillAmount));
        }
    }

    private IEnumerator SmoothUpdateSanityBar(float startFillAmount, float targetFillAmount)
    {
        // UI 부드럽게 업데이트 시간
        float duration = 1f;
        // 경과 시간 추적 변수
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            // fillamount값 부드럽게 변경
            filledSanity.fillAmount = Mathf.Lerp(startFillAmount, targetFillAmount, elapsedTime / duration);
            // 다음 프레임까지 대기
            yield return null;
        }

        // 전환이 완료된 후 최종 값 설정
        filledSanity.fillAmount = targetFillAmount;
    }

    // 정신력을 초기화하는 함수
    public void ResetSanity()
    {
        sanity = maxSanity; // 초기화 시 최대 정신력으로 설정
        sanityText.text = sanity.ToString(); // UI 텍스트 업데이트
        medicineText.text = medicine.ToString();
        gameOverTriggered = false; // 게임오버 상태 초기화
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine); // 이전 카운트다운 중지
            countdownCoroutine = null; // 카운트다운 코루틴 참조를 null로 설정
        }
    }

    public void AddMedicine()
    {
        medicine++;
        medicineText.text = medicine.ToString();
    }

    private IEnumerator GameOverCountdown(float delay)
    {
        float elapsedTime = 0f; // 경과 시간
        while (elapsedTime < delay)
        {
            float remainingTime = delay - elapsedTime;
            //sanityText.text = $"게임오버까지 {Mathf.Ceil(remainingTime)}초"; // 남은 시간 UI 업데이트
            yield return new WaitForSeconds(1f); // 1초 대기
            elapsedTime += 1f; // 경과 시간 업데이트
        }
        GameOver(); // 게임 오버 처리
    }

    // 게임 오버를 처리하는 함수
    private void GameOver()
    {
        sanityText.text = "Game Over"; // test
        AudioManager.Instance.StopSoundLoop();
        // 게임오버 로직 추가
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
