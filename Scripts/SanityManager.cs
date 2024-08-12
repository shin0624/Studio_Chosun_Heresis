using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; // 게임오버 시 씬을 재시작하기 위해 사용

public class SanityManager : MonoBehaviour
{
    public TextMeshProUGUI sanityText; // 정신력 수치 UI 텍스트
    public TextMeshProUGUI medicineText; // 회복약 수치 UI 텍스트

    public int sanity; // 정신력 수치
    public int maxSanity; // 최대 정신력 (난이도별)
    public int medicine; // 회복약 개수

    public string difficulty = "Normal";

    private bool gameOverTriggered = false; // 게임오버 상태를 추적
    private float countdownTime = 20f; // 정신력 0일 때 게임오버 시간

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
            sanityText.text = "정신력: " + sanity.ToString();

            // 정신력이 0 이하로 떨어지면 게임오버를 지연시키는 코루틴 호출
            if (sanity <= 0 && !gameOverTriggered)
            {
                sanity = 0;
                gameOverTriggered = true; // 게임오버가 트리거되었음을 표시
                if (countdownCoroutine != null) StopCoroutine(countdownCoroutine); // 기존 카운트다운 중지
                countdownCoroutine = GameOverCountdown(countdownTime);
                StartCoroutine(countdownCoroutine); // 카운트다운 시작
            }
        }
    }

    public void RestoreSanity()
    {
        // 정신력이 최대치 이하일 때만 회복
        if (sanity < maxSanity)
        {
            if (medicine > 0)
            {
                sanity++;
                medicine--;

                // UI 텍스트 업데이트
                sanityText.text = "정신력: " + sanity.ToString();
                medicineText.text = "회복약: " + medicine.ToString();

                // 정신력 회복 시 게임오버 상태 초기화
                if (gameOverTriggered && sanity > 0)
                {
                    if (countdownCoroutine != null) StopCoroutine(countdownCoroutine); // 카운트다운 중지
                    sanityText.text = "정신력: " + sanity.ToString(); // 정신력 UI 업데이트
                    gameOverTriggered = false; // 게임오버 트리거 상태 초기화
                }
            }
        }
    }

    // 정신력을 초기화하는 함수
    public void ResetSanity()
    {
        sanity = maxSanity; // 초기화 시 최대 정신력으로 설정
        sanityText.text = "정신력: " + sanity.ToString(); // UI 텍스트 업데이트
        medicineText.text = "회복약: " + medicine.ToString();
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

        medicineText.text = "회복약: " + medicine.ToString();
    }

    private IEnumerator GameOverCountdown(float delay)
    {
        float elapsedTime = 0f; // 경과 시간
        while (elapsedTime < delay)
        {
            float remainingTime = delay - elapsedTime;
            sanityText.text = $"게임오버까지 {Mathf.Ceil(remainingTime)}초"; // 남은 시간 UI 업데이트
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
    }
}
