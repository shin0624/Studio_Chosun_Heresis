using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectiveDisplayManager : ObjectiveController
{
    //게임 상에서 발생하는 모든 목표(Objective)출력을 제어할 스크립트
    //DialogueDisplayManger와 동일한 구조로 작성함.

    private Dictionary<string, bool> ObjectDisplay = new Dictionary<string, bool>();//목표가 중복 출력되는 것을 방지하기 위해 목표번호(숫자 + a)와 출력 여부를 딕셔너리로 관리

    protected override void OnStart()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;//씬이 로드될 때 호출되는 이벤트 핸들러 
    }

    protected override void OnUpdate()
    {
        
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;//이벤트핸들러 등록 해제
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)//씬이 전환될 때 마다 적절한 목표가 출력될 수 있도록 하는 함수
    {


    }

    public void DisplayObjectiveOnce(string newObjectiveID, string objectiveID)//메인 목표가 한번만 출력될 수 있도록 하는 메서드.추상클래스의 DisplayObjective를 사용하여 목표를 출력하고, 그때그때 출력된 목표 TF값을 VALUE로 한다
    {
        if(!ObjectDisplay.ContainsKey(newObjectiveID) || !ObjectDisplay[newObjectiveID])//출력된 목표를 KEY로, 출력여부를 VALUE로 딕셔너리에 저장하기 때문에, 한번 출력된 목표의 재출력을 방지할 수 있다.
        {
            DisplayObjective(newObjectiveID, objectiveID);
            ObjectDisplay[newObjectiveID] = true;
            // 출력 조건을 VALUE가 FALSE일 때로 한정.
        }
    }


    private void OnCollisionEnter(Collision collision)//특정 장소, 특정 아이템 등 충돌 시 나타나는 다이얼로그 표시
    {

        if (SceneManager.GetActiveScene().name == "1_2FloorScene")//지상 씬에서 출력될 다이얼로그
        {
            if (collision.gameObject.CompareTag("1F_Start"))
            {

                FindSecurityRoom();

            }
        }
    }


    void FindSecurityRoom()//단서에 적힌 보안실 비밀번호 알아내기
    {
        DisplayObjectiveOnce("001a", "001b");
    }

}
