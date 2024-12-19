using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BehaviorNode 
{
    //행동 트리 노드의 기본 클래스.
    protected BehaviorNode parent;
    protected List<BehaviorNode> children = new List<BehaviorNode>();//BT의 자식 노드들을 저장할 리스트
    public enum Status //BT의 노드 상태 열거체 (설공, 실패, 동작 중)
    {
        Success, 
        Failure, 
        Running
        }
    public abstract Status Evaluate(); // 하위 클래스에서 사용될 상태 평가 메서드
    public void AddChild(BehaviorNode child)//리스트에 자식 노드를 저장할 메서드
    {
        child.parent = this;
        children.Add(child);
    }
}
