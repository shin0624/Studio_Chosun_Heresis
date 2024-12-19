using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DoctorController : MonoBehaviour
{
    [SerializeField] private float patrolSpeed = 2.0f; // 탐색상태 이동 속도
    [SerializeField] private float chaseSpeed = 3.6f;// 추격 상태 시 속도
    [SerializeField] private Animator anim; // 닥터 에너미 애니메이터
    [SerializeField] private const float chaseRange = 8.0f; // 추격 가능 범위--> 이 거리를 벗어나면 추격 상태 해제
    [SerializeField] private const float detectionRange = 5.0f; // 탐지 가능 범위--> 이 거리 내에 플레이어가 들어오면 추격 상태로 변경
    [SerializeField] private const float attackRange = 2.0f;//공격 가능 범위
    [SerializeField] private float rotationSpeed = 5.0f;// 에너미 회전 속도
    [SerializeField] private Transform target;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private AudioSource ado;
    public bool Flag;//플레이어가 이벤트  트리거를 통과했는지 여부
    private Define.DoctorState currentState;//닥터 상태 변수
    private float distanceToPlayer;// 플레이어와 닥터 간 거리
    private bool isCoroutineRunning = false; //상태 전이를 위한 코루틴 실행 여부 플래그

    

    //애니메이터 파라미터 해시값을 해싱하여, 애니메이터 상태 전환을 메서드 하나에서 모두 관리하도록 통합
    private readonly int speedHash = Animator.StringToHash("Speed");
    private readonly int isWalkingHash = Animator.StringToHash("IsWalking");
    private readonly int isRunningHash = Animator.StringToHash("IsRunning");
    private readonly int attackTriggerHash = Animator.StringToHash("Attack");

     void Start() {
        ado = GetComponent<AudioSource>();
        target = null;
       //Flag = TempTrigger.isEnter;
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        currentState = Define.DoctorState.IDLE;
        agent.stoppingDistance = attackRange;// 네비메시 에이전트 객체가 갖는 정지거리를 변수로 저장
        agent.speed = patrolSpeed;//네비메시 에이전트 객체가 갖는 속도를 변수로 저장
        agent.updateRotation = false; // 네비메시 에이전트의 자동 회전을 비활성화

        StartCoroutine(FSM());
    }

    void Update() // 닥터가 행동 중 플레이어 방향을 바라보고 행동할 수 있도록 회전 로직을 추가
    {
        if(target!=null && currentState !=Define.DoctorState.ATTACK)// target에 플레이어가 할당되었고 attack상태가 아닐 경우 플레이어의 위치를 목적지로 설정
        {
            agent.SetDestination(target.position);
        }
        if(agent.velocity.sqrMagnitude > 0.1f)// 속도 제곱의 크기 임계값의 정확도 향상을 위해 sqrMagnitude로 계산 후 이동 방향으로 부드럽게 회전시킴
        {
            Vector3 direction = agent.velocity.normalized;//네비메시의 이동 방향을 계산
            if(direction!=Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);//목표 회전값 계산
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);//slerp를 사용하여 부드러운 회전을 적용
            }
        }
        else if(currentState!=Define.DoctorState.IDLE)// 정지 상태에서는 플레이어를 바라보도록 한다.
        {
            Vector3 directionToTarget = (target.position - transform.position).normalized;
             if (directionToTarget != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(new Vector3(directionToTarget.x, 0, directionToTarget.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
        }    
        Debug.Log(TempTrigger.isEnter);
    }
    public void AwakeDoctor()// 플레이어가 이벤트 트리거에 접근 시 닥터 행동 시작
    {
         if(TempTrigger.isEnter==true&& target==null)//플레이어가 이벤트 트리거에 접근 + 현재 target이 null이면 플레이어를 찾아 target에 할당
         {
            Debug.Log("Player Enter");
            Debug.Log(Flag);
            target = GameObject.FindWithTag("PLAYER").transform;
            SetState(Define.DoctorState.WALKING);//플레이어쪽으로 걸어온다. 추후 닥터 전용 컷신 또는 다이얼로그를 추가할 것.
            ado.Play();
         }
    }


    private void SetState(Define.DoctorState newState)// 닥터의 상태 변화 스위칭을 위한 함수
    {
        if(currentState!=newState)
        {
            currentState = newState;//닥터 상태 중복 호출이 아닐 경우 닥터 상태 스위칭
            Debug.Log($"currentState : {newState}");
            Debug.Log($"distance : {distanceToPlayer}");
        }
    }

    private void UpdateAnimatorParams(float speed, bool isWalking, bool isRunning)//애니메이터 제어를 통합 관리할 메서드. 각 상태 별 명확한 파라미터 값 설정이 가능.
    {
        anim.SetFloat(speedHash, speed);
        anim.SetBool(isWalkingHash, isWalking);
        anim.SetBool(isRunningHash, isRunning);
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
        UpdateAnimatorParams(0.0f, false, false);
        yield return new WaitForSeconds(1.0f);
        if(TempTrigger.isEnter==true && target!=null)
        {
            SetState(Define.DoctorState.WALKING);//플레이어가 트리거에 접근 시 walking상태로 전환
        }
    }
    private IEnumerator WALKING()
    {
        UpdateAnimatorParams(1.0f, true, false);
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
        UpdateAnimatorParams(1.5f, false, true);
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
    private IEnumerator ATTACK()// 공격 상태에서는 즉시 플레이어를 향해 회전하도록 함.
    {
        UpdateAnimatorParams(0f, false, false);
        anim.SetTrigger(attackTriggerHash);

        if(target!=null)
        {   
            // 공격 시 타겟을 향해 즉시 회전
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToTarget.x, 0, directionToTarget.z));
            transform.rotation = lookRotation;
        }
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);//공격 애니메이션 완료 대기

        if (target != null)
        {
            distanceToPlayer = Vector3.Distance(transform.position, target.position);

            if (distanceToPlayer > attackRange)
            {
                SetState(Define.DoctorState.RUNNING);
            }
        }
    }
}