using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

namespace UI.Settings
{
    public class AudioSettingsElement : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<AudioSettingsElement, UxmlTraits> { }

        private Slider m_MasterVolumeSlider;
        private Slider m_MusicVolumeSlider;
        private Slider m_SfxVolumeSlider;

        private const string k_MasterVolume = "MasterVolume";
        private const string k_MusicVolume = "MusicVolume";
        private const string k_SfxVolume = "SFXVolume";

        public AudioSettingsElement()
        {
            VisualTreeAsset visualTree = Resources.Load<VisualTreeAsset>("UI/Settings/AudioSettings");
            visualTree.CloneTree(this);

            m_MasterVolumeSlider = this.Q<Slider>("master-volume-slider");
            m_MusicVolumeSlider = this.Q<Slider>("music-volume-slider");
            m_SfxVolumeSlider = this.Q<Slider>("sfx-volume-slider");

            m_MasterVolumeSlider.value = PlayerPrefs.HasKey(k_MasterVolume) ? PlayerPrefs.GetFloat(k_MasterVolume) : 1f;
            m_MusicVolumeSlider.value = PlayerPrefs.HasKey(k_MusicVolume) ? PlayerPrefs.GetFloat(k_MusicVolume) : 1f;
            m_SfxVolumeSlider.value = PlayerPrefs.HasKey(k_SfxVolume) ? PlayerPrefs.GetFloat(k_SfxVolume) : 1f;

            AudioMixer audioMixer = Resources.Load<AudioMixer>("Audio/Main");

            m_MasterVolumeSlider.RegisterValueChangedCallback(evt =>
            {
                audioMixer.SetFloat(k_MasterVolume, Mathf.Log10(evt.newValue) * 20);
            });
            m_MusicVolumeSlider.RegisterValueChangedCallback(evt =>
            {
                audioMixer.SetFloat(k_MusicVolume, Mathf.Log10(evt.newValue) * 20);
            });
            m_SfxVolumeSlider.RegisterValueChangedCallback(evt =>
            {
                audioMixer.SetFloat(k_SfxVolume, Mathf.Log10(evt.newValue) * 20);
            });
        }

        public void Save()
        {
            PlayerPrefs.SetFloat(k_MasterVolume, m_MasterVolumeSlider.value);
            PlayerPrefs.SetFloat(k_MusicVolume, m_MusicVolumeSlider.value);
            PlayerPrefs.SetFloat(k_SfxVolume, m_SfxVolumeSlider.value);
        }

        public void Reset()
        {
            m_MasterVolumeSlider.value = PlayerPrefs.GetFloat(k_MasterVolume);
            m_MusicVolumeSlider.value = PlayerPrefs.GetFloat(k_MusicVolume);
            m_SfxVolumeSlider.value = PlayerPrefs.GetFloat(k_SfxVolume);
        }
    }
}