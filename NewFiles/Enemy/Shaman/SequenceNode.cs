using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceNode : BehaviorNode // 시퀀스노드 : 모든 자식 노드들을 순차적으로 실행. and게이트와 동일(모든 자식노드 성공 시 전체 성공, 하나라도 실패면 전체 실패)
{
    private int current = 0;
    public override Status Evaluate()//상태 평가
    {   
        if(current >= children.Count)// current는 자식노드가 모두 성공했을 때 자식 노드 수와 같아짐.
        {
            current = 0;//다시 0으로 초기화
            return Status.Success;//current값으로 성공 여부를 구별한다.
        }

        Status childStatus = children[current].Evaluate();//각 자식 노드가 성공했는지 확인
        
        switch(childStatus)
        {
            case Status.Running : 
                return Status.Running;//실행 중 --> 12.22 오류 발견(해결) : 계속 Running을 반환하고 있어서 시퀀스 평가가 중단되어버림. --> idleActionNode 리턴값을 Success로 수정
                
            case Status.Success : 
                current++; 
                return current >=children.Count ? Status.Success : Status.Running;//각 노드가 성공이면 current 증가 후 success 반환, 다음 자식으로 이동
                
            case Status.Failure : 
                current=0; 
                return Status.Failure;//실패
                
            default : return Status.Success;// 플레이어가 감지되지 않았거나 특정 행동을 하지 않는 경우 Idle 상태 유지
        }
    }
}
