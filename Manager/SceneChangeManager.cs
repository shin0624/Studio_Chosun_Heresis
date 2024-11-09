using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeManager : MonoBehaviour
{
    // 지상 씬 -> 지하 씬 -> 지상 씬으로 씬이 전환될 때, 반복되는 씬에서의 스폰 위치를 관리. 씬 카운트를 정적선언하여 값을 저장하고 이 값에 따라 반복되는 씬에서의 스폰위치를 결정한다.
    // 스폰 위치는 하드코딩이 아닌 spawnPoint라는 빈 오브젝트를 기준으로 설정. 스폰하고자 하는 위치에 빈 오브젝트를 놓는다.
    // Managers 클래스에 선언된 SceneChangeManager 타입 싱글톤 인스턴스에 접근하여 사용
    
    public static int SceneCount { get; private set; } = 0; // 본 클래스가 처음 로드될 때 사용
    [SerializeField]
    private GameObject player;

    private Vector3 UnderFloorPlayer = new Vector3(0.3f, 0.4f, 0.4f);


    public void Awake()
    {
        //Managers 클래스의 SceneChangeManager 인스턴스 참조
        SceneChangeManager sceneChangeManager = Managers.SceneChange;
        //씬이 로드될 때 호출되는 이벤트 등록
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)//씬 전환 시 플레이어 스폰 처리
    {
        SceneCount++;
        Debug.Log("SceneCount : " + SceneCount);
        GameObject SpawnPoint;

        if(scene.name=="New1_2FloorScene")
        {
            if(SceneCount==1)
            {
                SpawnPoint = GameObject.Find("FirstSpawnPoint");
            }
            else
            {
                SpawnPoint = GameObject.Find("LastSpawnPoint");  
            }
            SPSetting(SpawnPoint);//스폰 시 플레이어의 로테이션 포지션을 스폰장소에 맞춘다.
        }
        if(scene.name=="NewUnder3F")
        {
            SpawnPoint  = GameObject.Find("SecondSpawnPoint");
            SPSettingUnder(SpawnPoint);//지하는 플레이어 스케일 변경이 필요하므로 메서드를 따로 추가
        }
        if(scene.name =="NewUnderAfter")
        {
            SpawnPoint  = GameObject.Find("ThirdSpawnPoint");
            SPSettingUnder(SpawnPoint);
        }
    }

    public void SPSettingUnder(GameObject sp)
    {
          if(sp != null)
            {
                player.transform.position = sp.transform.position;
                player.transform.rotation = sp.transform.rotation;
                player.transform.localScale = UnderFloorPlayer;
                Debug.Log($"Spawn Point : {sp.name}");
             }
    }
    public void SPSetting(GameObject sp)
    {
         if (sp != null)//스폰포인트가 존재하면 플레이어 위치 이동
            {
                player.transform.position = sp.transform.position;
                player.transform.rotation = sp.transform.rotation;
                Debug.Log($"Spawn Point : {sp.name}");
            }
    }
}
