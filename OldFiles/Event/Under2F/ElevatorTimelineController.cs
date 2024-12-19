using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class ElevatorTimelineController : MonoBehaviour
{
    //엘리베이터 컷신 재생 및 y축 값 증가로 엘리베이터 작동을 제어하는 스크립트. elevator 오브젝트에 부착됨
    //타임라인 재생(엘리베이터 올라가는 컷신) -> 플레이어블 디렉터의 Wrap Mode를 활성화하여, 컷신 종료 후에도 포지션 키값이 원래대로 돌아가지 않게 함 -> 플레이어가 타고 올라가야 하므로, 리지드바디를 추가 후 컷신 종료 시 포지션 고정

    [SerializeField]
    private PlayableDirector timeline;
    [SerializeField]
    private Camera cam2;// 시네머신 가상카메라를 할당하고 조절하여, 타임라인이 종료된 후에 가상카메라도 비활성화.

    private Rigidbody rb; // 타임라인 종료 후에도 엘리베이터의 포지션이 고정되어야 하므로,
   
    private void Awake()
    {
        timeline = GetComponent<PlayableDirector>();
        rb = gameObject.GetComponent<Rigidbody>();

        if(timeline==null)
        {
            Debug.Log(" Elevator Event Timeline is null !");
        }
        if(rb==null)
        {
            Debug.Log(" Elevator RigidBody is null !");
        }
    }

    private void OnEnable()
    {
        timeline.stopped += OnTimelineFinished;
        

    }

    private void OnDisable()// 이벤트 구독 해제
    {
        timeline.stopped -= OnTimelineFinished;//타임라인 종료 이벤트 핸들러 추가
    }

    private void OnTriggerEnter(Collider other)//플레이어가 엘리베이터 콜라이더에 접근 시 활성화 
    {
        if(other.gameObject.CompareTag("PLAYER"))
        {
            Debug.Log("Player In HERE!");
            TimelinePlay();//타임라인 재생

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("PLAYER"))
        {
            Debug.Log("Player left the elevator area.");
        }
    }


    private void TimelinePlay()// 타임라인 플레이 시작
    {
        timeline.Play();
        Debug.Log("elevator timeline start");
    }

    private void OnTimelineFinished(PlayableDirector director)//타임라인 종료 시 엘리베이터의 포지션 고정
    {
        rb.constraints = RigidbodyConstraints.FreezePositionY;

        if(cam2 != null)
        {
            cam2.gameObject.SetActive(false);//컷신이 끝나도 가상카메라가 활성화 상태이면 비활성화로 바꿈.
        }


    }

}
