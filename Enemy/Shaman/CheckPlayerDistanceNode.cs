using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class CheckPlayerDistanceNode : BehaviorNode
{
    //플레이어와의 거리를 측정하고, 성공여부에 따라 IDLE 실행 혹은 루트노드의 두 번째 셀렉터로 이동한다.
    //Success-> IDLE / Failure -> 공격or 목조르기를 고르는 시퀀스로 이동
    private ShamanController shaman;
    private float distance;// 플레이어-보스 간 거리
    private bool invertCheck; // 간격 내에 플레이어가 존재 하는가?

    public CheckPlayerDistanceNode(ShamanController shaman, float distance, bool invertCheck)
    {//생성자에서 초기화
        this.shaman = shaman;
        this.distance = distance;
        this.invertCheck = invertCheck;
    }

    public override Status Evaluate()//평가함수 오버라이드. IDLE상태로 갈지, 공격 상태로 넘어갈지 체크한다.
    {
        float currentDistance = Vector3.Distance(shaman.transform.position, shaman.player.position);// 두 캐릭터 간 거리 계산
        bool inRange = currentDistance <=distance;// 현재 두 캐릭터 간 거리가 탐지 거리 이하일 때(간격에 들어왔을 때) true
        return (invertCheck ? !inRange : inRange) ? Status.Success : Status.Failure;

        //invertCheck가 true이면 !inRange(false)이면 success
        //invertCheck가 false이면 inRange(true)이면 failure
        // SetupBT에서 각각 invertCheck의 t/f여부를 시퀀스 별로 넣을 것.
    }
}
