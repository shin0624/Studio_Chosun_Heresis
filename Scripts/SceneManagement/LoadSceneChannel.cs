using System;
using UnityEngine;

namespace SceneManagement
{
    [CreateAssetMenu(fileName = "LoadSceneChannel", menuName = "Channels/LoadSceneChannel", order = 0)]
    public class LoadSceneChannel : ScriptableObject
    {
        public event Action<SceneReference> load;

        public void Load(SceneReference sceneReference)
        {
            load?.Invoke(sceneReference);
        }
    }
}