using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace SceneManagement
{
    public class Entrypoint : MonoBehaviour
    {
        [SerializeField] private SceneReference m_ManagersScene;
        [SerializeField] private SceneReference m_MainMenuScene;
        [SerializeField] private AssetReferenceT<LoadSceneChannel> m_LoadSceneChannel;
   
        private IEnumerator Start()
        {
            yield return m_ManagersScene.LoadSceneAsync(LoadSceneMode.Additive);
            var handle = m_LoadSceneChannel.LoadAssetAsync<LoadSceneChannel>();
            yield return handle;
            handle.Result.Load(m_MainMenuScene);
            SceneManager.UnloadSceneAsync(0);
        }
    
    }
}