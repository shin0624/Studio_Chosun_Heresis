using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShamancontrollerTemp2 : MonoBehaviour
{
    [SerializeField] private float patrolSpeed = 2.0f; // 탐색 상태 속도
    [SerializeField] private float chaseSpeed = 3.5f; // 추격 상태 속도
    [SerializeField] private Animator anim; // 애니메이터
    [SerializeField] private const float chaseRange = 9.0f; // 추격 가능 범위
    [SerializeField] private const float detectionRange = 11.0f; // 탐지 가능 범위
    [SerializeField] private const float attackRange = 2.5f; // 공격 가능 범위
    [SerializeField] private float rotationSpeed = 5.0f; // 회전 속도
    [SerializeField] private Transform target; // 플레이어 타겟
    [SerializeField] private NavMeshAgent agent; // 네비메쉬 에이전트
    [SerializeField] private CameraController cameraController;
    [SerializeField] private AudioSource ado;
    private Define.ShamanState currentState; // 현재 상태
    private float distanceToPlayer; // 플레이어와의 거리

    
    private bool hasRoared = false;// 포효 상태 추적을 위한 플래그
    private float roarTimer = 0.0f;
    private float roarDuration = 2.8f;//ROAR 클립 재생시간

    // 애니메이터 파라미터 해시값
    private readonly int isRunningHash = Animator.StringToHash("IsRunning");
    private readonly int attackTriggerHash = Animator.StringToHash("ATTACK");
    private readonly int chokeTriggerHash = Animator.StringToHash("StartChoke");
    private readonly int roarTriggerHash = Animator.StringToHash("ROAR");

    private void OnDrawGizmos() // 상호작용 가능 거리를 기즈모로 표현
      {
         // 탐지 거리
         Gizmos.color = Color.green;
         Gizmos.DrawWireSphere(this.transform.position, detectionRange);

         // 근접 공격 사거리
         Gizmos.color = Color.blue;
         Gizmos.DrawWireSphere(this.transform.position, attackRange);
      }

    void Start()
    {
        ado = GetComponent<AudioSource>();
        target = GameObject.FindWithTag("PLAYER").transform;
        cameraController = FindAnyObjectByType<CameraController>(); // ROAR 시 카메라 쉐이크를 위해 카메라컴포넌트 할당
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        currentState = Define.ShamanState.IDLE; // 첫 상태는 IDLE
        SetState(currentState);
        agent.stoppingDistance = attackRange;
        agent.speed = patrolSpeed;
        agent.updateRotation = false;

        StartCoroutine(FSM());
    }

    void Update()
    {
        if (target != null && currentState != Define.ShamanState.ATTACK) // ATTACK일 때 : 목적지를 플레이어로 설정
        {
            agent.SetDestination(target.position);
        }

        if (agent.velocity.sqrMagnitude > 0.1f) // 이동 시작 시
        {
            Vector3 direction = agent.velocity.normalized;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction); // 에너미가 플레이어를 바라보도록 한다.
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed); // 부드럽게 회전하며 플레이어를 바라볼 수 있도록
            }
        }
        else if (target != null && currentState != Define.ShamanState.IDLE) // IDLE일 경우
        {
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            if (directionToTarget != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(new Vector3(directionToTarget.x, 0, directionToTarget.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
        }
    }

    private void SetState(Define.ShamanState newState) // 에너미 상태 변화 호출 메서드
    {
        if (currentState != newState)
        {
            currentState = newState;
            Debug.Log($"Current State: {newState}");
        }
    }

    private IEnumerator FSM() // 유한상태기계 코루틴을 SWITCH로 작성
    {
        while (true)
        {
            switch (currentState)
            {
                case Define.ShamanState.IDLE:
                    yield return StartCoroutine(IDLE());
                    break;
                case Define.ShamanState.RUNNING:
                    yield return StartCoroutine(RUNNING());
                    break;
                case Define.ShamanState.ATTACK:
                    yield return StartCoroutine(ATTACK());
                    break;
                case Define.ShamanState.CHOKE:
                    yield return StartCoroutine(CHOKE());
                    break;
                case Define.ShamanState.ROAR:
                    yield return StartCoroutine(ROAR());
                    break;
            }
            yield return null;
        }
    }

    private IEnumerator IDLE() // IDLE상태. 플레이어가 사정거리 내에 들어오면 ROAR 애니메이션으로 전환된다.
    {
        anim.SetBool(isRunningHash, false);
        yield return new WaitForSeconds(1.0f);
        if (target != null)
        {
            distanceToPlayer = Vector3.Distance(transform.position, target.position);
            if (distanceToPlayer <= detectionRange)
            {
                SetState(Define.ShamanState.ROAR);
            }
        }
    }

    private IEnumerator RUNNING() // RUNNING 상태. ROAR가 끝나면 플레이어를 추격
    {
        anim.SetBool(isRunningHash, true);
        agent.speed = chaseSpeed;
        hasRoared = false;

        if (target != null)
        {
            distanceToPlayer = Vector3.Distance(transform.position, target.position);

            if (distanceToPlayer <= attackRange)
            {
                SetState(Random.Range(0f, 1f) < 0.3f ? Define.ShamanState.CHOKE : Define.ShamanState.ATTACK);// 랜덤값을 매겨서 CHOKE를 실행할 지, ATTACK을 실행할 지 결정하여 전환
            }
            else if (distanceToPlayer > chaseRange)
            {
                SetState(Define.ShamanState.IDLE);// 멀어지면 다시 IDLE
            }
        }
        yield return new WaitForSeconds(0.1f);
    }

    private IEnumerator ATTACK()// ATTACK 상태. 공격 중에는 이동을 정지하고 때린다.
    {
        anim.SetTrigger(attackTriggerHash);
        agent.speed = 0; // 공격 중 정지
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);

        SetState(Define.ShamanState.RUNNING);// 공격 후 RUNNING으로 전환하여 쫒아감
    }

    private IEnumerator CHOKE()// CHOKE 상태. 일정 확률로 CHOKE가 되면 플레이어를 들어올려 목을 조르고 다시 팽개친다. 정신력-2 감소
    {
        anim.SetTrigger(chokeTriggerHash);
        agent.speed = 0;
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);

        SetState(Define.ShamanState.RUNNING);
    }

    private IEnumerator ROAR()// 포효 상태. 플레이어를 발견하면 포효 후 쫒아간다.
    {
        anim.SetTrigger(roarTriggerHash); // 애니메이션 트리거
    PlayRoarEffects(); // 이펙트 실행. 포효소리가 들리면 카메라 쉐이크가 발동하여 좀 더 실감나는 느낌을 더함

    if (!hasRoared)
    {
        hasRoared = true; // 첫 포효를 체크
        roarTimer = 0.0f; // 타이머 초기화
    }

    while (roarTimer < roarDuration) // 포효 지속 시간 동안 대기
    {
        roarTimer += Time.deltaTime; // 시간 증가
        yield return null; // 다음 프레임까지 대기
    }

    // 포효 애니메이션이 완료되었는지 확인
    while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
    {
        yield return null; // 애니메이션이 끝날 때까지 대기
    }

    // 상태를 RUNNING으로 변경
    SetState(Define.ShamanState.RUNNING);
        
    }

    public void PlayRoarEffects()
     {
         
         ado.Play();
         cameraController.StartShake();// 포효소리가 들리면서 카메라 흔들림 발생
     }
}
