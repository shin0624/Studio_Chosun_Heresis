using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class Teleport_from2f_toUnderground : MonoBehaviour
{
    //지상 기도실 -> 지하 예배당 이동 스크립트 (지하->1층 이동 스크립트는 버튼 이벤트를 받으므로 본 스크립트와 따로 구현)

    public string targetSceneName = "UndergroundScene";

    private bool playerInRange = false;//플레이어가 문 가까이 있는지 여부

    private float holdTime = 0f;//플레이어가 문 근처에서 대기하는 시간

    public float requireHoldtime = 2f;//플레이어가 기도방 앞(문 컬라이더와 맞닿은 상태)로 3초 있었다면 지하 씬으로 이동(향후 컷신 추가 예정)

    public VideoPlayer videoplayer; // 애니메이션 재생용 변수 선언
   
    void Start()
    {
        if (videoplayer != null)
        {
            videoplayer.loopPointReached += OnVideoFinished;//애니메이션 재생 종료 시 씬 전환 이벤트 연결.
        }
    }

    void Update()
    {
            if (playerInRange)//플레어가 범위에 있다면
            {
                    holdTime += Time.deltaTime;// 시간이 올라가며 일정 시간에 도달했을 때 지하로 이동 --> 최종본에는 대기 대신 컷신 삽입
                    if (holdTime >= requireHoldtime)
                    {
                        PlayVideo();
                        Debug.Log("TimeLineAnimation Start!");
                        playerInRange= false;//중복 재생 방지.            
                    }
            }
            else
            {
                    holdTime = 0f;//플레이어가 범위에서 벗어나면 다시 초기화
            }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PLAYER"))
        {
            Debug.Log("playerinrange = true");
            playerInRange = true;//플레이어가 문 근처에 접근하면 true

        }
  
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PLAYER"))
        {
            Debug.Log("Player exited the trigger.");
            playerInRange = false;
            holdTime = 0f;
        }
    }

   private void PlayVideo()
    {
        if(videoplayer != null)
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
