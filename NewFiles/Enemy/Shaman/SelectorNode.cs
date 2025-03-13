using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorNode : BehaviorNode //Selector노드 : 성공할 때 까지 자식 노드를 순차적으로 시도한다. or게이트와 동일
{
    //여러 노드 중 하나만 수행해야 하는 특성을 가짐.
    private int current = 0;
    
    public override Status Evaluate()//구조는 시퀀스 노드와 거의 동일
    {
        //Debug.Log($"Evaluating child {current} of {children.Count}");
        
        Status childStatus = children[current].Evaluate();
        
        switch(childStatus)
        {
            case Status.Running : 
                return Status.Running;
                
            case Status.Success : //성공 시 current 초기화 후 성공 반환
                current = 0;   
                return Status.Success; 
                
            case Status.Failure ://실패 시(행동 노드 전체 실패 시 ->Failure / 아직 전체 다 실패하지 않았을 때 -> running)
                current++; 
                if(current >= children.Count)
                {  
                    current = 0; return Status.Failure;
                }
                return Status.Running;
                
            default :
                return Status.Success;
        }

    }
    }

