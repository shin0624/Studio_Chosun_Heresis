
using System.Collections.Generic;
using UnityEngine;
public class AttackActionNode : BehaviorNode
{
    private ShamanController shaman;
    private float attackTimer = 0.0f;//공격 타이머
    private float attackCooldown = 2.0f;//쿨타임
    private int attackIndex = 0;
    public AttackActionNode(ShamanController shaman)
    {
        this.shaman = shaman;
    }
    public override Status Evaluate()
    {
        //플레이어와의 거리 확인
        float distanceToPlayer = Vector3.Distance(shaman.transform.position, shaman.player.position );

        //공격 범위를 벗어나면 실패
        if(distanceToPlayer > shaman.attackRange)
        {
            return Status.Failure;
        }

        //공격 쿨다운 및 타이머 관리
        attackTimer+=Time.deltaTime;

        if(attackTimer >=attackCooldown)//쿨다운 넘어가면 다시 공격
        {
            shaman.animator.SetTrigger("ATTACK");

            //공격 판정 로직
            RaycastHit hit;
            if(Physics.Raycast(shaman.transform.position, shaman.transform.forward, out hit, shaman.attackRange))
            {
                if(hit.collider.CompareTag("Player") || hit.collider.CompareTag("PLAYER"))
                {
                    SanityManager sanitymanager = GameObject.FindAnyObjectByType<SanityManager>();
                    if(sanitymanager != null)
                    {
                        sanitymanager.DecreaseSanity();//공격 상태 진입 시 정신력 감소 호출.
                    }    
                }
            }
            attackTimer = 0.0f;//어택타이머 초기화.
            return Status.Success;//성공을 반환.
        }
        return Status.Running;//쿨다운 상태이면 러닝상태 유지
    }
}