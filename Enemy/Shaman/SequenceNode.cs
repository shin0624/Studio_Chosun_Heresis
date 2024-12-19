using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceNode : BehaviorNode
{// 시퀀스노드 : 모든 자식 노드들을 순차적으로 실행. and게이트와 동일(모든 자식노드 성공 시 전체 성공, 하나라도 실패면 전체 실패)

    private int current = 0;
    public override Status Evaluate()//상태 평가
    {   Debug.Log("Sequence Node!");     
        if(current >= children.Count)// current는 자식노드가 모두 성공했을 때 자식 노드 수와 같아짐.
        {
            current = 0;//다시 0으로 초기화
            return Status.Success;//current값으로 성공 여부를 구별한다.
        }

        Status childStatus = children[current].Evaluate();//각 자식 노드가 성공했는지 확인
        
        switch(childStatus)
        {
            case Status.Running : return Status.Running;//실행 중
            case Status.Success : current++; return current >=children.Count ? Status.Success : Status.Running;//각 노드가 성공이면 current 증가 후 success 반환, 다음 자식으로 이동
            case Status.Failure : current=0; return Status.Failure;//실패
            default : return Status.Success;// 디폴트 성공 반환
        }
    }
}
