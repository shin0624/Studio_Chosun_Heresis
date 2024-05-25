using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
public class RayCastEnemyController : MonoBehaviour
{
    //ENEMY AI 스크립트 --> package manager -> AI Navigater 임포트 후 에너미 오브젝트에 NavMeshAgent 컴포넌트 추가

    public Transform target;//에너미의 타겟
    private NavMeshAgent agent;
    private Define.EnemyState state;//Define스크립트의 에너미 상태 열거체 변수 선언
    public Animator anim;//애니메이터 변수

    private float WalkingStartTime;
    private const float WalkingDuration = 3.0f;


    void Start()
    {
        /*
        agent = GetComponent<NavMeshAgent>();//에너미에게 네비게이터 컴포넌트 부착
        target = GameObject.Find("PLAYER").transform;//플레이어의 위치를 타겟으로 설정
        agent.destination = target.transform.position;//에너미의 목적지를 플레이어로 설정
        */
        state = Define.EnemyState.IDLE;//에너미 첫 상태 : 유휴상태

        agent = GetComponent<NavMeshAgent>();//에너미 정의
        agent.isStopped = true;

    }


    void Update()
    {
#if if_else
        if (state == Define.EnemyState.IDLE)
        {
            UpdateIdle();

        }
        else if (state == Define.EnemyState.RUNNING)
        {
            UpdateRunning();
        }
        else if (state == Define.EnemyState.WALKING)
        {
            UpdateWalking();
        }
#endif
        //anim.SetFloat("MoveSpeed", agent.speed);
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
                // case Define.EnemyState.ATTACK:
                // UpdateAttack();
                // break;
        }


    }

    private void UpdateIdle()
    {
        agent.speed = 0;
        agent.isStopped = true;

        float distance = Vector3.Distance(transform.position, target.transform.position);

        Debug.Log("Checking if can see player...");
        if (CanSeePlayer())//플레이어에게 Ray가 Hit했다면
        {
            try
            {
                state = Define.EnemyState.WALKING;
                anim.SetTrigger("WALKING");
                agent.isStopped = false;
                WalkingStartTime = Time.time;//걷기 상태 시작시간 설정
                Debug.Log("idle->walking");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error during IDLE : {e.Message}");
            }
        }
        else
        {
            state = Define.EnemyState.IDLE;
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
                agent.destination = target.transform.position;
                Debug.Log("Walking Now...Target : Player");

                float distance = Vector3.Distance(transform.position, target.transform.position);
                if (distance <= 5)
                {
                    state = Define.EnemyState.RUNNING;
                    anim.SetTrigger("RUNNING");
                    return;
#if dd
                if (distance >= 6)
                {
                    Debug.Log("too far.. Idle");
                    state = Define.EnemyState.IDLE;
                    anim.SetTrigger("IDLE");
                    agent.isStopped = true;
                }

                if (Time.time - WalkingStartTime >= WalkingDuration)
                {
                    Debug.Log("Walking for 3 seconds...walking-->idle");
                    state = Define.EnemyState.IDLE;
                    anim.SetTrigger("IDLE");
                    agent.isStopped = true;
                }
#endif
                }
                //walkingduration 초과 시 distance가 3 이하여도 idle상태로 전이됨을 해결하기 위해, 로직 수정
                //->running상태 전이 시 walkingduration 조건을 무시해야 함
                if (Time.time - WalkingStartTime >= WalkingDuration)
                {
                    Debug.Log("Walking for 3 seconds...walking-->idle");
                    state = Define.EnemyState.IDLE;
                    anim.SetTrigger("IDLE");
                    agent.isStopped = true;
                }
            }
            else
            {
                Debug.Log("Agent is not on a NaviMesh.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error during WALKING : {e.Message}");
        }
    }

    private void UpdateRunning()
    {
        try
        {
            if (agent.isOnNavMesh)
            {
                agent.isStopped = false;
                agent.speed = 2.0f;//달리는 속도는 1.5f -->0에서 점점 빨라지게 하고싶음
                agent.destination = target.transform.position;//에너미의 목적지를 플레이어로 설정
                float distance = Vector3.Distance(transform.position, target.transform.position);
                Debug.Log("Running Now...Target : Player");

                if (distance > 3 && distance <= 6)
                {
                    // 플레이어와의 거리가 3 이상 6 이하로 멀어지면 걷기 상태로 전환
                    state = Define.EnemyState.WALKING;
                    anim.SetTrigger("WALKING");
                    WalkingStartTime = Time.time; // Walking 상태 시작 시간 재설정
                }
                else if (distance > 6)//플레이어가 에너미로부터 멀리 도망갔다면 다시 idle상태로.
                {
                    state = Define.EnemyState.IDLE;
                    anim.SetTrigger("IDLE");
                    agent.isStopped = true;
                }
            }
            else
            {
                Debug.LogError("Agent is not on a NavMesh.");
            }

        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in UpdateRunning method: {e.Message}");
        }
    }
#if lookaround
    private void UpdateLookAround()
    {
        agent.speed = 0;
        state = Define.EnemyState.LOOK_AROUND;
        anim.SetTrigger("Look Around");

        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance == 2)//거리가 2가 되는 순간 running으로 상태 변경
        {
            state = Define.EnemyState.RUNNING;
            anim.SetTrigger("RUNNING");
        }

    }
#endif

    private bool CanSeePlayer()//에너미의 시야에 플레이어가 들어왔는지 판단하기 위해 RayCast를 사용.
    {
        Vector3 directionToPlayer = (target.position - (transform.position + Vector3.up * 1.0f)).normalized;//에너미 포지션과 플레이어 포지션의 차 : 플레이어까지의 방향벡터
        //에너미 위치 + 방향벡터 조정을 통해 Ray 발사 방향이 상체가 되며 일직선으로 발사될 수 있도록 함
        float distanceToPlayer = Vector3.Distance(transform.position + Vector3.up *1.0f, target.position);//에너미 ~ 플레이어 까지의 거리

        float maxViewAngle = 60.0f;//시야 각 제한 각도(60도)
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);//에너미의 forward방향과 플레이어 방향 간 각도 계산
        //에너미의 직선방향 벡터와 (플레이어 위치좌표값 - 에너미 위치좌표값)의 벡터 간 각도를 계산한다.

        if(angleToPlayer <=maxViewAngle)
        {
            Vector3 rayOrigin = transform.position + Vector3.up* 1.0f;//ray가 에너미의 머리 높이에서 발사되도록 설정

            Debug.DrawRay(rayOrigin, directionToPlayer * 10.0f, Color.red);
            //에너미와 플레이어 사이 각도가 에너미 시야 각 제한 각도 범위 내 이면
            RaycastHit hit;//레이캐스트 변수 생성 --> 에너미와 플레이어 사이의 장애물 유무 확인 가능
            if(Physics.Raycast(rayOrigin, directionToPlayer, out hit, 10.0f ))//매개변수 : 레이캐스트 시작 방향, 뻗어가는 방향, out hit, 레이 길이
            {
                if(hit.transform == target)//Ray가 플레이어에게 Hit되었다면 참을 반환
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