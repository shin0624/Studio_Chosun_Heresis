using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadingSceneManager : MonoBehaviour
{
    public static string nextScene; // 다음 씬 이름

    // UI Elements
    [SerializeField] private Image loadingBar; // 로딩 바 이미지
    [SerializeField] private List<Sprite> backgroundImages = new List<Sprite>(); // 배경 이미지 리스트
    [SerializeField] private Image backgroundImage; // 배경 이미지 오브젝트
    [SerializeField] private Material bloodMaterial; // 피 머티리얼
    [SerializeField] private float waveSpeed = 0.1f; // 피가 찰랑이는 속도
    [SerializeField] private TextMeshProUGUI tipText; // 팁 텍스트
    [SerializeField] private TextMeshProUGUI mapNameText; // 맵 이름 텍스트

    // 팁 리스트
    public string[] tips = {
        "Tip: W A S D 를 이용하여 움직일 수 있습니다.",
        "Tip: F를 눌러 플래시를 끄고 켤 수 있습니다.",
        "Tip: 저는 전주이씨 덕흥대원군파 17대 시조 46세손 입니다.",
        "Tip: 팁 테스트1.",
        "Tip: 팁 테스트2.",
        "Tip: 팁 테스트3.",
        "Tip: 팁 테스트4.",
        "Tip: 팁 테스트5."
    };

    private Queue<string> recentTips = new Queue<string>(); // 최근 팁을 저장할 큐
    private int maxRecentTips = 3; // 중복 방지를 위한 최대 큐 사이즈

    void Start()
    {
        // 랜덤 배경 이미지 설정
        SetRandomLoadingImage();

        // 맵 이름 "헤레시스" 설정
        mapNameText.text = "헤레시스";

        // 비동기 로딩 시작
        StartCoroutine(LoadAsyncScene());

        // 팁 변경 코루틴 시작
        StartCoroutine(ChangeTips());
    }

    // 다른 스크립트에서 쉽게 호출할 수 있도록
    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("LoadingScene"); // 로딩 씬 호출
    }

    // 랜덤 배경 이미지 설정
    private void SetRandomLoadingImage()
    {
        if (backgroundImages.Count > 0)
        {
            int randomIndex = Random.Range(0, backgroundImages.Count);
            backgroundImage.sprite = backgroundImages[randomIndex];
        }
    }

    // 비동기 씬 로드
    IEnumerator LoadAsyncScene()
    {
        yield return null; // 프레임 끝까지 대기

        // 씬 로드 시작
        AsyncOperation operation = SceneManager.LoadSceneAsync(nextScene);
        operation.allowSceneActivation = false; // 자동으로 씬 전환되지 않도록 설정

        float loadingDuration = 2.0f; // 최소 로딩 시간 (초)
        float startTime = Time.time; // 로딩 시작 시간

        while (!operation.isDone)
        {
            // 로딩 진행도 반영 (0~1)
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            loadingBar.fillAmount = progress;

            // 피 텍스처의 UV 좌표를 움직여서 찰랑이는 효과 주기
            bloodMaterial.mainTextureOffset = new Vector2(0, Time.time * waveSpeed);
            //bloodMaterial.mainTextureOffset = new Vector2(Mathf.Sin(Time.time * waveSpeed), Mathf.Cos(Time.time * waveSpeed));

            // 로딩 완료 시
            if (operation.progress >= 0.9f)
            {
                // 최소 로딩 시간을 보장
                float elapsedTime = Time.time - startTime;
                if (elapsedTime < loadingDuration)
                {
                    // 남은 시간만큼 대기
                    yield return new WaitForSeconds(loadingDuration - elapsedTime);
                }

                loadingBar.fillAmount = 1.0f; // 로딩 완료
                operation.allowSceneActivation = true; // 씬 전환
            }

            yield return null;
        }
    }

    // 팁을 3초마다 랜덤으로 변경, 중복 방지
    IEnumerator ChangeTips()
    {
        while (true)
        {
            string newTip;
            do
            {
                newTip = tips[Random.Range(0, tips.Length)];
            } while (recentTips.Contains(newTip));

            tipText.text = newTip;

            // 최근 팁 저장 및 중복 방지
            recentTips.Enqueue(newTip);
            if (recentTips.Count > maxRecentTips)
            {
                recentTips.Dequeue(); // 오래된 팁 제거
            }

            // 3초 대기
            yield return new WaitForSeconds(3f);
        }
    }
}
