using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class GameClearController : MonoBehaviour
{
    // 1층 도착 후 게임 클리어 타임라인 재생 -> 게임 클리어 캔버스 출력 ->  종료

    [SerializeField]
    private PlayableDirector Timeline;

    public int SceneCountStatic; // 씬이 바뀌는 것을 구분할 씬카운트(씬체인지매니저에 있던 static변수)

    private void Awake()
    {
       

        Timeline = GetComponent<PlayableDirector>();
        if(Timeline==null)
        {
            Debug.Log("Clear Timeline is NULL");
        }
    }
    private void Update()
    {
        SceneCountStatic = SceneChangeManager.SceneCount;// 씬 카운트는 원래 정적 변수라, 한번 참조하면 그 값이 유지되어버리니까 업데이트문에서 값 변화를 계속 봐야함
    }

    private void OnEnable()//타임라인 종료 시 씬 전환 이벤트 핸들러 등록
    {
        Timeline.stopped += OnTimelineFinished;
    }

    private void OnDisable()
    {
        Timeline.stopped-= OnTimelineFinished;
    }

    private void OnTriggerEnter(Collider other)// 지상 1층에 두번째로 스폰 시 && 플레이어가 트리거 영역에 닿았을 시 타임라인 재생
    {
        
        if(other.CompareTag("PLAYER") && SceneCountStatic==2 || SceneCountStatic==3 )
        {
            ClearTimelinePlay();
        }
    }

    private void ClearTimelinePlay()
    {
        Timeline.Play();
    }

    private void OnTimelineFinished(PlayableDirector director)//타임라인 종료 시 게임 종료
    {
        Application.Quit();

    }

}
