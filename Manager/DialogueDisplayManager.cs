using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

//게임 상에서 발생하는 모든 다이얼로그 출력을 제어할 스크립트. 플레이어 액션, 트리거 이벤트 등을 받아 그때 그때 적절한 대사를 출력한다.
public class DialogueDisplayManager : DialogController
{
    private Dictionary<string, bool> displayDialogues = new Dictionary<string, bool>();//다이얼로그가 중복 출력되는 것을 방지하기위해, 다이얼로그 표시여부와 다이얼로그 번호값을 딕셔너리로 관리한다.
   // public AutoShutterScript autoShutter;//1층의 셔터 업다운 이벤트 다이얼로그 출력용
   // private DFSEnemyAI enemyAI;//지하층의 에너미 이벤트 다이얼로그 출력용


    protected override void OnStart()
    {
        //autoShutter = AutoShutterScript.Instance;//셔터 인스턴스 -> 처음 1층에서 셔터가 올라간 상태-> 지하를 지나 다시 1층으로 와도 셔터가 올라간 상태여야 하므로 상태 저장용.
       // enemyAI = FindObjectOfType<DFSEnemyAI>();//에너미ai 스크립트 참조를 얻는다. 

        SceneManager.sceneLoaded += OnSceneLoaded;//씬이 로드될 때 호출되는 이벤트 핸들러
    }

    protected override void OnUpdate()
    {
       
    }

    //private void Update()
    //{
    //    if(SceneManager.GetActiveScene().name== "UndergroundScene")
    //    {
    //        CanSeeEnemy();
    //    }
    //}

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;//이벤트 핸들러 등록 해제
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)//씬이 전환될 때 마다 적절한 대사가 출력될 수 있도록 하는 함수
    {
        if (scene.name=="1_2FloorScene")//1층 씬
        {
                DisplayDialogueOnce("10013");
        }
        
    }

    public void DisplayDialogueOnce(string dialogueID)//다이얼로그가 한번만 출력될 수 있도록 하는 메서드. 추상클래스의 startdialouge를 사용하여 다이얼로그를 출력하고, 그때그때 출력된 다이얼로그 TF값을 VALUE로 한다. 
    {
        if(!displayDialogues.ContainsKey(dialogueID) || !displayDialogues[dialogueID]) //출력된 다이얼로그를 KEY로, 출력여부를 VALUE로 딕셔너리에 저장하기 때문에, 한번 출력된 다이얼로그의 재출력을 방지할 수 있다.
        {
            //다이얼로그 출력 조건을 VALUE가 FALSE일 때로 한정.
            StartDialogue(dialogueID);
            displayDialogues[dialogueID] = true;
        }
    }


   // private AutoShutterScript GetAutoShutter()
   // {
    //    return autoShutter;
   // }

    private void OnCollisionEnter(Collision collision)//특정 장소, 특정 아이템 등 충돌 시 나타나는 다이얼로그 표시
    {
        
        if(SceneManager.GetActiveScene().name == "1_2FloorScene")//지상 씬에서 출력될 다이얼로그
        {
            if (collision.gameObject.CompareTag("1F_Start"))
            {
               
                DisplayDialogueOnce("00002");
               
            }

            if (collision.gameObject.CompareTag ("SecureRoom"))
            {
               
                DisplayDialogueOnce("00003");
            }

            if (collision.gameObject.CompareTag("Shutter"))
            { 
                DisplayDialogueOnce("10009");

                
            }

           // if (autoShutter!=null && autoShutter.RaiseShutter)
           // {
           //     DisplayDialogueOnce("10010");
           // }

            if (collision.gameObject.CompareTag("map"))
            {

                DisplayDialogueOnce("10012");
            }


        }
        else if(SceneManager.GetActiveScene().name == "UndergroundScene")//지하 씬에서 출력될 다이얼로그
        {
            if (collision.gameObject.CompareTag("coffin"))
            {
                Debug.Log("Spawn on the coffin");
                DisplayDialogueOnce("00005");
            }
            //Inventory inventory = collision.gameObject.GetComponent<Inventory>();
            //if(inventory != null)
            //{
           //     PlayerPickUpKey(inventory);//플레이어가 키를 습득하면 나오는 다이얼로그
            //}
            
            
            
        }
 
    }

    //void CanSeeEnemy()
    //{
    //    if(enemyAI!=null)
    //    {
    //        Define.EnemyState currentState = enemyAI.GetState();//에너미의 상태를 가져온다.
    //        switch(currentState)
    //        {
    //            case Define.EnemyState.WALKING://탐색중인 에너미를 발견했을 때
    //                DisplayDialogueOnce("00006");
    //                break;
    //            case Define.EnemyState.RUNNING:
    //                DisplayDialogueOnce("00007");
    //                break;
    //        }
    //    }
    //}

    //void PlayerPickUpKey(Inventory playerInventory)
    //{
    //    if(playerInventory.HasItem("Under_Key"))
    //    {
    //        DisplayDialogueOnce("10008");
    //    }
    //}

    public void PlayerUsingKey()
    {
        DisplayDialogueOnce("00009");
    }

    public void PlayerNotPickUpKey()
    {
        DisplayDialogueOnce("00008");
    }
    public void SecondSpawn()
    {
        DisplayDialogueOnce("10011");
    }

    public void IntroSceneDialogue01()//인트로씬용 다이얼로그(Intro#1)_First
    {
        DisplayDialogueOnce("11111");
    }
    public void IntroSceneDialogue02()//인트로씬용 다이얼로그(Intro#1)_Second
    {
        DisplayDialogueOnce("11112");

    }
    public void MeetWithDoctorDialogue()//2층 닥터 조우 이벤트용 다이얼로그
    {
        DisplayDialogueOnce("11113");
    }

    //void PlayerPickUpHammer(Inventory playerInventory)
    //{
    //    if (playerInventory.HasItem("Hammer"))
    //    {
    //        DisplayDialogueOnce("10014");
    //    }
    //}
}
