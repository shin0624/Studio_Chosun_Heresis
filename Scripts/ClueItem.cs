using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class ClueItem : Interactable
{
    // 단서 기본 정보
    public Sprite clueImage;                // 표시될 단서 이미지
    public string clueTitle;                // 단서 제목
    [TextArea(3, 10)]                       // 여러 줄 입력이 가능하도록 설정
    public string clueDescription;          // 단서 설명

    // UI 요소들
    public GameObject clueCanvas;           // 단서를 표시할 전체 UI 캔버스
    public Image displayImage;              // 단서 이미지를 표시할 이미지 컴포넌트
    public TextMeshProUGUI titleText;       // 제목 텍스트
    public TextMeshProUGUI descriptionText; // 설명 텍스트

    // 애니메이션 관련 변수들
    public float animationDuration = 0.5f;   // 카드 애니메이션 재생 시간
    public float startPositionY = -1000f;    // 시작 위치 (화면 아래)
    public float endPositionY = 0f;          // 도착 위치 (화면 중앙)
    public float textFadeDuration = 0.3f;    // 텍스트 페이드 인/아웃 시간
    public float textDelayDuration = 0.2f;   // 텍스트 표시 전 대기 시간

    private RectTransform displayRect;       // 이미지의 RectTransform 컴포넌트
    private bool isShowing = false;          // 현재 단서가 보이는지 여부
    private Coroutine currentAnimation;      // 현재 실행 중인 애니메이션 추적

    private void Start()
    {
        // 필수 컴포넌트 체크
        if (!ValidateComponents()) return;

        clueCanvas.SetActive(false);         // 시작할 때 UI 숨기기
        displayRect = displayImage.GetComponent<RectTransform>();

        // RectTransform 컴포넌트 확인
        if (displayRect == null)
        {
            Debug.LogError("Display Image must have RectTransform component");
            enabled = false;
            return;
        }

        // 텍스트 초기 상태 설정 (투명)
        titleText.alpha = 0;
        descriptionText.alpha = 0;
    }

    // 필수 컴포넌트들이 모두 있는지 확인
    private bool ValidateComponents()
    {
        if (clueCanvas == null || displayImage == null ||
            titleText == null || descriptionText == null)
        {
            Debug.LogError($"Missing required components on {gameObject.name}");
            enabled = false;
            return false;
        }
        return true;
    }

    private void Update()
    {
        // ESC 키로 단서창 닫기
        if (isShowing && Input.GetKeyDown(KeyCode.Escape))
        {
            HideClue();
        }
    }

    public override void Interact()
    {
        // E키 입력시 단서 보여주기 (스크립트가 활성화된 경우에만)
        if (!isShowing && enabled) ShowClue();
    }

    private void ShowClue()
    {
        // 컴포넌트 유효성 검사
        if (!ValidateComponents()) return;

        // 이전 애니메이션이 실행 중이면 중지
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }

        isShowing = true;
        clueCanvas.SetActive(true);

        // UI 내용 설정
        displayImage.sprite = clueImage;
        titleText.text = clueTitle;
        descriptionText.text = clueDescription;

        // 시작 위치 설정 (화면 아래)
        Vector2 startPos = displayRect.anchoredPosition;
        startPos.y = startPositionY;
        displayRect.anchoredPosition = startPos;

        // 텍스트 초기화 (투명)
        titleText.alpha = 0;
        descriptionText.alpha = 0;

        // 애니메이션 시작
        currentAnimation = StartCoroutine(AnimateClueCard());
    }

    private void HideClue()
    {
        // 이전 애니메이션이 실행 중이면 중지
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }
        currentAnimation = StartCoroutine(HideClueAnimation());
    }

    private void OnDisable()
    {
        // 스크립트가 비활성화될 때 실행 중인 애니메이션 정리
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
            currentAnimation = null;
        }

        // UI 강제로 숨기기
        if (clueCanvas != null)
        {
            clueCanvas.SetActive(false);
        }
        isShowing = false;
    }

    private IEnumerator AnimateClueCard()
    {
        float elapsed = 0f;
        Vector2 startPos = displayRect.anchoredPosition;
        Vector2 endPos = startPos;
        endPos.y = endPositionY;

        // 카드가 아래에서 위로 올라오는 애니메이션
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.SmoothStep(0, 1, elapsed / animationDuration);

            // 위치와 크기 동시 조정
            displayRect.anchoredPosition = Vector2.Lerp(startPos, endPos, progress);
            displayRect.localScale = Vector3.Lerp(new Vector3(0.8f, 0.8f, 1f), Vector3.one, progress);

            yield return null;
        }

        // 최종 위치와 크기로 설정
        displayRect.anchoredPosition = endPos;
        displayRect.localScale = Vector3.one;

        // 텍스트 페이드 인 시작
        yield return new WaitForSeconds(textDelayDuration);
        StartCoroutine(FadeInText(titleText));           // 제목 먼저 표시
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(FadeInText(descriptionText));     // 설명 나중에 표시
    }

    private IEnumerator FadeInText(TextMeshProUGUI text)
    {
        // 텍스트 페이드 인 애니메이션
        float elapsed = 0f;
        while (elapsed < textFadeDuration)
        {
            elapsed += Time.deltaTime;
            text.alpha = Mathf.Lerp(0, 1, elapsed / textFadeDuration);
            yield return null;
        }
        text.alpha = 1;
    }

    private IEnumerator HideClueAnimation()
    {
        // 텍스트 먼저 사라지게 하기
        StartCoroutine(FadeOutText(descriptionText));
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(FadeOutText(titleText));

        // 카드가 위에서 아래로 내려가는 애니메이션
        float elapsed = 0f;
        Vector2 startPos = displayRect.anchoredPosition;
        Vector2 endPos = startPos;
        endPos.y = startPositionY;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.SmoothStep(0, 1, elapsed / animationDuration);

            // 위치와 크기 동시 조정
            displayRect.anchoredPosition = Vector2.Lerp(startPos, endPos, progress);
            displayRect.localScale = Vector3.Lerp(Vector3.one, new Vector3(0.8f, 0.8f, 1f), progress);

            yield return null;
        }

        // UI 비활성화
        isShowing = false;
        clueCanvas.SetActive(false);
    }

    private IEnumerator FadeOutText(TextMeshProUGUI text)
    {
        // 텍스트 페이드 아웃 애니메이션
        float elapsed = 0f;
        while (elapsed < textFadeDuration)
        {
            elapsed += Time.deltaTime;
            text.alpha = Mathf.Lerp(1, 0, elapsed / textFadeDuration);
            yield return null;
        }
        text.alpha = 0;
    }
}