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
        return Status.Running;
    }
}
