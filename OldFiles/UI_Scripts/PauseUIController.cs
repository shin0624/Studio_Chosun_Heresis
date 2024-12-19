using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseUIController : MonoBehaviour
{
    public Canvas pauseCanvas;
    public Button backButton;
    public Button exitButton;

    private bool isPaused = false;

    void Start()
    {
        if(pauseCanvas !=null)
        {
            pauseCanvas.gameObject.SetActive(false);//비활성화 상태로 초기화
        }
        else
        {
            Debug.LogError("Pause Canvas not assigned!");
        }

        if(backButton != null)
        {
            backButton.onClick.AddListener(OnBackButtonClicked);
        }
        else
        {
            Debug.LogError("BackButton not assigned!");
        }

        if(exitButton != null)
        {
            exitButton.onClick.AddListener(OnExitButtonClicked);
        }
        else
        {
            Debug.LogError("ExitButton not assigned!");
        }
    }

   
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
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
    void PauseGame()
    {
        isPaused = true;
        if(pauseCanvas!=null)
        {
            pauseCanvas.gameObject.SetActive(true);
        }
        Time.timeScale = 0f;
        Debug.Log("game paused");
    }

    void ResumeGame()
    {
        isPaused = false;
        if(pauseCanvas!=null)
        {
            pauseCanvas.gameObject.SetActive(false);
        }
        Time.timeScale = 1f;
        Debug.Log("game resumed");
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
        // 이벤트 핸들러 정리
        if (backButton != null)
        {
            backButton.onClick.RemoveListener(OnBackButtonClicked);
        }
        if (exitButton != null)
        {
            exitButton.onClick.RemoveListener(OnExitButtonClicked);
        }
    }
}
