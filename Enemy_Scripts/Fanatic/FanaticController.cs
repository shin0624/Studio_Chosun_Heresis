using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Android;

public class FanaticController : MonoBehaviour
{
    //광신도 캐릭터 컨트롤러

    [SerializeField]
    private Transform Player; // 플레이어의 트랜스폼
    [SerializeField]
    private NavMeshAgent agent;// Bake된 NavMesh에서 활동할 에너미
    [SerializeField]
    private Animator anim;
    [SerializeField]
    private const float ChaseRange = 12.0f;// 플레이어 추격가능 범위
    [SerializeField]
    private const float DetectionRange = 8.0f;//플레이어 탐지 거리
    [SerializeField]
    private const float AttackRange = 1.0f;// 공격 가능 범위

    private Define.EnemyState state;//에너미 스테이트 변수 선언
    private float DistanceToPlayer;//플레이어와의 거리를 저장할 변수 선언

    private Stack<Vector3> dfsStack = new Stack<Vector3>(); // 깊이우선탐색 스택
    private HashSet<Vector3> visitedPositions = new HashSet<Vector3>();// 방문한 위치를 저장할 해쉬셋

    void Start()
    {
        state = Define.EnemyState.IDLE; // 초기상태 : idle
        agent = GetComponent<NavMeshAgent>();
        agent.isStopped= true;
        BeginDFS();//처음에 DFS 탐색 시작
    }
    
    void Update()
    {
        DistanceToPlayer = Vector3.Distance(transform.position, Player.position);// 거리 계산은 각 메서드가 아닌 Update문에서 시행. 즉각적으로 에너미 상태가 스위칭되어야 하므로, 매 프레임 계산하는 것이 간편할 수 있음.

        switch(state)
        {
            case Define.EnemyState.IDLE:
            case Define.EnemyState.WALKING:
                UpdateDFS();//탐색을 통해 맵을 돌아다닌다.
                break;
            case Define.EnemyState.RUNNING:
                UpdateChase();//플레이어를 추격
                break;
            case Define.EnemyState.ATTACK:
                UpdateAttack();//플레이어를 공격 
                break;
        }
    }

    private void BeginDFS()// dfs탐색을 시작하는 메서드
    {
        dfsStack.Clear();// 스택 초기화
        visitedPositions.Clear();//방문장소 해쉬셋 초기화
        dfsStack.Push(transform.position);// 에너미의 현재 위치에서 탐색 시작

        SetState(Define.EnemyState.WALKING, "WALKING");//걸어다니며 탐색 시작
    }

    private void UpdateDFS()
    {
        if(agent.isOnNavMesh && dfsStack.Count>0)//에너미가 bake된 navmesh에 있고, 탐색이 시작되었을 때
        {
            agent.isStopped = false;
            agent.speed = 0.2f;
            if(DistanceToPlayer <=DetectionRange)// 설정해놓은 탐색 반경 내에 플레이어가 있다면 --> 추격(running)
            {
                SetState(Define.EnemyState.RUNNING, "RUNNING");
                return;
            }
            
            //탐색 진행
            if(!agent.hasPath || agent.remainingDistance < 3.0f)// 초기 경로를 갖고있지 않거나 목표지점에 도달한 경우
            {
                Vector3 CurrentPos = dfsStack.Pop();//스택에서 위치를 꺼내서 이동
                visitedPositions.Add(CurrentPos);//방문한 위치로 기록.

                //상하좌우 4방향으로 탐색을 진행.
                Vector3[] directions = {Vector3.forward, Vector3.back, Vector3.left, Vector3.right};
                foreach (Vector3 direction in directions)
                {   
                    Vector3 NewPos = CurrentPos + direction * 1.1f ;// 1.5m 간격으로 새로운 위치를 계산
                    //SamplePosition((Vector3 sourcePosition, out NavmeshHit hit, float maxDistance, int areaMask)
                    // 샘플포지션 메서드 : areaMask에 해당하는 NavMesh 중에서, maxDistance 반경 내에서 sourcePosition에 최근접한 위치를 찾아 hit에 담는다.
                    if (NavMesh.SamplePosition(NewPos, out NavMeshHit hit, 3.0f, NavMesh.AllAreas ) && !visitedPositions.Contains(hit.position)) //1.5m 이내에서, 위의 새로이 계산한 위치와 최근접한 위치를 찾아 hit에 담는다.
                    {
                        dfsStack.Push(hit.position);//유효한 위치이면 스택에 추가   
                    }
                }
                agent.destination= CurrentPos;//이동할 위치 설정
                Debug.Log($"Destination : {CurrentPos}  / agent Dest : {agent.destination}");

                if(agent.destination == CurrentPos)
                {
                    Debug.Log("Same destination is set repeatedly. Confirmation required");
                }  
            }
        }
    }

    private void UpdateChase()// 추격상태 메서드
    {
        if(agent.isOnNavMesh)
        {
            agent.isStopped = false;
            agent.speed = 0.7f;
            agent.destination = Player.position;

            if (DistanceToPlayer > ChaseRange)//플레이어가 추적 범위 밖으로 나가면
            {
                BeginDFS();//다시 탐색 시작
            }
            else if(DistanceToPlayer <AttackRange)//플레이어와의 거리가 AttackRange 이하로 줄어들면 공격상태로 변화
            {
                UpdateAttack();
                return;
            }
        }
    }

    private void SetState(Define.EnemyState NewState, string AnimationTrigger)
    {
        if(state!=NewState)//불필요한 상태 변경(running상태에서 또 running을 설정하는 경우 등) 최소화
        {
            state = NewState;
            anim.SetTrigger(AnimationTrigger);//애니메이션 상태 전환
        }
    }

    private void UpdateAttack()
    {
        if(agent.isOnNavMesh)
        {
            agent.isStopped = true;// 플레이어에 가까이 다가가면 멈춰서 공격
            SetState(Define.EnemyState.ATTACK, "ATTACK");

            if (DistanceToPlayer > AttackRange)//플레이어가 공격 가능 범위 밖으로 나가면
            {
                UpdateChase();// 다시 쫒기 시작 -> 다시 공격 범위로 들어오면 공격하고, 그대로 범위를 벗어나면 탐색으로 전환.
                return;
            }
        }
    }
}
