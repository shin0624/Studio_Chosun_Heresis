using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Settings
{
    public class SettingsElement : VisualElement
    {
        private AudioSettingsElement m_AudioSettings;
        private VisualSettingsElement m_VisualSettings;
        private Button m_GraphicsTab;
        private Button m_AudioTab;

        public new class UxmlFactory : UxmlFactory<SettingsElement, UxmlTraits> { }

        public SettingsElement()
        {
            VisualTreeAsset visualTree = Resources.Load<VisualTreeAsset>("UI/Settings/Settings");
            visualTree.CloneTree(this);

            m_AudioSettings = this.Q<AudioSettingsElement>();
            m_VisualSettings = this.Q<VisualSettingsElement>();

            Button saveButton = this.Q<Button>("save-button");
            saveButton.clicked += Save;
            Button resetButton = this.Q<Button>("reset-button");
            resetButton.clicked += Reset;

            m_GraphicsTab = this.Q<Button>("graphics-tab");
            m_GraphicsTab.clicked += OnClickedGraphicsTab;
            m_AudioTab = this.Q<Button>("audio-tab");
            m_AudioTab.clicked += OnClickedAudioTab;

            OnClickedGraphicsTab();
        }

        private void OnClickedAudioTab()
        {
            m_VisualSettings.style.display = DisplayStyle.None;
            m_AudioSettings.style.display = DisplayStyle.Flex;
            m_GraphicsTab.SetEnabled(true);
            m_AudioTab.SetEnabled(false);
        }

        private void OnClickedGraphicsTab()
        {
            m_VisualSettings.style.display = DisplayStyle.Flex;
            m_AudioSettings.style.display = DisplayStyle.None;
            m_GraphicsTab.SetEnabled(false);
            m_AudioTab.SetEnabled(true);
        }

        private void Save()
        {
            m_AudioSettings.Save();
            m_VisualSettings.Save();
        }

        private void Reset()
        {
            m_AudioSettings.Reset();
            m_VisualSettings.Reset();
        }
    }
}