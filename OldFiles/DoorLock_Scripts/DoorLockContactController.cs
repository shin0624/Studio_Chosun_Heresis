using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DoorLockContactController : MonoBehaviour
{
    public string DoorLockPrefabPath = "PREFABS_MAKESELF/DoorLock/DoorLockCanvas";//ui 프리팹 경로
    private GameObject DoorLockUIInstance;// ui 프리팹 인스턴스
    

    void Start()
    {
        GameObject DoorLockPrefab = Resources.Load<GameObject>(DoorLockPrefabPath);//게임 시작 시 프리팹 경로에서 프리팹을 로드

        if (DoorLockPrefab==null)//널체크
        {
            Debug.LogError("DoorLockCanvas is NULL.");
            return;
        }
        
        DoorLockUIInstance = Instantiate(DoorLockPrefab);//프리팹 인스턴스 생성 및 비활성화
        DoorLockUIInstance.SetActive(false);//초기에는 비활성화

        DoorLockController.OnDoorLockClosed += DeactivateDoorLockUI;//DoorLockController의 이벤트를 구독--> ESC키가 눌렸을 때 UI를 비활성화 처리
    }
    private void OnDestroy()
    {
        DoorLockController.OnDoorLockClosed -= DeactivateDoorLockUI;//이벤트 구독 해제
    }

    private void OnTriggerStay(Collider other)//충돌상태 + E키가 눌리면 ui가 활성화되어야 하므로, 지속직인 키 입력 감지가 필요.
                                              //OnCollisionEnter는 충돌 순간만 호출되기 떄문에 부적절
    {
        if(other.gameObject.CompareTag("PLAYER"))//플레이어가 가까이 온 후
        {
            if (Input.GetKeyDown(KeyCode.E)) // 상호작용 버튼을 누르면 활성화
            {
                if(DoorLockUIInstance!=null)//ui인스턴스가 존재하는 경우
                {
                    ActivateDoorLockUI();
                }
                else//ui인스턴스가 없는 경우. 혹은 이미 비밀번호를 해제한 후 다시 오브젝트에게 다가와서 e키를 눌렀을 때의 오류를 방지.
                {
                    Debug.Log("You Have Already Unlocked Password.");
                }
                
            }

            
        }
    }

    private void ActivateDoorLockUI()// UI 활성화 메서드
    {
        //Time.timeScale = 0f;//게임시간 일시 정지
        DoorLockUIInstance.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        

    }
    private void DeactivateDoorLockUI()
    {
        //Time.timeScale = 1f;//게임 재개
        DoorLockUIInstance.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
