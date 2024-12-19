using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// 지하 --> 지상 씬 이동 스크립트
public class TeleportSceneManager : MonoBehaviour
{
    public string targetSceneName = "1_2FloorScene"; //지상 씬 이름

    private bool playerInRange = false;//플레이어가 범위 내에 있는지 여부

    private float holdTime = 0f;// E 버튼을 누르고 있는 시간

    public float requireHoldTime = 3f;//버튼을 눌러야 하는 최소 시간

    //플레이어가 문 근처에 오면 상호작용 버튼을 3초간 클릭하라는 문구와 함께 눌린 시간이 게이지로 표시됨.
    public Text holdMessage; //지시 메시지

    //키 이벤트 추가
    public string requiredItemName = "Under_Key";//필요한 열쇠 아이템 이름
    private Inventory playerInventory;//플레이어 인벤토리를 불러온다.
    private DialogueDisplayManager dialogueDisplayManager;

    //public Vector3 newSpawnPosition = new Vector3(-16.6749992f, -2.4059999f, 23f);//새로운 스폰 위치

    void Start()
    {
        //시작 시 ui요소 비활성화.
        holdMessage.gameObject.SetActive(false);
      

        dialogueDisplayManager = FindObjectOfType<DialogueDisplayManager>();
    }



    void Update()
    {
        try
        {
            if(playerInRange)
            {
                holdMessage.gameObject.SetActive(true);
               

                if(Input.GetKey(KeyCode.E))
                {
                    holdTime += Time.deltaTime; //플레이어가 범위 내에 있을 때 상호작용 버튼이 눌리면 holdtime이 증가함.
                   

                    if(holdTime >= requireHoldTime)//holdtime이 requireHoldTime을 초과하면 지상 씬으로 이동.
                    {
                        //키를 가지고 있다면 문을 열고 지상 씬으로 이동할 수 있다.
                        if(playerInventory!=null && playerInventory.HasItem(requiredItemName))
                        {
                            dialogueDisplayManager.PlayerUsingKey();//키를 가지고 있다는 다이얼로그 출력.
                            SceneManager.LoadScene(targetSceneName);
                           
                           
                        }
                        else
                        {
                            dialogueDisplayManager.PlayerNotPickUpKey();//키를 가지고 있지 않다는 다이얼로그 출력.
                        }
                    }
                }
                else
                {
                    holdTime = 0f;//상호작용 버튼이 눌리지 않으면 holdtime을 0으로 초기화.
                    
                }
            }
            else
            {
                holdMessage.gameObject.SetActive(false);
            
            }
        }
        catch(SystemException e)
        {
            Debug.LogError($"Error In LastDoor : {e.Message}");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("PLAYER"))
        {
            playerInRange = true;//플레이어가 문 근처에 접근하면 playerInRange가 true로 설정됨.
            playerInventory = other.GetComponent<Inventory>();//플레이어가 접근하면 인벤토리를 받아온다. 여기서 키 유무 체크가 이루어질 것.
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("PLAYER"))
        {
            playerInRange= false;//플레이어가 문을 벗어나면 playerInRange가 false가 되고 holdtime이 초기화됨.
            holdTime = 0f;

        }
    }

}
