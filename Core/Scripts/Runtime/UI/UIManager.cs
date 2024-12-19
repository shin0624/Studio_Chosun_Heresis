using Core.Input;
using SaveSystem.Scripts.Runtime;
using SaveSystem.Scripts.Runtime.Channels;
using SceneManagement;
//using UI.LoadingScreen;
using UnityEngine;

namespace Core.Scripts.Runtime.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private InputReader m_InputReader;
        [SerializeField] private PauseMenuUI m_PauseMenu;
       // [SerializeField] private InGame m_InGame;
        [SerializeField] private GameData m_GameData;
        [SerializeField] private SaveDataChannel m_SaveDataChannel;
        [SerializeField] private SceneReference m_MainMenuScene;
        [SerializeField] private LoadSceneChannel m_LoadSceneChannel;
        // [SerializeField] private SceneReadyChannel m_SceneReadyChannel;
        // [SerializeField] private LoadingScreen m_LoadingScreen;

      //  private InGame m_InGame;

        // private void Start()
        // {
        // m_InGame.gameObject.SetActive(true);

        // }

        //  private void Update()
        // { 


        // }

        private void Save()
        {
            m_SaveDataChannel.Save();
            m_GameData.SaveToBinaryFile();
        }

        private void OnEnable()
        {
            m_InputReader.paused += OnPause;
            m_InputReader.unpaused += OnUnpause;
            m_PauseMenu.resumed += OnUnpause;
            m_PauseMenu.openedMainMenu += OpenMainMenu;
          //  m_LoadSceneChannel.load += OnLoadScene;
           // m_SceneReadyChannel.ready += OnSceneReady;
        }

        private void OnDisable()
        {
            m_InputReader.paused -= OnPause;
            m_InputReader.unpaused -= OnUnpause;
            m_PauseMenu.resumed -= OnUnpause;
            m_PauseMenu.openedMainMenu -= OpenMainMenu;
          //  m_LoadSceneChannel.load -= OnLoadScene;
          //  m_SceneReadyChannel.ready -= OnSceneReady;
        }

        private void OnSceneReady()
        {
            //m_LoadingScreen.gameObject.SetActive(false);
        }

        private void OnLoadScene(SceneReference sceneReference)
        {
            //m_LoadingScreen.gameObject.SetActive(true);
        }

        private void OnPause()
        {
            m_PauseMenu.gameObject.SetActive(true);
           // m_InGame.gameObject.SetActive(false);
            m_InputReader.EnableUIInput();
        }

        private void OnUnpause()
        {
            m_PauseMenu.gameObject.SetActive(false);
            m_InputReader.EnableGameplayInput();
        }

        private void OpenMainMenu()
        {
            Save();
            m_LoadSceneChannel.Load(m_MainMenuScene);
            m_PauseMenu.gameObject.SetActive(false);
            m_InputReader.EnableGameplayInput();

        }
    }
}