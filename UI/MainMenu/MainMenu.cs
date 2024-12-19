using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI.PlainButton;
using UnityEngine.UIElements;
using System;
using SceneManagement;
using SaveSystem.Scripts.Runtime;
using SaveSystem.Scripts.Runtime.Channels;
using UnityEngine.Playables;

[RequireComponent(typeof(UIDocument))]

public class main : MonoBehaviour
{
    private UIDocument m_UIDocument;
    private VisualElement m_ConfirmationModal;
    private VisualElement m_Credits;
    [SerializeField] private LoadSceneChannel m_LoadSceneChannel;
    [SerializeField] private SceneReference m_StartingLocation;
    [SerializeField] private GameData m_GameData;
    [SerializeField] private LoadDataChannel m_LoadDataChannel;

    [SerializeField] private PlayableDirector timeline; // 스타트 시 재생될 타임라인 디렉터
    [SerializeField] private AudioSource ado;//시작 기본 배경음
    private VisualElement root;


    private void Awake()
    {
      m_UIDocument = GetComponent<UIDocument>();

        if(timeline==null)
        {
            Debug.LogError("PlayableDirector component not assigned!");
            return;
        }

        if(ado==null)
        {
            Debug.LogError("AudioSource component not assigned!");
        }
    }

    private void OnEnable()
    {
        //게임 이어서하기: continue-버튼
        root = m_UIDocument.rootVisualElement;
        PlainButton continueButton = m_UIDocument.rootVisualElement.Q<PlainButton>(name: "button-continue");
        continueButton.SetEnabled(m_GameData.hasPreviousSave);
        continueButton.clicked += ContinuePreviouseGame;


        //------------------------------------------------------------------------------------------------------

        //게임 종료: exit-버튼
        PlainButton exitButton = root.Q<PlainButton>("button-exit");
        exitButton.clicked += ShowConfirmationModal;
        //exitButton.clicked += () => Debug.Log("exit test"); 버튼 호출 테스트

        //게임 종료버튼 클릭시 한번 더 의사를 물어보는 팝업창
        m_ConfirmationModal = root.Q("confirmation-modal");

        //게임 종료하기: quit-버튼
        Button confirmation = m_ConfirmationModal.Q<Button>("button-quit");
        confirmation.clicked += QuitGame;

        //게임 종료 하지 않고 계속하기: concle-버튼
        Button cancelButton=m_ConfirmationModal.Q<Button>("button-cancle");
        cancelButton.clicked += Cancel;

        //------------------------------------------------------------------------------------------------------

        //new 게임 생성 : new-game-버튼
        Button newGameButton = root.Q<PlainButton>("button-new-game");
        newGameButton.clicked += StartNewGame;


        //------------------------------------------------------------------------------------------------------

        //credit menu: credit 버튼

        PlainButton creditButton = root.Q<PlainButton>("button-credits");
        m_Credits = root.Q("credit-modal");
        creditButton.clicked += OpenCredits;

        //credit menu close button: 크레딧 화면의 닫기 버튼
        Button closeCreditsButton = m_Credits.Q<Button>("button-close");
        closeCreditsButton.clicked += CloseCredits;

        timeline.stopped += OnTimelineFinished;//타임라인 종료 이벤트 핸들러 추가
        ado.Play();
    }

    private void OnDisable()
    {
        timeline.stopped -= OnTimelineFinished;//이벤트 핸들러 제거
    }

    private void OpenCredits()
    {
        m_Credits.style.display = DisplayStyle.Flex;

    }

    private void CloseCredits()
    {
        m_Credits.style.display = DisplayStyle.None;

    }

    private void ContinuePreviouseGame()
    {
        m_GameData.LoadFromBinaryFile();
        m_LoadDataChannel.Load();

    }
    private void StartNewGame()
    {
        //LoadingSceneManager.LoadScene("New1_2FloorScene");
        // m_LoadSceneChannel.Load(m_StartingLocation);

       root.style.display= DisplayStyle.None;//스타트버튼 클릭 시 ui비활성화.
        ado.Stop();
        timeline.Play();//스타트버튼 클릭 시 타임라인 재생
    }

    //게임 종료 팝업창 띄우기
    private void ShowConfirmationModal()
    {
        m_ConfirmationModal.style.display = DisplayStyle.Flex;
    }
    //게임 종료 팝업창 숨기기
    private void Cancel()
    {
        m_ConfirmationModal.style.display= DisplayStyle.None;
    }
    //게임 종료시키기 
    private void  QuitGame()
    { 
        m_ConfirmationModal.style.display=DisplayStyle.None;
        Application.Quit();
    }

    private void OnTimelineFinished(PlayableDirector director)
    {
        LoadingSceneManager.LoadScene("New1_2FloorScene");//로딩씬을 먼저 호출 후 해당 씬을 호출.
    }
  

}
