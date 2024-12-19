using UnityEngine;
using UnityEngine.UIElements;

public class Temp_Pause : MonoBehaviour
{
    private UIDocument uiDocument;
    private VisualElement root;
    private Button backButton;
    private Button exitButton;
    private bool isPaused = false;

    void Start()
    {
        // UIDocument 참조 가져오기
        uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null)
        {
            Debug.LogError("UIDocument component not found!");
            return;
        }

        // UXML 파일의 root VisualElement 가져오기
        root = uiDocument.rootVisualElement;

        // BackButton 찾기 및 클릭 이벤트 핸들러 등록
        backButton = root.Q<Button>("BackButton");
        if (backButton != null)
        {
            backButton.clickable.clicked += OnBackButtonClicked;
        }
        else
        {
            Debug.LogError("BackButton not found in the UXML.");
        }

        // ExitButton 찾기 및 클릭 이벤트 핸들러 등록
        exitButton = root.Q<Button>("ExitButton");
        if (exitButton != null)
        {
            exitButton.clickable.clicked += OnExitButtonClicked;
        }
        else
        {
            Debug.LogError("ExitButton not found in the UXML.");
        }

        // 초기에는 Pause UI 비활성화
        root.style.display = DisplayStyle.None;
    }

    void Update()
    {
        // ESC 키 입력 처리
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    void PauseGame()
    {
        isPaused = true;
        root.style.display = DisplayStyle.Flex;
        Time.timeScale = 0f; // 게임 일시 정지
        Debug.Log("Game paused.");
    }

    void ResumeGame()
    {
        isPaused = false;
        root.style.display = DisplayStyle.None;
        Time.timeScale = 1f; // 게임 재개
        Debug.Log("Game resumed.");
    }

    void OnBackButtonClicked()
    {
        Debug.Log("BackButton clicked.");
        ResumeGame(); // 일시 정지 해제
    }

    void OnExitButtonClicked()
    {
        Debug.Log("ExitButton clicked.");
        Application.Quit(); // 게임 종료
    }

    void OnDestroy()
    {
        // 게임 오브젝트가 파괴될 때 이벤트 핸들러 정리
        if (backButton != null)
        {
            backButton.clickable.clicked -= OnBackButtonClicked;
        }
        if (exitButton != null)
        {
            exitButton.clickable.clicked -= OnExitButtonClicked;
        }
    }
}
