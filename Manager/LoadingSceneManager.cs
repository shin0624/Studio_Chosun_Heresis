using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

// 로딩 화면을 관리하는 매니저 클래스
// 씬 전환 시 로딩 화면을 표시하고, 진행 상태를 시각적으로 표현
public class LoadingSceneManager : MonoBehaviour
{
    // 다음에 로드할 씬의 이름을 저장하는 정적 변수
    public static string nextScene;

    // === UI 컴포넌트 참조 ===
    [Header("UI References")]
    [SerializeField] private Slider loadingBar;           // 로딩 진행도를 표시할 슬라이더
    [SerializeField] private Image backgroundImage;       // 배경 이미지
    [SerializeField] private TextMeshProUGUI tipText;     // 팁 메시지를 표시할 텍스트
    [SerializeField] private TextMeshProUGUI mapNameText; // 맵 이름을 표시할 텍스트

    // === 로딩 화면 설정 ===
    [Header("Loading Settings")]
    [SerializeField] private List<Sprite> backgroundImages = new List<Sprite>();  // 랜덤하게 표시될 배경 이미지 목록
    [SerializeField] private Material bloodMaterial;       // 피 효과에 사용될 머티리얼
    [SerializeField] private float minLoadingTime = 5.0f;  // 최소 로딩 시간(초)
    [SerializeField] private float tipChangeInterval = 3.0f; // 팁 변경 간격(초)

    // === 로딩바 설정 ===
    [Header("Loading Bar Settings")]
    [SerializeField] private float fillSpeed = 0.2f;      // 로딩바가 채워지는 속도
    [SerializeField] private float smoothness = 1f;       // 로딩바 움직임의 부드러움
    [SerializeField] private float initialDelay = 0.5f;   // 초기 대기 시간

    // === 피 효과 설정 ===
    [Header("Blood Effect Settings")]
    [SerializeField, Range(0, 5)] private float waveSpeed = 1f;      // 웨이브 움직임 속도
    [SerializeField, Range(0, 10)] private float waveFrequency = 3f; // 웨이브 주파수
    [SerializeField, Range(0, 0.5f)] private float waveAmplitude = 0.1f; // 웨이브 진폭

    // === 캐시된 문자열 상수 ===
    private const string LOADING_SCENE = "LoadingScene";  // 로딩 씬 이름
    private const string MAP_NAME = "헤레시스";           // 맵 이름
    private const string MAP_NAME_TEXT = "MapNameText";   // 맵 이름 텍스트 오브젝트 이름

    // 캐시된 WaitForSeconds 객체들 (성능 최적화용)
    private static readonly WaitForSeconds INITIAL_SCENE_DELAY = new WaitForSeconds(0.1f);
    private static readonly WaitForSeconds LOADING_COMPLETE_DELAY = new WaitForSeconds(0.5f);

    // === 게임 팁 목록 ===
    // 플레이어에게 표시될 팁 메시지들을 저장하는 배열
    private static readonly string[] TIPS = {
       "Tip: W A S D 를 이용하여 움직일 수 있습니다.",
       "Tip: F를 눌러 플래시를 끄고 켤 수 있습니다.",
       "Tip: 일지나 메모를 주의 깊게 읽어보세요. 중요한 정보가 담겨있을 수 있습니다.",
       "Tip: 아이템을 찾을 때는 구석구석을 잘 살펴보세요. 작은 단서가 숨어있을 수 있습니다.",
       "방금 들은 속삭임은 진짜일까요, 아니면 당신의 상상일까요?",
       "당신 뒤에 누가 있는 것 같나요? 아마도 착각일 겁니다... 아마도요."
   };

    // 최근에 표시된 팁의 최대 개수 (중복 방지용)
    private const int MAX_RECENT_TIPS = 3;

    // === 프라이빗 변수들 ===
    private float targetProgress;          // 목표 진행도
    private float currentProgress;         // 현재 진행도
    private bool isOperationComplete;      // 씬 로딩 완료 여부
    private AsyncOperation loadOperation;   // 비동기 씬 로딩 작업
    private bool isLoading;                // 현재 로딩 중인지 여부
    private readonly HashSet<int> recentTipIndices = new HashSet<int>();  // 최근 표시된 팁 인덱스 저장
    private readonly System.Random random = new System.Random();           // 랜덤 생성기
    private WaitForSeconds tipChangeWait;  // 팁 변경 대기 시간

    // 컴포넌트 초기화
    private void Awake()
    {
        InitializeVariables();     // 변수 초기화
        AutoAssignComponents();    // 컴포넌트 자동 할당
        ValidateReferences();      // 참조 유효성 검사
    }

    // 변수 초기화
    private void InitializeVariables()
    {
        targetProgress = 0f;
        currentProgress = 0f;
        isOperationComplete = false;
        isLoading = false;
        tipChangeWait = new WaitForSeconds(tipChangeInterval);
    }

    // 누락된 컴포넌트 자동 찾기 및 할당
    private void AutoAssignComponents()
    {
        loadingBar ??= GetComponentInChildren<Slider>();
        backgroundImage ??= GetComponentInChildren<Image>();
        tipText ??= GetComponentInChildren<TextMeshProUGUI>();
        mapNameText ??= GameObject.Find(MAP_NAME_TEXT)?.GetComponent<TextMeshProUGUI>();
    }

    // 필수 컴포넌트 유효성 검사
    private void ValidateReferences()
    {
        if (!AreComponentsValid())
        {
            Debug.LogError("Essential components are missing. Disabling LoadingSceneManager.");
            enabled = false;
        }
    }

    // 모든 필수 컴포넌트가 할당되었는지 확인
    private bool AreComponentsValid() =>
        loadingBar != null &&
        backgroundImage != null &&
        tipText != null &&
        mapNameText != null &&
        bloodMaterial != null;

    // 컴포넌트 시작 시 초기화
    private void Start()
    {
        if (!enabled || string.IsNullOrEmpty(nextScene)) return;
        InitializeLoadingScene();
    }

    // 로딩 화면 초기화
    private void InitializeLoadingScene()
    {
        ResetLoadingBar();         // 로딩바 초기화
        SetRandomLoadingImage();   // 랜덤 배경 이미지 설정
        StartLoadingProcesses();   // 로딩 프로세스 시작
    }

    // 로딩바 초기화
    private void ResetLoadingBar()
    {
        if (loadingBar != null)
        {
            loadingBar.value = 0f;
            loadingBar.interactable = false;
        }
    }

    // 랜덤 배경 이미지 설정
    private void SetRandomLoadingImage()
    {
        if (backgroundImages == null || backgroundImages.Count == 0) return;
        backgroundImage.sprite = backgroundImages[Random.Range(0, backgroundImages.Count)];
    }

    // 로딩 프로세스 시작
    private void StartLoadingProcesses()
    {
        mapNameText.text = MAP_NAME;
        isLoading = true;

        StartCoroutine(UpdateLoadingBar());   // 로딩바 업데이트 시작
        StartCoroutine(DelayedLoadScene());   // 씬 로딩 시작
        StartCoroutine(ChangeTips());         // 팁 변경 시작
        UpdateBloodEffect();                  // 초기 피 효과 설정
    }

    // 피 효과 업데이트
    private void UpdateBloodEffect()
    {
        if (bloodMaterial == null) return;

        // 쉐이더 프로퍼티 업데이트
        bloodMaterial.SetFloat("_WaveSpeed", waveSpeed);
        bloodMaterial.SetFloat("_WaveFrequency", waveFrequency);
        bloodMaterial.SetFloat("_WaveAmplitude", waveAmplitude);
    }

    // 지연된 씬 로딩 시작
    private IEnumerator DelayedLoadScene()
    {
        yield return INITIAL_SCENE_DELAY;
        StartCoroutine(LoadAsyncScene());
    }

    // 다른 씬에서 로딩 화면을 호출할 때 사용하는 정적 메서드
    public static void LoadScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName)) return;

        nextScene = sceneName;
        ResetCurrentLoadingBar();
        SceneManager.LoadScene(LOADING_SCENE);
    }

    // 현재 로딩바 리셋
    private static void ResetCurrentLoadingBar()
    {
        if (FindObjectOfType<LoadingSceneManager>() is { } currentManager &&
            currentManager.loadingBar != null)
        {
            currentManager.loadingBar.value = 0f;
        }
    }

    // 비동기 씬 로딩
    private IEnumerator LoadAsyncScene()
    {
        yield return INITIAL_SCENE_DELAY;

        loadOperation = SceneManager.LoadSceneAsync(nextScene);
        if (loadOperation == null) yield break;

        loadOperation.allowSceneActivation = false;

        while (!loadOperation.isDone)
        {
            if (loadOperation.progress >= 0.9f)
            {
                isOperationComplete = true;
            }
            yield return null;
        }
    }

    // 로딩바 업데이트
    private IEnumerator UpdateLoadingBar()
    {
        float startTime = Time.time;
        WaitForEndOfFrame endOfFrame = new WaitForEndOfFrame();

        yield return endOfFrame;
        yield return new WaitForSeconds(initialDelay);

        while (true)
        {
            float elapsedTime = Time.time - startTime;

            if (loadOperation == null)
            {
                yield return endOfFrame;
                continue;
            }

            UpdateProgress(elapsedTime);
            UpdateBloodEffect();

            if (ShouldCompleteLoading(elapsedTime))
            {
                CompleteLoading();
                break;
            }

            yield return endOfFrame;
        }
    }

    // 진행도 업데이트
    private void UpdateProgress(float elapsedTime)
    {
        if (isOperationComplete && elapsedTime >= minLoadingTime)
        {
            targetProgress = 1f;
            currentProgress = Mathf.MoveTowards(currentProgress, targetProgress, fillSpeed * 0.5f * Time.deltaTime);
        }
        else
        {
            float artificialProgress = (elapsedTime / minLoadingTime) * 0.9f;
            targetProgress = Mathf.Min(artificialProgress, loadOperation.progress / 0.9f);
            currentProgress = Mathf.Lerp(currentProgress, targetProgress, Time.deltaTime * smoothness);
        }

        loadingBar.value = currentProgress;
    }

    // 로딩 완료 조건 체크
    private bool ShouldCompleteLoading(float elapsedTime) =>
        currentProgress >= 0.99f && isOperationComplete && elapsedTime >= minLoadingTime;

    // 로딩 완료 처리
    private void CompleteLoading()
    {
        loadingBar.value = 1f;
        loadOperation.allowSceneActivation = true;
    }

    // 팁 메시지 변경
    private IEnumerator ChangeTips()
    {
        while (isLoading)
        {
            tipText.text = GetRandomUniqueTip();
            yield return tipChangeWait;
        }
    }

    // 중복되지 않는 랜덤 팁 선택
    private string GetRandomUniqueTip()
    {
        int tipIndex;
        do
        {
            tipIndex = Random.Range(0, TIPS.Length);
        } while (!recentTipIndices.Add(tipIndex) && recentTipIndices.Count < TIPS.Length);

        if (recentTipIndices.Count > MAX_RECENT_TIPS)
        {
            recentTipIndices.Clear();
            recentTipIndices.Add(tipIndex);
        }

        return TIPS[tipIndex];
    }

    // 컴포넌트가 파괴될 때 정리 작업
    private void OnDestroy()
    {
        isLoading = false;
        StopAllCoroutines();
        recentTipIndices.Clear();
    }

}