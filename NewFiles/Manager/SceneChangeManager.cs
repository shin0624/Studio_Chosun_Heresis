using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeManager : MonoBehaviour
{
    [SerializeField]
    private GameObject player;
    
    private Vector3 UnderFloorPlayer = new Vector3(0.4f, 0.7f, 0.4f);
    private Vector3 Under3FPlayer = new Vector3(1.0f, 1.6f, 0.9f);
    //private quaternion Under3FPlayerRotation = Quaternion.Euler(-90.0f,0f,90f );
    public static int SceneCount = 0;
    public int GetCount {get {return SceneCount;}}
    private void Awake()
    {
        SceneCount = PlayerPrefs.GetInt("SceneCount", 0);// 이전 씬 카운트 값을 로드. 없으면 기본값 0.
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) // 씬 이동 순서 : New1_2FloorScene -> NewUnder3F -> NewUnderAfter -> New1_2FloorScene -> 엔딩
    {
        GameObject spawnPoint = null;

        // 씬에 따른 스폰 지점 결정
        if(scene.name == "New1_2FloorScene")//지상 병원 층 스폰 시. 같은 씬을 두번 반복하면서 각 스폰 마다 다른 상황이 연출되어야 함
        {
            spawnPoint = GetSavedSpawnPoint("New1_2FloorScene", "FirstSpawnPoint", "LastSpawnPoint");
            SetSpawnPosition(spawnPoint);
        }
        else if(scene.name == "NewUnder3F")//지하 3층(무당집)스폰 시
        {
            spawnPoint = GameObject.Find("SecondSpawnPoint");
            SetSpawnPositionWithScaleUnder3F(spawnPoint);
        }
        else if(scene.name == "NewUnderAfter")//지하 2층(공사현장, 정신병동, 주차장) 스폰 시
        {
            spawnPoint = GameObject.Find("ThirdSpawnPoint");
            SetSpawnPositionWithScaleUnder2F(spawnPoint);
            IncrementSceneCount();//지하로 왔을 때 씬 카운트를 증가시켜놓는다.
        }
    }

    private GameObject GetSavedSpawnPoint(string sceneName, string firstSpawnPointName, string lastSpawnPointName)
    {
        //SceneCount 값에 따라 다른 스폰 포인트를 결정
        string savedPointName = (SceneCount == 0) ? firstSpawnPointName : lastSpawnPointName;//SceneCount가 0이면(게임이 처음 시작되고 처음 1층으로 스폰 시) firstSpawnPoint로 스폰, 이 경우가 아니면 LastSpawnPoint로 스폰됨.
         Debug.Log($"SceneCount: {SceneCount}, Saved Spawn Point: {savedPointName}");
         return GameObject.Find(savedPointName);
    }

    // 지하 2층, 지하 3층의 맵 크기가 다르기 때문에, 스폰된 플레이어의 스케일을 각각 다르게 해야 한다.
    public void SetSpawnPositionWithScaleUnder2F(GameObject sp)//NewUnderAfter(지하2층)에서 스폰될 때
    {
        if(sp != null)
        {
            player.transform.position = sp.transform.position;
            player.transform.rotation = sp.transform.rotation;
            player.transform.localScale = UnderFloorPlayer;
            Debug.Log($"Spawn Point : {sp.name}");
            SaveSpawnPoint(sp); // 현재 위치 저장
        }
    }
    public void SetSpawnPositionWithScaleUnder3F(GameObject sp)//NewUnder3F(지하3층)에서 스폰될 때
    {
        if(sp != null)
        {
            player.transform.position = sp.transform.position;
            //player.transform.rotation = Under3FPlayerRotation;
            player.transform.rotation = sp.transform.rotation;
            player.transform.localScale = Under3FPlayer;
            Debug.Log($"Spawn Point : {sp.name}");
            SaveSpawnPoint(sp); // 현재 위치 저장
        }
    }
    public void SetSpawnPosition(GameObject sp)//New1_2Floor(지상 병원 층)에서 스폰될 때
    {
        if (sp != null)
        {
            player.transform.position = sp.transform.position;
            player.transform.rotation = sp.transform.rotation;
            Debug.Log($"Spawn Point : {sp.name}");
            SaveSpawnPoint(sp); // 현재 위치 저장
        }
    }

    private void SaveSpawnPoint(GameObject spawnPoint)// 씬 이름과 스폰지점을 PlayerPrefs에 저장한다. 
    {
        // 현재 씬 이름과 스폰 지점 이름을 저장
        string sceneName = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString($"{sceneName}_SpawnPoint", spawnPoint.name);
        PlayerPrefs.SetInt("SceneCount", SceneCount);
        PlayerPrefs.Save();
    }

    public void IncrementSceneCount()//씬카운트 증가 메서드. 
    {
        SceneCount++;
        PlayerPrefs.SetInt("SceneCount", SceneCount);
        PlayerPrefs.Save();
    }

    private void OnApplicationQuit() // 게임 종료 시 씬 카운트 값 초기화. PlayerPrefs값은 게임 종료 후에도 레지스트리에 남아있기에, 초기화 필요. 대신 게임 세션 중에는 값이 유지될 것.
    {
        PlayerPrefs.DeleteKey("SceneCount");
        PlayerPrefs.Save();    
    }
}
