using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DoctorController : MonoBehaviour
{
    [SerializeField] private float patrolSpeed = 2.0f; // 탐색상태 이동 속도
    [SerializeField] private float chaseSpeed = 4.0f;// 추격 상태 시 속도
    [SerializeField] private Animator anim; // 닥터 에너미 애니메이터
    [SerializeField] private const float chaseRange = 5.0f; // 추격 가능 범위--> 이 거리를 벗어나면 추격 상태 해제
    [SerializeField] private const float detectionRange = 3.0f; // 탐지 가능 범위--> 이 거리 내에 플레이어가 들어오면 추격 상태로 변경
    [SerializeField] private const float attackRange = 1.0f;//공격 가능 범위
    [SerializeField] private Transform target;
    [SerializeField] private NavMeshAgent agent;
    public JailDoorEventController Flag;//플레이어가 이벤트  트리거를 통과했는지 여부
    private Define.DoctorState currentState;//닥터 상태 변수
    private float distanceToPlayer;// 플레이어와 닥터 간 거리
    private bool isCoroutineRunning = false; //상태 전이를 위한 코루틴 실행 여부 플래그


     void Start() {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        currentState = Define.DoctorState.IDLE;
        agent.stoppingDistance = attackRange;// 네비메시 에이전트 객체가 갖는 정지거리를 변수로 저장
        agent.speed = patrolSpeed;//네비메시 에이전트 객체가 갖는 속도를 변수로 저장
    }

    void Update() 
    {
        if(target!=null && currentState !=Define.DoctorState.ATTACK)// target에 플레이어가 할당되었고 attack상태가 아닐 경우 플레이어의 위치를 목적지로 설정
        {
            agent.SetDestination(target.position);
        }    
    }
    void AwakeDoctor()// 플레이어가 이벤트 트리거에 접근 시 닥터 행동 시작
    {
         if(Flag.isEnter && target==null)//플레이어가 이벤트 트리거에 접근 + 현재 target이 null이면 플레이어를 찾아 target에 할당
         {
            target = GameObject.FindWithTag("PLAYER").transform;
            SetState(Define.DoctorState.WALKING);//플레이어쪽으로 걸어온다. 추후 닥터 전용 컷신 또는 다이얼로그를 추가할 것.
         }
    }


    private void SetState(Define.DoctorState newState)// 닥터의 상태 변화 스위칭을 위한 함수
    {
        if(currentState!=newState)
        {
            currentState = newState;//닥터 상태 중복 호출이 아닐 경우 닥터 상태 스위칭
        }
    }
    private void PlayAnimation(string animationName)// 닥터의 상태 별 애니메이션 재생을 위한 함수
    {
        if(!anim.GetCurrentAnimatorStateInfo(0).IsName(animationName))// 현재 애니메이터에서 진행되는 애니메이션 상태를 체크 -> animationName이라는 이름의 애니메이션이 재생되고 있지 않다면 조건문 실행
        {
            anim.Play(animationName, 0, 0);// 인자 중 normalizedTime = 0 --> 애니메이션이 아직 시작되지 않았다는 뜻.
        }
    }

    private IEnumerator FSM()//닥터 상태를 관리할 유한 상태 기계 코루틴
    {
        while(true)
        {
            switch(currentState)//현재 상태 변수에 따라 반복하며 닥터 상태를 제어한다.
            {
                case Define.DoctorState.IDLE:
                yield return StartCoroutine(IDLE());break;
                
                case Define.DoctorState.WALKING:
                yield return StartCoroutine(WALKING());break;
                
                case Define.DoctorState.RUNNING:
                yield return StartCoroutine(RUNNING());break;
                
                case Define.DoctorState.ATTACK:
                yield return StartCoroutine(ATTACK());break; 
                 
            }yield return null;
        }
    }
    private IEnumerator IDLE()
    {
        PlayAnimation("IDLE");
        yield return new WaitForSeconds(1.0f);
        if(Flag.isEnter && target!=null)
        {
            SetState(Define.DoctorState.WALKING);//플레이어가 트리거에 접근 시 walking상태로 전환
        }
    }
    private IEnumerator WALKING()
    {
        PlayAnimation("WALKING");
        agent.speed = patrolSpeed;
        if(target!=null)
        {
            distanceToPlayer = Vector3.Distance(transform.position, target.position);// 플레이어와 닥터 간 거리 계산
            if(distanceToPlayer <=detectionRange)
            {
                SetState(Define.DoctorState.RUNNING);// 둘 사이 거리가 탐지 거리 내로 들어오면 추격 상태로 전환
            }
        }yield return new WaitForSeconds(0.1f);// 0.1초 전환 대기
    }
    private IEnumerator RUNNING()
    {
        PlayAnimation("RUNNING");
        agent.speed  =  chaseSpeed;
        if(target!=null)
        {
            distanceToPlayer = Vector3.Distance(transform.position, target.position);
            if(distanceToPlayer <= attackRange)
            {
                SetState(Define.DoctorState.ATTACK);// 둘 사이 거리가 공격 가능 범위 이내라면 공격 상태로 전환
            }
            else if(distanceToPlayer > chaseRange)
            {
                SetState(Define.DoctorState.WALKING);//둘 사이 거리가 추격 가능 범위를 넘어섰다면 walking 상태로 전환
            }
        }yield return new WaitForSeconds(0.1f);// 0.1초 전환 대기
    }
    private IEnumerator ATTACK()
    {
        PlayAnimation("ATTACK");
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);//공격 애니메이션 완료 대기
        if(target!=null)
        {
            distanceToPlayer = Vector3.Distance(transform.position, target.position);
            if(distanceToPlayer > attackRange)
            {
                SetState(Define.DoctorState.RUNNING);// 공격 가능 범위를 벗어나다면 추격 상태로 전환
            }
        }
    }
}
