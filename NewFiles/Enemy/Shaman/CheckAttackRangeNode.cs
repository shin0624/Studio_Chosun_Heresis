using System;
using System.Collections.Generic;
using UnityEngine;

public class CheckAttackRangeNode : BehaviorNode
{
        //공격 가능 거리인지 판단하는 노드.
        private ShamanController shaman;
        
        public CheckAttackRangeNode(ShamanController shaman)
        {
            this.shaman = shaman;
        }

    public override Status Evaluate()
    {
        float distance = Vector3.Distance(shaman.transform.position, shaman.player.position);
        return distance <= shaman.attackRange ? Status.Success : Status.Failure;//공격 가능 거리라면 success를 반환. 아니라면 failure를 반환하여 다시 트리 로직을 수행.
    }
}