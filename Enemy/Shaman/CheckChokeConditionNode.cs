using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckChokeConditionNode : BehaviorNode
{   //상태트리의 두 번째 자식인 공격Selector에서 choke, attack의 실행 확률을 계산하는 스크립트.
  private ShamanController shaman;
  public CheckChokeConditionNode(ShamanController shaman)
  { //생성자에서 초기화
    this.shaman = shaman;
  }
    public override Status Evaluate()
    {
        if(shaman.isChoking) return Status.Running;//초크 상태가 실행중일 경우 Running(동작중)을 반환
        
        float distance = Vector3.Distance(shaman.transform.position, shaman.player.position);
        
        if(distance <=shaman.chokeRange && Random.value < shaman.chokeProbility)//플레이어가 초크 실행 가능한 거리 내에 있고 랜덤으로 선택된 확률이 초크 실행 가능성보다 낮다면
        {
            return Status.Success;//성공. 2번째 노드의 시퀀스 중 첫째가 초크이므로 성공을 반환하고 초크 행동 시작
        }
        return Status.Failure;// 초크 발생 확률이 랜덤 값보다 낮다면 초크실행x, Attack실행.
    }
}