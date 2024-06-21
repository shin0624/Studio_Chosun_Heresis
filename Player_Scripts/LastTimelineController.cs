using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class LastTimelineController : MonoBehaviour
{
    //2층 기도실 -> 엔딩 씬으로 전환되는 스크립트.

    public string targetSceneName = "EndScene";

    public VideoPlayer videoplayer; // 애니메이션 재생용 변수 선언
    private bool playerInRange = false;

    void Start()
    {
        if (videoplayer != null)
        {
            videoplayer.loopPointReached += OnVideoFinished;//애니메이션 재생 종료 시 씬 전환 이벤트 연결.
        }
    }

    void Update()
    {
        if(playerInRange)
        {
            PlayVideo();
            Debug.Log("LastAnimation Start!");
            playerInRange = false;//중복 재생 방지.  
        }     
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PLAYER"))
        {

            Debug.Log("playerinrange = true");
            playerInRange = true;
        }

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PLAYER"))
        {
            Debug.Log("Player exited the trigger.");
            playerInRange = false;
        }
    }

    private void PlayVideo()
    {
        if (videoplayer != null)
        {
            videoplayer.Play();//애니메이션 재생
        }
        else
        {
            Debug.LogWarning("videoPlayer is not assigned");
        }
    }
    private void OnVideoFinished(VideoPlayer vp)//애니메이션 종료 시 씬 전환.
    {
        Debug.Log("Video Finished. change Scene");
        SceneManager.LoadScene(targetSceneName);
    }


}
