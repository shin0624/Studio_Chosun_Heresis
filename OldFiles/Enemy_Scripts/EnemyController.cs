using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
public class EnemyController : MonoBehaviour
{
    //ENEMY AI 스크립트 --> package manager -> AI Navigater 임포트 후 에너미 오브젝트에 NavMeshAgent 컴포넌트 추가

    public Transform target;//에너미의 타겟
    NavMeshAgent agent;
    Define.EnemyState state;//Define스크립트의 에너미 상태 열거체 변수 선언
    public Animator anim;//애니메이터 변수

    float WalkingStartTime;
    float WalkingDuration = 3.0f;





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
    }

    private void UpdateIdle()
    {
        agent.speed = 0;
        agent.isStopped = true;

        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance>4 && distance<7)//플레이어와 에너미의 거리가4이상 7이하(4<distance<7) 에너미가 걷기 시작
        {
            try
            {
                agent.isStopped = false;
                state = Define.EnemyState.WALKING;
                anim.SetTrigger("WALKING");
                Debug.Log("idle->walking");
                

                WalkingStartTime = Time.time;
            }
            catch(System.Exception e)
            {
                Debug.LogError($"Error during IDLE : {e.Message}");
            }
    
            
        }
        else
            state = Define.EnemyState.IDLE;
    }

    private void UpdateWalking()
    {
       
        try
        {
            agent.isStopped = false;
            agent.speed = 0.5f;
            
            if (agent.isOnNavMesh)
            {
                agent.destination = target.transform.position;
                Debug.Log("Walking Now...Target : Player");
                float distance = Vector3.Distance(transform.position, target.transform.position);
              
                if (distance <=3)     
                {
                    state = Define.EnemyState.RUNNING;
                    anim.SetTrigger("RUNNING");
                   
                }


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
        catch(System.Exception e)
        {
            Debug.LogError($"Error during WALKING : {e.Message}");
        }
       

    }

    private void UpdateRunning()
    {
        try 
        {
            agent.isStopped = false;
            agent.speed = 1.5f;//달리는 속도는 1.5f -->0에서 점점 빨라지게 하고싶음
            if(agent.isOnNavMesh)
            {
                agent.destination = target.transform.position;//에너미의 목적지를 플레이어로 설정
                float distance = Vector3.Distance(transform.position, target.transform.position);
                if (distance== 2)
                {
                    //state = Define.EnemyState.ATTACK;//아직 공격 안만듦.
                    //anim.SetTrigger("ATTACK");
                    Debug.Log("distance = 2");
                }
                else if (distance >= 6)//플레이어가 에너미로부터 멀리 도망갔다면 다시 idle상태로.
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
   
}