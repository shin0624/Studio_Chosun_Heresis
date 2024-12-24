using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using SceneManagement;

public class PauseUIManager : MonoBehaviour
{
     [SerializeField] private UIDocument m_UIDocument;
    void Start()
    {
        var root = m_UIDocument.rootVisualElement;

        Button resumeButton = root.Q<Button>("resume-button");
        resumeButton.clicked += UIClose;

        Button mainMenuButton = root.Q<Button>("main-menu-button");
        mainMenuButton.clicked += OpenMainMenu;

        root.style.display = DisplayStyle.None; // 초기 UI 숨김
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            var root = m_UIDocument.rootVisualElement;

            if (root.style.display == DisplayStyle.None)
                UIOpen(); // 팝업 열기
            else
                UIClose(); // 팝업 닫기
        }
    }

    void UIOpen()
    {
        var root = m_UIDocument.rootVisualElement;
        root.style.display = DisplayStyle.Flex; // UI 표시
        Time.timeScale = 0; // 게임 시간 정지
    }

    void UIClose()
    {
        var root = m_UIDocument.rootVisualElement;
        root.style.display = DisplayStyle.None; // UI 숨김
        Time.timeScale = 1; // 게임 시간 재개
    }

    void OpenMainMenu()
    {
        Time.timeScale = 1; // 게임 시간 재개

    }
}


