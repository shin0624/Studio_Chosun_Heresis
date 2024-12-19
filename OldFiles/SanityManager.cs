using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; // 게임오버 시 씬을 재시작하기 위해 사용
using System;

public class SanityManager : MonoBehaviour
{
    // 약 먹는 사운드
    public AudioClip eatingSound;

    public TextMeshProUGUI sanityText; // 정신력 수치 UI 텍스트
    public TextMeshProUGUI medicineText; // 회복약 수치 UI 텍스트

    public int sanity; // 정신력 수치 --> 사용되는 정신력 수치는 소문자로 기재

    public int maxSanity; // 최대 정신력 (난이도별)
    public int medicine; // 회복약 개수

    public string difficulty = "Normal";

    private bool gameOverTriggered = false; // 게임오버 상태를 추적
    private bool isRestoringSanity = false; // 정신력 회복 중인지 추적
    private float countdownTime = 15f; // 정신력 0일 때 게임오버 시간
    private float lastMedicineTime = -Mathf.Infinity; // 마지막으로 약을 먹은 시간

    private IEnumerator countdownCoroutine; // 카운트다운 코루틴 참조 저장

    //정신력 감소 타이머 추가. 정신력이 1, 2일 때 10초 경과 시 정신력 한 단계 하락.
    private float sanityTimer = 0f;
    [SerializeField]
    private float sanityDecreaseInterval = 10.0f;// 정신력 지속시간.

    // sanity 이벤트 추가 --> 정신력 ui 이벤트를 위해 게터세터를 선언할 변수는 SSanity로 기재
    public event Action<int> OnSanityChanged;// sanity값에 따라 화면 효과를 다르게 하기 위해 이벤트 생성. SanityImageController에서 구독하여 사용할 것. 
    public int SSanity { get { return sanity; } set { if (sanity != value) { sanity = value; OnSanityChanged?.Invoke(sanity); sanityTimer = 0f; } } }//SSanity값이 변경될 때 이벤트 발생. 정신력 변경 시 타이머 초기화 추가

    private CameraController cameraController;//정신력 감소시 카메라 흔들림 효과 시작을 전달하기 위해 참조
    
    void Start()
    {
        // 난이도에 따라 초기 정신력 값 설정
        SetDifficulty();
        ResetSanity();
        cameraController  = FindAnyObjectByType<CameraController>();
    }

    void Update()
    {
        // 테스트를 위해 Q 키를 누르면 정신력 감소
        if (Input.GetKeyDown(KeyCode.Q))
        {
            DecreaseSanity();
            Debug.Log($"now sanity : {sanity} ");

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

        if(SSanity == 1 || SSanity == 2)
        {
            sanityTimer += Time.deltaTime;
            if(sanityTimer>=sanityDecreaseInterval)// 10초 넘어가면
            {
                DecreaseSanity();//정신력 감소
                sanityTimer = 0f;//타이머 초기화.
            }
        }



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

            if(cameraController!=null)
            {
                cameraController.StartShake();//카메라 흔들림 시작
            }

            // 정신력이 0 이하로 떨어지면 게임오버를 지연시키는 코루틴 호출
            if (sanity <= 0 && !gameOverTriggered)
            {
                sanity = 0;
                sanityText.color = new Color32(230, 25, 5, 255); // 0일 시 붉은 색
                gameOverTriggered = true; // 게임오버가 트리거되었음을 표시
                if (countdownCoroutine != null) StopCoroutine(countdownCoroutine); // 기존 카운트다운 중지
                countdownCoroutine = GameOverCountdown(countdownTime);
                StartCoroutine(countdownCoroutine); // 카운트다운 시작
            }

        }
    }

    public void RestoreSanity()
    {
        // 현재 시간과 마지막으로 약을 먹은 시간 비교 및 회복 중인지 확인
        if (!isRestoringSanity && Time.time - lastMedicineTime >= 5f) // 5초가 지났는지 확인
        {
            // 정신력이 최대치 이하일 때만 회복
            if (SSanity < maxSanity)
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
        yield return new WaitForSeconds(5f); // 5초 대기

        SSanity++;
        sanityText.text = SSanity.ToString(); // UI 텍스트 업데이트

        // 정신력 회복 시 게임오버 상태 초기화
        if (gameOverTriggered && SSanity > 0)
        {
            if (countdownCoroutine != null) StopCoroutine(countdownCoroutine); // 카운트다운 중지
            sanityText.color = new Color32(25, 25, 5, 255); // 회복 시 기본 색상으로
            sanityText.text = sanity.ToString(); // 정신력 UI 업데이트
            gameOverTriggered = false; // 게임오버 트리거 상태 초기화
        }

        isRestoringSanity = false; // 회복 완료
    }

    // 정신력을 초기화하는 함수
    public void ResetSanity()
    {
        SSanity = maxSanity; // 초기화 시 최대 정신력으로 설정
        sanityText.text = SSanity.ToString(); // UI 텍스트 업데이트
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
        // 게임오버 로직 추가
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


}
