using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FanaticController : MonoBehaviour
{
    //광신도 캐릭터 컨트롤러
//--------------------------에너미 행동 관련 변수-----------------------------------------
    [SerializeField] private Transform Player;//플레이어의 트랜스폼
    [SerializeField] private NavMeshAgent Agent; // Bake된 NavMesh에서 활동할 에너미
    [SerializeField] private Animator Anim;
    [SerializeField] private const float ChaseRange = 3.0f;//플레이어 추격 가능 범위
    [SerializeField] private const float DetectionRange = 3.0f;// 플레이어 탐지 거리
    [SerializeField] private const float AttackRange = 0.7f;// 공격 가능 범위
    private Define.EnemyState state;//에너미 상태 변수
    private float DistanceToPlayer;//플레이어와의 거리를 저장할 변수

//--------------------------에너미 경로 계산 관련 변수------------------------------------
    private List<Vector3> Path = new List<Vector3>();// A*알고리즘으로 계산된 경로를저장할 리스트
    private int CurrentPathIndex = 0;// 에너미가 현재 이동중인 경로 지점의 인덱스. 처음에는 Path[0]으로 이동.

    //--------------------------공격 및 정신력 감소 관련 변수------------------------------
    public SanityManager sanityManager; // SanityManager를 참조
    private bool canAttack = true; // 공격 가능 여부를 체크할 변수
    private float attackCooldown = 3.0f;//공격 쿨타임
    //------------------------------------------------------------------------------------
    private void Start()
    {
        state = Define.EnemyState.IDLE;//초기상태 : IDLE
        Agent = GetComponent<NavMeshAgent>();
        Agent.isStopped = true;
        Anim = GetComponent<Animator>();
        sanityManager = GameObject.FindAnyObjectByType<SanityManager>(); // 추가: SanityManager 찾기
        BeginPatrol();//처음에 탐지 시작
    }

    private void Update()
    {
        DistanceToPlayer = Vector3.Distance(transform.position, Player.position);//플레이어와 에너미 사이의 거리를 계산
        switch (state)
        { 
            case Define.EnemyState.IDLE:
                break;
            case Define.EnemyState.WALKING:
                Patrol();//경로에 따라 탐색을 계속 진행
                break;
            case Define.EnemyState.RUNNING:
                UpdateChase();// 플레이어를 추격
                break;
            case Define.EnemyState.ATTACK:
                UpdateAttack();//플레이어를 공격
                break;
        }    
    }

    private void Patrol()// 탐색상태
    {
        if(Agent.isOnNavMesh && Path.Count>0)
        {
            Agent.isStopped = false;
            Agent.speed = 0.2f;

            if(DistanceToPlayer <=DetectionRange && state!=Define.EnemyState.ATTACK)//탐지 범위 내에 플레이어가 존재하면 && 공격 상태가 아닐 때 추격을 시작한다.
            {
                SetState(Define.EnemyState.RUNNING, "RUNNING");
                return;
            }

            if(!Agent.hasPath || Agent.remainingDistance < 1.0f)//현재 경로가 없거나, 목표 지점에 도달하면
            {
                if(CurrentPathIndex < Path.Count)//경로 상의 다음 지점으로 이동
                {
                    Agent.SetDestination(Path[CurrentPathIndex]);
                    CurrentPathIndex++;//현재 이동중인 경로의 다음 경로로 이동할 것.
                }
                else// 경로 끝에 도달하면 새 경로 계산
                {
                    CalculateNewPath();
                }
            }
        }
    }

    private void BeginPatrol()
    {
        SetState(Define.EnemyState.WALKING, "WALKING"); // 걸어다니며 탐색 시작
        Agent.isStopped = false;
        CalculateNewPath();// 새로운 경로를 계산
        Debug.Log($"현재 플레이어와의 거리 : {DistanceToPlayer}" );
        Debug.Log($"현재 상태 : {state}");
    }

    private void UpdateAttack()// 공격 후 -> 플레이어와의 거리가 공격 가능 범위를 넘어간 상태 && 플레이어와의 거리가 아직 탐지 범위에 포함될 때 다시 쫒아가 플레이어를 공격해야 함.
    {
        if(Agent.isOnNavMesh)
        {      
            if(canAttack)
            {
            Agent.isStopped = true;//공격 시 그 자리에서 멈춤
            SetState(Define.EnemyState.ATTACK, "ATTACK");
            StartCoroutine(AttackCooldown());
            Debug.Log($"현재 플레이어와의 거리 : {DistanceToPlayer}" );
            Debug.Log($"현재 상태 : {state}");
            //sanityManager.DecreaseSanity(); //정신력 감소 호출
            StartCoroutine(DelayDecreaseSanity());
            }
            if(DistanceToPlayer > AttackRange)// 공격범위를 벗어났다면
            {
                UpdateChase();
                return;
            }     
        } 
    }

    private void UpdateChase()
    {
        if(Agent.isOnNavMesh)
        {
                    Debug.Log($"현재 플레이어와의 거리 : {DistanceToPlayer}" );
        Debug.Log($"현재 상태 : {state}");
            SetState(Define.EnemyState.RUNNING, "RUNNING");
            Agent.isStopped = false;
            Agent.speed = 0.7f;
            Agent.destination = Player.position;// 목적지를 플레이어 포지션으로 설정하여 추격
            if(DistanceToPlayer > ChaseRange)//플레이어와의 거리가 추격 가능 범위를 벗어났다면
            {
                BeginPatrol();//탐지 상태로 전환 
            }
            else if(DistanceToPlayer < AttackRange)//공격 가능 범위까지 다가갔다면
            {
                UpdateAttack();
                return;
            }
        }
    }

    private void CalculateNewPath()// 새로운 경로를 계산하는 메서드
    {
        Vector3 RandomDirection = Random.insideUnitSphere * 10.0f;// 반경 1을 갖는 구 안의 임의 지점 * 10으로 경로 설정
        RandomDirection += transform.position;// 에너미 포지션 값에 랜덤 값을 더한다

        NavMeshHit hit;
        //SamplePosition((Vector3 sourcePosition, out NavmeshHit hit, float maxDistance, int areaMask)
        // 샘플포지션 메서드 : areaMask에 해당하는 NavMesh 중에서, maxDistance 반경 내에서 sourcePosition에 최근접한 위치를 찾아 hit에 담는다.
        if (NavMesh.SamplePosition(RandomDirection, out hit, 10.0f, NavMesh.AllAreas))
        {
            Path.Clear();
            Path.Add(hit.position);//A* 알고리즘에 해당하는 경로 설정
            CurrentPathIndex = 0;// 현재위치를 담는 변수 초기화
            Agent.SetDestination(Path[CurrentPathIndex]);
        }
    }

    private void SetState(Define.EnemyState NewState, string AnimationTrigger)// 상태변경 메서드
    {
        if (state != NewState) { state = NewState; Anim.SetTrigger(AnimationTrigger); }//불필요한 상태 변경을 최소화. 각각의 상태에 맞게 애니메이터의 트리거를 바꾸어준다
    }
    
    private IEnumerator AttackCooldown()//공격 쿨타임을 관리하는 코루틴. attackCooldown만큼 대기 후 공격 가능상태로 전환
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    private IEnumerator DelayDecreaseSanity()
    {
        yield return new WaitForSeconds(1.0f);
        sanityManager.DecreaseSanity();
    }
}
