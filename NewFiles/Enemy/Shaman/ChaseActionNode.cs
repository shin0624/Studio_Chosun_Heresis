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

        if (distanceToPlayer > respawnDistance)//너무 멀어지면 리스폰되거나, 그 자리에서 IDLE상태가 된다.
        {
            int maxAttempts = 10;//랜덤 위치를 찾기 위한 시도 횟수에 제한을 둔다.
            int attempts = 0;
            NavMeshHit hit;
        
            while (attempts < maxAttempts)//시도 제한 횟수까지 반복하며 리스폰 가능 위치를 찾는다.
            {
                Vector3 randomDirection = Random.insideUnitSphere * respawnDistance;
                randomDirection += shaman.player.position;
                
                //SamplePosition 메서드를 사용하여 최대 respawnDistance 까지 탐색하며 랜덤 위치를 찾는다.
                if (NavMesh.SamplePosition(randomDirection, out hit, respawnDistance, NavMesh.AllAreas))
                {
                    shaman.transform.position = hit.position;//에너미 위치를 랜덤 위치로 리스폰.
                    shaman.agent.ResetPath(); // 기존 경로 초기화
                    hasRoared = false; // 포효 상태 초기화
                    return Status.Success; // 리스폰 후 Idle 상태로 전환
                }
        
                attempts++;
            }
        
            shaman.agent.ResetPath(); // 이동 중지
            hasRoared = false; // 포효 상태 초기화
            return Status.Success; // 유효한 위치를 찾지 못한 경우, 현재 위치에서 Idle 상태 유지
        }
        shaman.agent.SetDestination(shaman.player.position); //플레이어 추적
        
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
