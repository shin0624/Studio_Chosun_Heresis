using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class ShamanController : MonoBehaviour
{
    # region shaman구현로직개요
    //무당 보스캐릭터 컨트롤러 : 무당은 IDLE, ATTACK, RUNNING, CHOKE, ROAR상태가 있음
    //플레이어가 일정 거리에 들어오기 전까지 IDLE
    //플레이어가 탐지되면 ROAR 모션 -> RUNNING 상태로 순차 전환
    //플레이어와의 거리가 공격 가능 범위 내일 때 : ATTACK 또는 CHOKE
    //CHOKE 상태와 ATTACK 상태는 RUNNING에서 전환되며, 낮은 확률로 CHOKE가 선택됨
    //CHOKE 상태로 전환되면 플레이어의 목을 졸라 위로 들어올림. 한번에 sanity를 2씩 줄이는 필살기.
    // 보스캐릭터는 NAVMESH 위에서 움직인다.
    //플레이어와의 거리가 일정 이상으로 멀어지면 NAVMESH 위의 다른 좌표로 이동하여 플레이어 접근을 IDLE상태로 기다린다.
    // 무당 캐릭터는 Behavior Tree 패턴으로 구현하여 CHOKE와 ATTACK 사이 조건부 전환을 단순화 할 것, 또한 보스캐릭터이기에 추후 확장성을 대비할 수 있음 
    // 행동 트리 패턴을 기반으로 Choke와 attack 실행 여부를 확률로 판정, idle->combat selector -> running을 반복
    # endregion
    //---필수 할당 컴포넌트들--
     public NavMeshAgent agent;
     public Animator animator;
     public Transform player;
     [SerializeField] public AudioClip ado;
     [SerializeField] public CameraController cameraController;

    // --- 플레이어 탐지 관련 변수들---
     [Header("Detection Settings")]
     public float detectionRange = 15.0f;
     public float attackRange = 2.5f;
     public float chokeRange = 0.7f;
     public float chokeProbility = 0.2f;//초크, 공격이 랜덤으로 선택되어야 하므로 확률을 선언

     //--- 행동트리 관련 변수들 --
     private BehaviorNode rootNode;//BT의 루트노드 변수 선언
     public bool isChoking = false;// 목조르기 수행 중인가?
     
     void Start()   
     {  
        agent  = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        cameraController = FindAnyObjectByType<CameraController>();
        player = GameObject.FindGameObjectWithTag("PLAYER").transform;
        SetUpBT();
     }

      private void OnDrawGizmos()
      {
         // 탐지 거리
         Gizmos.color = Color.green;
         Gizmos.DrawWireSphere(this.transform.position, detectionRange);

         // 근접 공격 사거리
         Gizmos.color = Color.blue;
         Gizmos.DrawWireSphere(this.transform.position, attackRange);
      }

     private void Update() 
     {
      rootNode.Evaluate();//매 프레임 평가 수행
     }

     public void PlayRoarEffects()
     {
         
         AudioSource.PlayClipAtPoint(ado, transform.position);//Shaman이 있는 장소에서 소리가 울려퍼져야 하기 때문에 PlayClipAtPoint를 사용.
         cameraController.StartShake();// 포효소리가 들리면서 카메라 흔들림 발생
     }

     private void SetUpBT()//행동트리 셋업
     {
      Debug.Log("BT START");
        rootNode = new SelectorNode(); // 루트 노드. 왼쪽->오른쪽으로 진행하며 우선 순위가 높은 자식 노드부터 실행. 자식 노드들 중 성공한 노드가 있다면 그 노드를 실행하고 종료.
        
        //idle 서브트리
        var idleSequence = new SequenceNode();
        idleSequence.AddChild(new CheckPlayerDistanceNode(this, detectionRange, true));//플레이어와의 거리 체크
        idleSequence.AddChild(new IdleActionNode(this));

        //combat 서브트리
        var combatSelector = new SelectorNode();

        //Choke시퀀스
        var chokeSequence = new SequenceNode();
        chokeSequence.AddChild(new CheckChokeConditionNode(this)); //초크할 조건이 되는지 체크
        chokeSequence.AddChild(new ChokeActionNode(this));

        //Attack시퀀스
        var attackSequence = new SequenceNode();
        attackSequence.AddChild(new CheckAttackRangeNode(this));//공격할 조건이 되는지 체크
        attackSequence.AddChild(new AttackActionNode(this));

        //Chase시퀀스
        var chaseSequence = new SequenceNode();
        chaseSequence.AddChild(new CheckPlayerDistanceNode(this, detectionRange,false)); // 추격할 조건이 되는지 체크
        chaseSequence.AddChild(new ChaseActionNode(this));

         //2번째 노드(공격or초크or추격)의 자식으로 각각의 시퀀스를 삽입
        combatSelector.AddChild(chokeSequence);
        combatSelector.AddChild(attackSequence);
        combatSelector.AddChild(chaseSequence);

        //루트노드에 IDLE 서브트리, CombatSelector 서브트리를 자식으로 삽입
        rootNode.AddChild(idleSequence);
        rootNode.AddChild(combatSelector);
     }
}