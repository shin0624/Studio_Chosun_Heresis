using System;
using UnityEngine;

namespace Core.Scripts.Runtime
{
    public class SettingsManager : MonoBehaviour
    {
        private const string k_FullScreen = "FullScreen";
        private const string k_ResolutionIndex = "ResolutionIndex";
        private void Awake()
        {
            Resolution currentResolution = Screen.resolutions[PlayerPrefs.GetInt(k_ResolutionIndex)];
            Screen.fullScreen = Convert.ToBoolean(PlayerPrefs.GetInt(k_FullScreen));
            Screen.SetResolution(currentResolution.width, currentResolution.height, Screen.fullScreen);
        }
    }
}