using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChaseActionNode : BehaviorNode
{
    //추격 전 플레이어를 발견하고 ROAR 애니메이션이 재생되어야 함
    private ShamanController shaman;
    private float respawnDistance = 20.0f;//플레이어가 멀어지면 무당은 일정 거리에서 다시 리스폰되어야 함.
    private bool hasRoared = false;// 포효 상태 추적을 위한 플래그
    private float roarTimer = 0.0f;
    private float roarDuration = 2.8f;//ROAR 클립 재생시간
    
    public ChaseActionNode(ShamanController shaman)
    {
        this.shaman = shaman;
    }

    public override Status Evaluate()
    {
        //플레이어와의 거리 확인
        float distanceToPlayer = Vector3.Distance(shaman.transform.position, shaman.player.position);
        
        //플레이어 감지 시 최초 한 번만 ROAR 재생
        if(!hasRoared)
        {
            shaman.animator.SetTrigger("ROAR");
            //shaman.PlayRoarEffects();
            hasRoared = true;
            roarTimer = 0.0f;
        }

        if(hasRoared)//ROAR 클립 지속 시간 동안 대기.
        {
            roarTimer+=Time.deltaTime;
            if(roarTimer < roarDuration){return Status.Running;}//지속시간 체크
        }

        //너무 멀어지면 리스폰 또는 실패.
        if(distanceToPlayer > respawnDistance)
        {
            //Navmesh위의 새로운 위치로 이동.
            NavMeshHit hit;
            Vector3 randomDirection = Random.insideUnitSphere * respawnDistance;//네비메시 위의 원형 랜덤 구간 * 거리
            randomDirection+=shaman.player.position;

            if(NavMesh.SamplePosition(randomDirection, out hit, respawnDistance, NavMesh.AllAreas))//위치가 존재한다면
            {
                shaman.agent.SetDestination(hit.position);
                hasRoared = false; // 포효 상태 초기화
                return Status.Failure;
            }
        }

        //플레이어 추적
        shaman.agent.SetDestination(shaman.player.position);
        
        //추적 애니메이션 클립 재생
        shaman.animator.SetBool("IsRunnig", true);
        shaman.agent.speed = 3.0f;
        
        //공격 범위에 들어오면 성공
        if(distanceToPlayer <= shaman.attackRange)
        {
            shaman.animator.SetBool("IsRunning", false); 
            hasRoared = false;
            return Status.Success;//다음 노드로 전환.
        }

        return Status.Running;//계속 추적하는 중임을 반환.
    }

}
