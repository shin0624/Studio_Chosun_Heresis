using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IdleActionNode : BehaviorNode
{
    //idle 애니메이션을 실행하는 노드
     private ShamanController shaman;
     public IdleActionNode(ShamanController shaman)
     {
        this.shaman= shaman;
     }
    public override Status Evaluate()
    {
        shaman.animator.SetTrigger("IDLE");
        return Status.Success;//12.22 IDLE상태 지속 및 시퀀스 평가 중단 오류 해결 방안 : Running 대신 Success를 반환. 이렇게 하면 거리체크노드가 계속 재평가되고, 플레이어가 범위 내에 접근 시 IDLE 시퀀스 실패, 다음 노드가 평가되어 공격 행동이 시작될 수 있음.
    }
}
