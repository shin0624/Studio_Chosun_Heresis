using System;
using UnityEngine;

namespace SaveSystem.Scripts.Runtime.Channels
{
    [CreateAssetMenu(fileName = "LoadDataChannel", menuName = "Channels/LoadDataChannel", order = 0)]
    public class LoadDataChannel : ScriptableObject
    {
        public event Action load;

        public void Load()
        {
            load?.Invoke();
        }
    }
}