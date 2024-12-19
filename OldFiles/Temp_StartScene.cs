using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.Video;

public class Temp_StartScene : MonoBehaviour
{
    private Button StartButton;//스타트 버튼
    private Button ExitButton;// 익시트 버튼

    public VideoPlayer vdo;//비디오 플레이어
    public AudioSource ado;//오디오소스
    private VisualElement root;

    void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;//버튼들
        

        StartButton = root.Q<Button>("button-start");
        ExitButton = root.Q<Button>("button-exit");

        StartButton.clicked += StartButtonClicked;
        ExitButton.clicked += ExitButtonClicked;

        vdo.loopPointReached += OnVideoFinished;// 비디오플레이어가 종료되었는지 알기 위해 looppointreached이벤트에 핸들러 추가
        
    }

     void OnDisable()
    {
        StartButton.clicked -= StartButtonClicked;
        ExitButton.clicked -= ExitButtonClicked;
        vdo.loopPointReached -= OnVideoFinished;//핸들러 제거
    }

    private void StartButtonClicked()
    {
        root.style.display = DisplayStyle.None; // 스타트 버튼 클릭 시 ui 비활성화.
        vdo.Prepare();//비디오 준비 시작
        vdo.prepareCompleted += OnVideoPrepared;//비디오 준비 완료 이벤트에 핸들러 추가
        
     
    }
    
    private void OnVideoPrepared(VideoPlayer soucre )//비디오가 준비되면 비디오, 오디오 재생
    {
        vdo.prepareCompleted -= OnVideoPrepared;
        vdo.Play();
        ado.Play();
    }

    // 스타트 버튼 클릭 시 컷신 영상 재생 후 메인 씬으로 진입.
    private void OnVideoFinished(VideoPlayer vdo)
    {
        ado.Stop();//오디오 정지
        SceneManager.LoadScene("UndergroundScene");
    }

    private void ExitButtonClicked()
    {
        Application.Quit();
    }


}
