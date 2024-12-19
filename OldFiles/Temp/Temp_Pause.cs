using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Temp_Pause : MonoBehaviour
{
    [SerializeField]
    private GameObject pauseMenu; // Inspector에서 직접 참조하도록 변경
    private bool isPaused = false;

    void Start()
    {
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
            Debug.Log("Pause menu initialized and set to inactive.");
        }
        else
        {
            Debug.LogError("Pause menu not assigned! Please assign the Temp_PauseUI GameObject in the Inspector.");
        }
    }

    void Update()
    {
        // Esc 키 입력 처리
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape key pressed.");
            if (!isPaused)
            {
                PauseGame();
            }
            else
            {
                ExitGame();
            }
        }

        // Space 키 입력 처리
        if (isPaused && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space key pressed.");
            ResumeGame();
        }
    }

    private void PauseGame()
    {
        if (pauseMenu != null)
        {
            isPaused = true;
            pauseMenu.SetActive(true);
            Time.timeScale = 0f; // 게임을 일시 정지합니다.
            Debug.Log("Game paused.");
        }
    }

    private void ResumeGame()
    {
        if (pauseMenu != null)
        {
            isPaused = false;
            pauseMenu.SetActive(false);
            Time.timeScale = 1f; // 게임을 재개합니다.
            Debug.Log("Game resumed.");
        }
    }

    private void ExitGame()
    {
        Debug.Log("Exiting game.");
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
