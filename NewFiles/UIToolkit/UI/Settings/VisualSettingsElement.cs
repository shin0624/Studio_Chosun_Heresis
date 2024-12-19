using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Settings
{
    public class VisualSettingsElement : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<VisualSettingsElement, UxmlTraits> { }

        private Label m_ResolutionLabel;
        private Label m_FullScreenLabel;

        private const string k_ResolutionIndex = "ResolutionIndex";
        private const string k_FullScreen = "FullScreen";

        private int m_CurrentResolutionIndex;
        private bool m_IsFullScreen;

        public VisualSettingsElement()
        {
            VisualTreeAsset visualTree = Resources.Load<VisualTreeAsset>("UI/Settings/VisualSettings");
            visualTree.CloneTree(this);

            m_ResolutionLabel = this.Q<Label>("resolution-setting-value");
            m_FullScreenLabel = this.Q<Label>("full-screen-setting-value");
            Button previousResolutionButton = this.Q<Button>("resolution-previous-button");
            previousResolutionButton.clicked += PreviousResolution;
            Button nextResolutionButton = this.Q<Button>("resolution-next-button");
            nextResolutionButton.clicked += NextResolution;
            Button previousFullScreenButton = this.Q<Button>("full-screen-previous-button");
            previousFullScreenButton.clicked += PreviousFullScreen;
            Button nextFullScreenButton = this.Q<Button>("full-screen-next-button");
            nextFullScreenButton.clicked += NextFullScreen;

            m_IsFullScreen = Screen.fullScreen;
            SetFullScreenField();

            if (PlayerPrefs.HasKey(k_ResolutionIndex))
            {
                m_CurrentResolutionIndex = PlayerPrefs.GetInt(k_ResolutionIndex);
            }
            else
            {
                m_CurrentResolutionIndex = Array.FindIndex(Screen.resolutions,
                    resolution => resolution.width == Screen.currentResolution.width &&
                                  resolution.height == Screen.currentResolution.height);
            }
            OnResolutionChanged();
        }

        public void Save()
        {
            PlayerPrefs.SetInt(k_ResolutionIndex, m_CurrentResolutionIndex);
            PlayerPrefs.SetInt(k_FullScreen, Convert.ToInt32(m_IsFullScreen));
        }

        public void Reset()
        {
            m_CurrentResolutionIndex = PlayerPrefs.GetInt(k_ResolutionIndex);
            OnResolutionChanged();
            m_IsFullScreen = Convert.ToBoolean(PlayerPrefs.GetInt(k_FullScreen));
            OnFullScreenChanged();
        }

        private void NextFullScreen()
        {
            m_IsFullScreen = true;
            OnFullScreenChanged();
        }

        private void PreviousFullScreen()
        {
            m_IsFullScreen = false;
            OnFullScreenChanged();
        }

        private void OnFullScreenChanged()
        {
            Screen.fullScreen = m_IsFullScreen;
            SetFullScreenField();
        }

        private void SetFullScreenField()
        {
            m_FullScreenLabel.text = m_IsFullScreen ? "On" : "Off";
        }

        private void NextResolution()
        {
            m_CurrentResolutionIndex = Mathf.Clamp(m_CurrentResolutionIndex + 1, 0, Screen.resolutions.Length - 1);
            OnResolutionChanged();
        }

        private void PreviousResolution()
        {
            m_CurrentResolutionIndex = Mathf.Clamp(m_CurrentResolutionIndex - 1, 0, Screen.resolutions.Length - 1);
            OnResolutionChanged();
        }

        private void OnResolutionChanged()
        {
            Resolution resolution = Screen.resolutions[m_CurrentResolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, m_IsFullScreen);
            SetResolutionField();
        }

        private void SetResolutionField()
        {
            string displayText = Screen.resolutions[m_CurrentResolutionIndex].ToString();
            displayText = displayText.Substring(0, displayText.IndexOf("@"));
            m_ResolutionLabel.text = displayText;
        }
    }
}