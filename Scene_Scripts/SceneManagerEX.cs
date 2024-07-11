using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

// 씬 전환시 변경값들의 저장을 담당하는 스크립트. 1층 -> 지하 -> 1층으로 씬 전환 시, 서로 다른 위치에서 스폰되어야 하기 때문에 관리가 필요
public class SceneManagerEX : MonoBehaviour
{
    public static SceneManagerEX SceneManagerInstance { get; private set; }//씬 매니저 인스턴스를 선언.
    public static int SceneCount { get; private set; } = 0;//초기화->SceneManagerEX클래스가 처음 로드될 때에만 사용
    public  GameObject player;

    public Vector3 LastSpawnPosition = new Vector3(-16.9419994f, -2.5f, 25.1469994f);//최종 스폰 위치
    public Vector3 FirstSpawnPosition = new Vector3(-57.2439995f, -2.38499999f, 16.757f);//첫 스폰 위치

    public Vector3 UnderSpawnPosition = new Vector3(-303.083008f, -43.3740005f, -18.6930008f);//지하스폰포지션
    public Quaternion UnderSpawnRotation =   Quaternion.Euler(271.706604f, 20.3141384f, 161.09494f);//지하스폰로테이션
    public Vector3 UnderSpawnScale = new Vector3(0.35261631f, 0.683449626f, 0.422265708f);//지하스폰스케일

    public void Awake()//초기화
    {
      
       if(SceneManagerInstance ==null)
        {
            SceneManagerInstance = this;
            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += OnSceneLoaded;//이벤트핸들러 등록 
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnDestroy()
    {
        if(SceneManagerInstance==this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;//이벤트 핸들러 해제
        }
    }
    private void OnEnable()
    {
        if (SceneManager.GetActiveScene().name == "UndergroundScene")
        {
            player.transform.position = UnderSpawnPosition;
            Under_TimelineController.Under_TimelineController_Instance?.PlayTimeline();
        }

    }


    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //1층 씬이 로드될 때 씬 카운트 값 증가 및 스폰 위치 설정
        if(scene.name== "1_2FloorScene")
        {
            SceneCount++;//씬카운트는 씬이 로드될 때에만 증가할 것.
            Debug.Log($"scenecount : {SceneCount}");

            if(SceneCount==2)
            {
                player.transform.position = LastSpawnPosition;
                


            }
            else if(SceneCount==1)
            {
                player.transform.position = FirstSpawnPosition;
            }
        }
        else if(scene.name == "UndergroundScene")
        {
            
            player.transform.position = UnderSpawnPosition;
            Under_TimelineController.Under_TimelineController_Instance?.PlayTimeline();
        }
    }


}
