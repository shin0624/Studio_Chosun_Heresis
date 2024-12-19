using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineController2F : MonoBehaviour
{
    // 2층에서 발생하는 타임라인 이벤트를 위한 스크립트. 문 가까이 가서 E 키를 누르면 타임라인이 실행되고, 타임라인 종료 시 지하 맵으로 이동하게 될 것.

    [SerializeField]
    private PlayableDirector timeline;

    private void Awake()
    {
        timeline = GetComponent<PlayableDirector>();//플레이어블 디렉터 컴포넌트를 찾는다.
        if (timeline == null)
        {
            Debug.Log("Timeline(2F) is NULL!");
        }
    }

    private void OnEnable()
    {
        timeline.stopped += OnTimelineFinished;//타임라인 종료 이벤트 핸들러 추가
    }

    private void OnTriggerStay(Collider other)//충돌상태 + E키가 눌리면 ui가 활성화되어야 하므로, 지속직인 키 입력 감지가 필요.
                                              //OnCollisionEnter는 충돌 순간만 호출되기 떄문에 부적절
    {
        if (other.gameObject.CompareTag("PLAYER"))//플레이어가 가까이 온 후
        {
            if(timeline!=null)
            {
                InputManager.OnInteractKeyPressed += TimelinePlay;// E 키 입력을 이벤트로 처리
            }
            else
            {
                Debug.Log("Timeline(2F) is NULL!");
            }
            

        }
    }

    private void OnTriggerExit(Collider other)//플레이어가 트리거 영역을 벗어났을 시
    {
        if (other.CompareTag("PLAYER"))
        {
            InputManager.OnInteractKeyPressed -= TimelinePlay;
        }
    }

    private void TimelinePlay()//타임라인 플레이 시작
    {
        timeline.Play();
    }

    private void OnTimelineFinished(PlayableDirector director)//타임라인 종료 시 지하 씬으로 이동함.
    {
        LoadingSceneManager.LoadScene("NewUnder3F");//로딩씬을 먼저 호출 후 해당 씬을 호출.
    }
}
