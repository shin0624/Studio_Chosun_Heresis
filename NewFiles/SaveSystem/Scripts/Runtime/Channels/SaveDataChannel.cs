using System;
using UnityEngine;

namespace SaveSystem.Scripts.Runtime.Channels
{
    [CreateAssetMenu(fileName = "SaveDataChannel", menuName = "Channels/SaveDataChannel", order = 0)]
    public class SaveDataChannel : ScriptableObject
    {
        public event Action save;

        public void Save()
        {
            save?.Invoke();
        }
    }
}