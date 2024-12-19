using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShamancontrollerTemp2 : MonoBehaviour
{
    [SerializeField] private float patrolSpeed = 2.0f; // 탐색 상태 속도
    [SerializeField] private float chaseSpeed = 3.5f; // 추격 상태 속도
    [SerializeField] private Animator anim; // 애니메이터
    [SerializeField] private const float chaseRange = 7.0f; // 추격 가능 범위
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

    private void OnDrawGizmos()
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
        cameraController = FindAnyObjectByType<CameraController>();
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        currentState = Define.ShamanState.IDLE;
        SetState(currentState);
        agent.stoppingDistance = attackRange;
        agent.speed = patrolSpeed;
        agent.updateRotation = false;

        StartCoroutine(FSM());
    }

    void Update()
    {
        if (target != null && currentState != Define.ShamanState.ATTACK)
        {
            agent.SetDestination(target.position);
        }

        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            Vector3 direction = agent.velocity.normalized;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
        }
        else if (target != null && currentState != Define.ShamanState.IDLE)
        {
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            if (directionToTarget != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(new Vector3(directionToTarget.x, 0, directionToTarget.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
        }
    }

    private void SetState(Define.ShamanState newState)
    {
        if (currentState != newState)
        {
            currentState = newState;
            Debug.Log($"Current State: {newState}");
        }
    }

    private IEnumerator FSM()
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

    private IEnumerator IDLE()
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

    private IEnumerator RUNNING()
    {
        anim.SetBool(isRunningHash, true);
        agent.speed = chaseSpeed;
        hasRoared = false;

        if (target != null)
        {
            distanceToPlayer = Vector3.Distance(transform.position, target.position);

            if (distanceToPlayer <= attackRange)
            {
                SetState(Random.Range(0f, 1f) < 0.3f ? Define.ShamanState.CHOKE : Define.ShamanState.ATTACK);
            }
            else if (distanceToPlayer > chaseRange)
            {
                SetState(Define.ShamanState.IDLE);
            }
        }
        yield return new WaitForSeconds(0.1f);
    }

    private IEnumerator ATTACK()
    {
        anim.SetTrigger(attackTriggerHash);
        agent.speed = 0; // 공격 중 정지
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);

        SetState(Define.ShamanState.RUNNING);
    }

    private IEnumerator CHOKE()
    {
        anim.SetTrigger(chokeTriggerHash);
        agent.speed = 0;
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);

        SetState(Define.ShamanState.RUNNING);
    }

    private IEnumerator ROAR()
    {
        anim.SetTrigger(roarTriggerHash); // 애니메이션 트리거
    PlayRoarEffects(); // 이펙트 실행

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
