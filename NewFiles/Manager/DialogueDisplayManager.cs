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
   
   public int GetScenecount = 0;

    protected override void OnStart()
    {
        GetScenecount = SceneChangeManager.SceneCount;
        SceneManager.sceneLoaded += OnSceneLoaded;//씬이 로드될 때 호출되는 이벤트 핸들러  
    }

    protected override void OnUpdate()
    {
       
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;//이벤트 핸들러 등록 해제
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)//씬이 전환될 때 마다 적절한 대사가 출력될 수 있도록 하는 함수
    {

        if(scene.name=="New1_2FloorScene")
        {
            DisplayDialogueOnce("00002");
        }
        if (scene.name=="NewUnder3F")//1층 씬
        {
            DisplayDialogueOnce("11114");
        }
        if(scene.name=="NewUnderAfter")
        {
            DisplayDialogueOnce("11115");
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

    private void OnCollisionEnter(Collision collision)//특정 장소, 특정 아이템 등 충돌 시 나타나는 다이얼로그 표시
    {
        
        if(SceneManager.GetActiveScene().name == "New1_2FloorScene")//지상 씬에서 출력될 다이얼로그
        {
            if (collision.gameObject.CompareTag ("SecureRoom"))
            {
               Debug.Log("dialogue !!");
                DisplayDialogueOnce("00003");
            }

            if (collision.gameObject.CompareTag("Shutter") && GetScenecount==0)
            { 
                Debug.Log("dialogue !!");
                DisplayDialogueOnce("10009");                
            }


        }
 
    }

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
    public void ShamanRoarDialogue()
    {
        DisplayDialogueOnce("11116");
    }
    public void ShamanRunDialogue()
    {
        DisplayDialogueOnce("11117");
    }
    public void DoctorRunDialogue()
    {
        DisplayDialogueOnce("11118");
    }
    public void EventPhonCalling()
    {
        DisplayDialogueOnce("11119");
    }


}
