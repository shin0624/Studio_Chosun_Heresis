/*
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.AI;

public class DFS_ENEMY_AI : MonoBehaviour
{
    public Transform target;
    private NavMeshAgent agent;
    private Define.EnemyState state;
    public Animator anim;

    private float WalkingStartTime;//걷기 시작한 시간
    private const float WalkingDurtaion = 3.0f;//걷기 한계 시간

    private Stack<Vector3> dfsStack = new Stack<Vector3>();// DFS 탐색을 위한 스택
    private HashSet<Vector3> visitedPositions = new HashSet<Vector3>();//방문한 위치를 저장하는 해쉬셋 집합. 해쉬셋이므로 중복 허용x


    void Start()//초기 상태는 IDLE
    {
        state = Define.EnemyState.IDLE;
        agent = GetComponent<NavMeshAgent>();
        agent.isStopped= true;
    }

   
    void Update()
    {
        switch (state)
        {
            case Define.EnemyState.IDLE:
                UpdateIdle();
                break;
            case Define.EnemyState.WALKING:
                UpdateWalking();
                break;
            case Define.EnemyState.RUNNING:
                UpdateRunning();
                break;
            case Define.EnemyState.SEARCHING:
                UpdateSearching();
                break;
        }
    }

    private void UpdateIdle()
    {
        agent.speed = 0;
        agent.isStopped= true;
        if(CanSeePlayer())//플레이어가 에너미의 시선에 잡히면 walking상태로 전이
        {
            try
            {
                state = Define.EnemyState.WALKING;
                anim.SetTrigger("WALKING");
                agent.isStopped = false;
                WalkingStartTime = Time.time;//걷는 시간을 변수에 저장한다.
            }
            catch(SystemException e)
            {
                Debug.LogError($"Error during IDLE : {e.Message}");
            }
           
        }
    }

    private void UpdateWalking()
    {
        try
        {
            if (agent.isOnNavMesh)
            {
                agent.isStopped = false;
                agent.speed = 0.5f;
                agent.destination = target.transform.position;//에너미의 목표를 플레이어로 설정
                float distance = Vector3.Distance(transform.position, target.transform.position);
                
                if(distance<=5)
                {
                    state = Define.EnemyState.RUNNING;
                    anim.SetTrigger("RUNNING");
                    return;
                }
                if(Time.time - WalkingStartTime >= WalkingDurtaion && distance>=10)// 에너미가 3초 이상 걸으며, 플레이어가 시야 반경 내에 없다면 탐색 상태로 전이
                {
                    state = Define.EnemyState.SEARCHING;
                    anim.SetTrigger("SEARCHING");
                    agent.isStopped = false;

                    dfsStack.Clear();//스택 초기화 후 진행 경로를 스택에 push.
                    visitedPositions.Clear();//지나온 위치 클리어.
                    dfsStack.Push(transform.position);//현재 위치를 스택에 push.
                }

            }
            else
            {
                Debug.Log("Agent is not on a NaviMesh.");
            }
        }
        catch(SystemException e)
        {
            Debug.LogError($"Error during WALKING : {e.Message}");
        }
    }

    private void UpdateRunning()
    {
        try
        {
            if(agent.isOnNavMesh)
            {
              agent.isStopped = false;
                agent.speed = 1.5f;
                agent.destination = target.transform.position;
                float distance = Vector3.Distance(transform.position, target.transform.position);

                if(distance > 3 && distance <=6)
                {
                    state = Define.EnemyState.WALKING;
                    anim.SetTrigger("WALKING");
                    WalkingStartTime = Time.time;
                }
                else if(distance >6)
                {
                    state = Define.EnemyState.SEARCHING;
                    anim.SetTrigger("SEARCHING");
                    agent.isStopped = false;

                    dfsStack.Clear();
                    visitedPositions.Clear();
                    dfsStack.Push(transform.position);
                }
            }
            else
            {
                Debug.LogError("Agent is not on a NavMesh.");
            }
        }
        catch(SystemException e)
        {
            Debug.LogError($"Error in UpdateRunning method: {e.Message}");
        }
    }

    private void UpdateSearching()
    {
        try
        {
            if(agent.isOnNavMesh)
            {
                agent.isStopped = false;
                agent.speed = 0.3f;

                if(CanSeePlayer())
                {
                    state = Define.EnemyState.WALKING;
                    anim.SetTrigger("WALKING");
                    WalkingStartTime = Time.time;
                    return;
                }
                if(!agent.hasPath || agent.remainingDistance <0.5)// 에너미가 목적지를 가지고 있지 않거나, 현재 경로의 남은 거리가 0.5 미만일 경우
                {
                    if(dfsStack.Count>0)// 스택에 요소가 존재하면
                    {
                        Vector3 currentPos = dfsStack.Pop();//가장 마지막에 스택에 추가된 위치를 pop하여 방문 위치로 표시
                        visitedPositions.Add(currentPos);//현재 위치를 해시셋에 추가하여 중복 방문을 방지.

                        Vector3[] directions = { Vector3.forward, Vector3.back, Vector3.left, Vector3.right };
                        // 앞 뒤 좌 우 4방향으로 탐색
                        foreach(Vector3 direction in directions)
                        {
                            //각 방향으로 5미터 떨어진 새로운 위치를 계산한다.
                            Vector3 newPos = currentPos + direction * 0.5f;
                            NavMeshHit hit;
                            if(NavMesh.SamplePosition(newPos, out hit, 5.0f, NavMesh.AllAreas) && !visitedPositions.Contains(hit.position))
                            {
                                //새롭게 계산된 위치가 Bake 된 NavMesh 맵 위에 유효한 부분인지 확인. 즉, 에너미가 이동할 수 있는 부분인지 확인. Hit는 유효한 위치 정보를 반환.
                                //유효한 위치가 아직 미방문이라면 해당 위치를 스택에 추가. 
                                dfsStack.Push(hit.position);
                            }
                        }
                        agent.destination = currentPos;//에너미의 목적지를 새로운 위치로 설정.
                    }
                    else
                    {
                        //탐색을 다 마친 경우 idle상태로 돌아감.
                        state = Define.EnemyState.IDLE;
                        anim.SetTrigger("IDLE");
                    }
                }
            }
        }
        catch(SystemException e)
        {
            Debug.LogError($"Error in UpdateSearching method: {e.Message}");
        }
    }

    private bool CanSeePlayer()//에너미의 시야에 플레이어가 들어왔는지 판단하기 위해 RayCast를 사용.
    {
        Vector3 directionToPlayer = (target.position - (transform.position + Vector3.up * 1.0f)).normalized;//에너미 포지션과 플레이어 포지션의 차 : 플레이어까지의 방향벡터
        //에너미 위치 + 방향벡터 조정을 통해 Ray 발사 방향이 상체가 되며 일직선으로 발사될 수 있도록 함
        float distanceToPlayer = Vector3.Distance(transform.position + Vector3.up * 1.0f, target.position);//에너미 ~ 플레이어 까지의 거리

        float maxViewAngle = 60.0f;//시야 각 제한 각도(60도)
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);//에너미의 forward방향과 플레이어 방향 간 각도 계산
        //에너미의 직선방향 벡터와 (플레이어 위치좌표값 - 에너미 위치좌표값)의 벡터 간 각도를 계산한다.

        if (angleToPlayer <= maxViewAngle)
        {
            Vector3 rayOrigin = transform.position + Vector3.up * 1.0f;//ray가 에너미의 머리 높이에서 발사되도록 설정

            Debug.DrawRay(rayOrigin, directionToPlayer * 10.0f, Color.red);
            //에너미와 플레이어 사이 각도가 에너미 시야 각 제한 각도 범위 내 이면
            RaycastHit hit;//레이캐스트 변수 생성 --> 에너미와 플레이어 사이의 장애물 유무 확인 가능
            if (Physics.Raycast(rayOrigin, directionToPlayer, out hit, 10.0f))//매개변수 : 레이캐스트 시작 방향, 뻗어가는 방향, out hit, 레이 길이
            {
                if (hit.transform == target)//Ray가 플레이어에게 Hit되었다면 참을 반환
                {
                    Debug.Log("Ray Hit to Player");
                    return true;
                }
                else
                {
                    Debug.Log("Ray did not hit the player, hit: " + hit.transform.name);
                }
            }
            else
            {
                Debug.Log("Ray did not hit anything.");
            }
        }
        else
        {
            Debug.Log("Player is out of view angle.");
        }
        return false;// 에너미 시야에 플레이어가 없다면 거짓을 반환.
    }
}

*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DFSEnemyAI : MonoBehaviour
{
    public Transform target;
    private NavMeshAgent agent;
    private Define.EnemyState state;
    public Animator anim;

    private float walkingStartTime; // 걷기 시작한 시간
    private const float WalkingDuration = 3.0f; // 걷기 한계 시간

    private Stack<Vector3> dfsStack = new Stack<Vector3>(); // DFS 탐색을 위한 스택
    private HashSet<Vector3> visitedPositions = new HashSet<Vector3>(); // 방문한 위치를 저장하는 해쉬셋 집합. 해쉬셋이므로 중복 허용x

    void Start() // 초기 상태는 IDLE
    {
        state = Define.EnemyState.IDLE;
        agent = GetComponent<NavMeshAgent>();
        agent.isStopped = false;
    }

    void Update()
    {
        switch (state)
        {
            case Define.EnemyState.IDLE:
                UpdateIdle();
                break;
            case Define.EnemyState.WALKING:
                UpdateWalking();
                break;
            case Define.EnemyState.RUNNING:
                UpdateRunning();
                break;
            case Define.EnemyState.SEARCHING:
                UpdateSearching();
                break;
        }
    }

    private void UpdateIdle()
    {
        agent.speed = 0;
        agent.isStopped = true;
        if (CanSeePlayer()) // 플레이어가 에너미의 시선에 잡히면 walking상태로 전이
        {
            SetState(Define.EnemyState.WALKING, "WALKING");
            walkingStartTime = Time.time; // 걷는 시간을 변수에 저장한다.
        }
    }

    private void UpdateWalking()
    {
        if (agent.isOnNavMesh)
        {
            agent.isStopped = false;
            agent.speed = 0.3f;
            SetState(Define.EnemyState.WALKING, "WALKING");
            if(CanSeePlayer())
            {
                agent.destination = target.transform.position; // 에너미의 목표를 플레이어로 설정
                float distance = Vector3.Distance(transform.position, target.transform.position);
                if (distance <= 5)
                {
                    SetState(Define.EnemyState.RUNNING, "RUNNING");
                    return;
                }
                if (Time.time - walkingStartTime >= WalkingDuration && distance >= 10) // 에너미가 3초 이상 걸으며, 플레이어가 시야 반경 내에 없다면 탐색 상태로 전이
                {
                    SetState(Define.EnemyState.SEARCHING, "SEARCHING");
                    dfsStack.Clear(); // 스택 초기화 후 진행 경로를 스택에 push.
                    visitedPositions.Clear(); // 지나온 위치 클리어.
                    dfsStack.Push(transform.position); // 현재 위치를 스택에 push.
                }
            }
           
        }
        else
        {
            Debug.Log("Agent is not on a NavMesh.");
        }
    }

    private void UpdateRunning()
    {
        if (agent.isOnNavMesh)
        {
            agent.isStopped = false;
            agent.speed = 1.5f;
            agent.destination = target.transform.position;
            float distance = Vector3.Distance(transform.position, target.transform.position);

            if (distance > 3 && distance <= 6)
            {
                SetState(Define.EnemyState.WALKING, "WALKING");
                walkingStartTime = Time.time;
            }
            else if (distance > 6)
            {
                SetState(Define.EnemyState.SEARCHING, "SEARCHING");
                dfsStack.Clear();
                visitedPositions.Clear();
                dfsStack.Push(transform.position);
            }
        }
        else
        {
            Debug.LogError("Agent is not on a NavMesh.");
        }
    }

    private void UpdateSearching()
    {
        if (agent.isOnNavMesh)
        {
            agent.isStopped = false;
            agent.speed = 0.3f;

            if (CanSeePlayer())
            {
                SetState(Define.EnemyState.WALKING, "WALKING");
                walkingStartTime = Time.time;
                return;
            }

            if (!agent.hasPath || agent.remainingDistance < 0.5f) // 에너미가 목적지를 가지고 있지 않거나, 현재 경로의 남은 거리가 0.5 미만일 경우
            {
                if (dfsStack.Count > 0) // 스택에 요소가 존재하면
                {
                    Vector3 currentPos = dfsStack.Pop(); // 가장 마지막에 스택에 추가된 위치를 pop하여 방문 위치로 표시
                    visitedPositions.Add(currentPos); // 현재 위치를 해시셋에 추가하여 중복 방문을 방지.

                    Vector3[] directions = { Vector3.forward, Vector3.back, Vector3.left, Vector3.right };
                    // 앞 뒤 좌 우 4방향으로 탐색
                    foreach (Vector3 direction in directions)
                    {
                        // 각 방향으로 5미터 떨어진 새로운 위치를 계산한다.
                        Vector3 newPos = currentPos + direction * 0.5f;
                        if (NavMesh.SamplePosition(newPos, out NavMeshHit hit, 5.0f, NavMesh.AllAreas) && !visitedPositions.Contains(hit.position))
                        {
                            // 유효한 위치가 아직 미방문이라면 해당 위치를 스택에 추가.
                            dfsStack.Push(hit.position);
                        }
                    }
                    agent.destination = currentPos; // 에너미의 목적지를 새로운 위치로 설정.
                }
                else
                {
                    // 탐색을 다 마친 경우 idle상태로 돌아감.
                    SetState(Define.EnemyState.IDLE, "IDLE");
                }
            }
        }
    }

    private bool CanSeePlayer() // 에너미의 시야에 플레이어가 들어왔는지 판단하기 위해 RayCast를 사용.
    {
        Vector3 directionToPlayer = (target.position - (transform.position + Vector3.up * 1.0f)).normalized; // 에너미 포지션과 플레이어 포지션의 차 : 플레이어까지의 방향벡터
        float distanceToPlayer = Vector3.Distance(transform.position + Vector3.up * 1.0f, target.position); // 에너미 ~ 플레이어 까지의 거리

        float maxViewAngle = 60.0f; // 시야 각 제한 각도(60도)
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer); // 에너미의 forward방향과 플레이어 방향 간 각도 계산

        if (angleToPlayer <= maxViewAngle)
        {
            Vector3 rayOrigin = transform.position + Vector3.up * 1.0f; // ray가 에너미의 머리 높이에서 발사되도록 설정
            Debug.DrawRay(rayOrigin, directionToPlayer * 10.0f, Color.red);

            if (Physics.Raycast(rayOrigin, directionToPlayer, out RaycastHit hit, 10.0f)) // 매개변수 : 레이캐스트 시작 방향, 뻗어가는 방향, out hit, 레이 길이
            {
                if (hit.transform == target) // Ray가 플레이어에게 Hit되었다면 참을 반환
                {
                    Debug.Log("Ray Hit to Player");
                    return true;
                }
                else
                {
                    Debug.Log("Ray did not hit the player, hit: " + hit.transform.name);
                    SetState(Define.EnemyState.SEARCHING, "SEARCHING");
                }
            }
            else
            {
                Debug.Log("Ray did not hit anything.");
            }
        }
        else
        {
            Debug.Log("Player is out of view angle.");
        }
        return false; // 에너미 시야에 플레이어가 없다면 거짓을 반환.
    }

    private void SetState(Define.EnemyState newState, string animationTrigger)
    {
        state = newState;
        anim.SetTrigger(animationTrigger);
    }
}
