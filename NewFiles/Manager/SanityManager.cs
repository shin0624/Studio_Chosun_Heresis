using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class SanityManager : MonoBehaviour
{
    [Header("Scripts")]
    public AudioManager audioManager;

    [Header("UI Elements")]
    public Image filledMedicine;            // 회복약 UI 이미지 (fillAmount로 표시)
    public Image filledSanity;              // 정신력 UI 이미지 (fillAmount로 표시)
    public Image redScreenPanel;            // 붉은색 화면 패널 UI
    public TextMeshProUGUI sanityText;      // 정신력 수치를 표시하는 텍스트
    public TextMeshProUGUI medicineText;    // 회복약 개수를 표시하는 텍스트

    [Header("Sounds")]
    public AudioClip eatingSound;   // 회복약 사용 시 재생할 사운드
    public AudioClip heartbeat;     // 정신력 0일 때 재생할 심장 소리

    [Header("Settings")]
    public int sanity;                      // 현재 정신력 수치
    public int maxSanity;                   // 최대 정신력 수치
    public int medicine;                    // 현재 보유한 회복약 개수
    public string difficulty = "Normal";    // 게임 난이도 설정

    private bool gameOverTriggered = false; // 게임 오버 상태를 추적
    private bool isRestoringSanity = false; // 정신력 회복 중인지 여부를 추적
    private float countdownTime = 10f;      // 정신력이 0일 때 게임 오버까지 대기 시간 
    private float lastMedicineTime = -Mathf.Infinity;   // 마지막 회복약 사용 시간

    private IEnumerator countdownCoroutine; // 게임 오버 카운트다운 코루틴 참조
    private CameraController cameraController;//정신력 감소시 카메라 흔들림 효과 시작을 전달하기 위해 참조
    void Start()
    {
        // 게임 난이도에 따라 초기 정신력 값을 설정하고 정신력을 초기화
        audioManager = GetComponent<AudioManager>();
        cameraController  = FindAnyObjectByType<CameraController>();
        SetDifficulty();
        ResetSanity();
    }

    void Update()
    {
        // Q 키를 눌러서 테스트용으로 정신력 감소
        if (Input.GetKeyDown(KeyCode.Q))
        {
            DecreaseSanity();
        }

        // R 키를 눌러서 테스트용으로 정신력 회복
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestoreSanity();
        }

        // M 키를 눌러서 테스트용으로 회복약 획득
        if (Input.GetKeyDown(KeyCode.M))
        {
            AddMedicine();
        }

        // 빌드 시에는 테스트용 키를 제거
    }

    // 난이도에 따라 최대 정신력 값을 설정
    private void SetDifficulty()
    {
        switch (difficulty)
        {
            case "Easy":
                maxSanity = 5; // Easy 난이도에서 최대 정신력 설정
                break;
            case "Normal":
                maxSanity = 3; // Normal 난이도에서 최대 정신력 설정
                break;
            default:
                maxSanity = 3; // 기본값으로 Normal 난이도 설정
                break;
        }
    }

    // 정신력을 감소시키는 메서드
    public void DecreaseSanity()
    {
        // 정신력이 0보다 클 때만 감소
        if (sanity > 0)
        {
            sanity--;
            sanityText.text = sanity.ToString(); // UI 업데이트
             if(cameraController!=null)
            {
                cameraController.StartShake();//카메라 흔들림 시작
            }
            
            // 정신력이 0 이하로 떨어지면 게임 오버 상태로 전환
            if (sanity <= 0 && !gameOverTriggered)
            {
                sanity = 0;
                sanityText.color = new Color32(230, 25, 5, 255);    // 정신력이 0일 때 정신력 텍스트 색상 변경
                gameOverTriggered = true;                           // 게임 오버 상태 설정
                audioManager.PlaySound(heartbeat);              // 심장 소리 반복 재생
                if (countdownCoroutine != null) StopCoroutine(countdownCoroutine); // 기존 카운트다운 중지
                countdownCoroutine = GameOverCountdown(countdownTime);
                StartCoroutine(countdownCoroutine); // 카운트다운 시작

                // 패널의 알파값을 부드럽게 증가
                if (redScreenPanel != null)
                {
                    StartCoroutine(FadeRedScreenPanel(redScreenPanel.color.a, 20f / 255f, 1f));
                }
            }

            // 정신력 감소에 따라 UI 업데이트
            UpdateSanityBar();
        }
    }

    // 정신력을 회복시키는 메서드
    public void RestoreSanity()
    {
        // 마지막 회복약 사용 후 5초가 지났는지 확인
        if (!isRestoringSanity && Time.time - lastMedicineTime >= 5f)
        {
            // 정신력이 최대치 미만일 때만 회복
            if (sanity < maxSanity)
            {
                if (medicine > 0)
                {
                    isRestoringSanity = true;                   // 회복 시작
                    StartCoroutine(DelayedRestoreSanity());     // 5초 후 정신력 회복
                    medicine--;
                    medicineText.text = medicine.ToString();    // 회복약 개수 UI 업데이트

                    audioManager.PlaySound(eatingSound);        // 회복약 사용 사운드 재생

                    lastMedicineTime = Time.time;               // 마지막 약을 먹은 시간 기록
                }
            }
        }
        else
        {
            // Debug.Log("약을 다시 먹기 위해서는 5초를 기다려야 합니다.");
        }
    }

    // 약을 먹은 후 5초 뒤에 정신력을 회복하는 코루틴
    private IEnumerator DelayedRestoreSanity()
    {
        float duration = 5f;    // 회복에 걸리는 시간
        float elapsedTime = 0f; // 경과 시간 추적

        filledMedicine.fillAmount = 1f; // 회복약 UI의 fillAmount를 초기화

        // 회복 과정이 진행되는 동안 반복
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            filledMedicine.fillAmount = Mathf.Lerp(1f, 0f, elapsedTime / duration); // UI의 fillAmount 애니메이션
            yield return null; // 다음 프레임까지 대기
        }

        sanity++;
        sanityText.text = sanity.ToString(); // 정신력 수치 UI 업데이트

        // 정신력 회복 시 게임 오버 상태 초기화
        if (gameOverTriggered && sanity > 0)
        {
            if (countdownCoroutine != null) StopCoroutine(countdownCoroutine); // 카운트다운 중지
            sanityText.color = new Color32(25, 25, 5, 255); // 정신력 회복 시 정신력 텍스트 기본 색상으로 변경
            sanityText.text = sanity.ToString();    // 정신력 수치 UI 업데이트
            gameOverTriggered = false;              // 게임 오버 상태 초기화
            audioManager.StopSound(heartbeat);           // 심장 소리 정지

            // 패널의 알파값을 부드럽게 감소
            if (redScreenPanel != null)
            {
                StartCoroutine(FadeRedScreenPanel(redScreenPanel.color.a, 0f, 1f)); // 1초 동안 알파값을 0으로 변경
            }
        }

        isRestoringSanity = false; // 회복 완료

        // 정신력 회복 후 UI 업데이트
        UpdateSanityBar();
    }

    // 정신력 바 UI를 업데이트하는 메서드
    private void UpdateSanityBar()
    {
        if (maxSanity > 0)
        {
            float targetFillAmount = 1 - ((float)sanity / maxSanity); // 정신력 비율 계산
            StartCoroutine(SmoothUpdateSanityBar(filledSanity.fillAmount, targetFillAmount)); // 부드럽게 업데이트
        }
    }

    // 정신력 바의 fillAmount 값을 부드럽게 업데이트하는 코루틴
    private IEnumerator SmoothUpdateSanityBar(float startFillAmount, float targetFillAmount)
    {
        float duration = 1f;    // 부드럽게 업데이트할 시간
        float elapsedTime = 0f; // 경과 시간 추적

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            filledSanity.fillAmount = Mathf.Lerp(startFillAmount, targetFillAmount, elapsedTime / duration); // UI 애니메이션
            yield return null; // 다음 프레임까지 대기
        }

        filledSanity.fillAmount = targetFillAmount; // 최종 값 설정
    }

    // 붉은색 화면 패널의 알파값을 부드럽게 변화시키는 코루틴
    private IEnumerator FadeRedScreenPanel(float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;
        Color startColor = redScreenPanel.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, endAlpha);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            redScreenPanel.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }

        redScreenPanel.color = endColor; // 최종 색상 설정
    }

    // 정신력을 초기화하는 메서드
    public void ResetSanity()
    {
        sanity = maxSanity;                         // 초기화 시 최대 정신력으로 설정
        sanityText.text = sanity.ToString();        // 정신력 수치 UI 업데이트
        medicineText.text = medicine.ToString();    // 회복약 개수 UI 업데이트
        gameOverTriggered = false;                  // 게임 오버 상태 초기화
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);  // 카운트다운 중지
            countdownCoroutine = null;          // 코루틴 참조 초기화
        }
    }

    // 회복약 개수를 증가시키는 메서드
    public void AddMedicine()
    {
        medicine++;
        medicineText.text = medicine.ToString(); // UI 업데이트
    }

    // 게임 오버까지의 카운트다운을 처리하는 코루틴
    private IEnumerator GameOverCountdown(float delay)
    {
        float elapsedTime = 0f; // 경과 시간 추적
        while (elapsedTime < delay)
        {
            float remainingTime = delay - elapsedTime;
            // sanityText.text = $"게임오버까지 {Mathf.Ceil(remainingTime)}초"; // 남은 시간 UI 업데이트
            yield return new WaitForSeconds(1f); // 1초 대기
            elapsedTime += 1f; // 경과 시간 업데이트
        }
        GameOver(); // 게임 오버 처리
    }

    // 게임 오버를 처리하는 메서드
    private void GameOver()
    {
        // sanityText.text = "Game Over"; // 게임 오버 메시지 표시
        audioManager.StopSound(heartbeat); // 심장 소리 정지
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name); // 현재 장면 다시 로드
        /*
         * 게임 오버 로직 추가
         */
    }
}