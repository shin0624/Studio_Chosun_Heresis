using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PauseUICONTROLLER_UITOOLKIT : MonoBehaviour
{
    //일시정지 ui 제어 스크립트. 
    private Button BackButton;//백 버튼
    private Button SettingsButton;// 셋팅 버튼
    private Button ExitButton;//익시트 버튼

    private VisualElement root;
    private bool isPaused = false;

    private void Awake()
    {
        if (GetComponent<UIDocument>() == null)
        {
            Debug.LogError("UIDocument component not found!");
            return;
        }
    }

    void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;//버튼들


        BackButton = root.Q<Button>("button-Back");
        SettingsButton = root.Q<Button>("button-settings");
        ExitButton = root.Q<Button>("button-Exit");

        BackButton.clicked += BackButtonClicked;
        SettingsButton.clicked += SettingsButtonClicked;
        ExitButton.clicked += ExitButtonClicked;

        root.style.display = DisplayStyle.None;//처음에는 ui를 숨긴다.
    }
    void OnDisable()
    {
        BackButton.clicked -= BackButtonClicked;
        SettingsButton.clicked -= SettingsButtonClicked;
        ExitButton.clicked -= ExitButtonClicked;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))//esc버튼 클릭으로 ui 활성화
        {
            if(isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
    private void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;//게임 시간 일시 정지
        root.style.display = DisplayStyle.Flex;//ui를 표시한다. 즉  esc버튼을 클릭함.
        UnityEngine.Cursor.lockState = CursorLockMode.None;//마우스 커서를 ui에서 고정 해제
        UnityEngine.Cursor.visible = true;//마우스 커서를 보이도록 설정
    
    }

    private void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;//게임재개
        root.style.display = DisplayStyle.None;//ui를 숨긴다.
       UnityEngine.Cursor.lockState = CursorLockMode.Locked;//마우스 커서를 ui에서 고정
        UnityEngine.Cursor.visible = false;//마우스 커서를 숨김

    }

    private void BackButtonClicked()
    {
        Debug.Log("Back button clicked");
        ResumeGame();
    }

    private void SettingsButtonClicked()
    {
        Debug.Log("Settings button clicked");
    }

    private void ExitButtonClicked() 
    {
        Debug.Log("Exit button clicked");
        Application.Quit();
    }


}
