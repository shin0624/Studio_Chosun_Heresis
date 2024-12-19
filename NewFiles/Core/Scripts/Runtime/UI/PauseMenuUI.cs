using System;
using UI.Settings;
using UnityEngine;
using UnityEngine.UIElements;

namespace Core.Scripts.Runtime.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class PauseMenuUI : MonoBehaviour
    {
        private UIDocument m_UIDocument;
        private SettingsElement m_Settings;
        public event Action resumed;
        public event Action openedMainMenu;
        

        private void Awake()
        {
            m_UIDocument = GetComponent<UIDocument>();
        }

        private void OnEnable()
        {
            var root = m_UIDocument.rootVisualElement;
            m_Settings = root.Q<SettingsElement>();

            Button resumeButton = root.Q<Button>("resume-button");
            resumeButton.clicked += Resume;

            Button mainMenuButton = root.Q<Button>("main-menu-button");
            mainMenuButton.clicked += OpenMainMenu;

            //Button closeButton = root.Q<Button>("close-button");
            //closeButton.clicked += Resume;

            Button settingsButton = root.Q<Button>("settings-button");
            settingsButton.clicked += OpenSettings;

            //Button closeSettingsButton = m_Settings.Q<Button>("close-button");
           // closeSettingsButton.clicked += CloseSettings;
            Time.timeScale = 0;
        }

        private void CloseSettings()
        {
            m_Settings.style.display = DisplayStyle.None;
        }

        private void OpenSettings()
        {
            m_Settings.style.display = DisplayStyle.Flex;
        }

        private void OpenMainMenu()
        {
            openedMainMenu?.Invoke();
        }

        private void OnDisable()
        {
            Time.timeScale = 1;
        }

        private void Resume()
        {
            resumed?.Invoke();
        }
    }
}