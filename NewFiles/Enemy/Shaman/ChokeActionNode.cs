using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class ChokeActionNode : BehaviorNode
{
    //초크 애니메이션을 실행하는 클래스.
    private ShamanController shaman;
    private float chokeTimer = 0.0f;//초크 상태 유지 시간
    private const float CHOKE_DURATION = 3.0f;//초크상태 해제 시간

    public ChokeActionNode(ShamanController shaman)
    {//생성자로 초기화
        this.shaman = shaman;
    }
    public override Status Evaluate()
    {
        if(!shaman.isChoking)//초크 미실행일 때
        {
            shaman.isChoking = true;
            chokeTimer = 0.0f;//평가 시작 시 마다 새로 타이머 맞춤
            shaman.animator.SetTrigger("StartChoke");//애니메이터에서 초크 상태 세트

            SanityManager sanitymanager = GameObject.FindAnyObjectByType<SanityManager>();
            if(sanitymanager != null)
            {
                sanitymanager.DecreaseSanity();//초크 상태 진입 시 정신력 감소 호출. 후에 -2되는 메서드로 교체
            }    
        }
        chokeTimer+=Time.deltaTime;//초크 시간 시작
        if(chokeTimer>=CHOKE_DURATION)//초크 해제시간이 되면
        {
            shaman.isChoking = false;
            shaman.animator.SetTrigger("EndChoke");
            return Status.Success;// 초크 성공 반환 -> 다시 행동트리 평가 반복. 초크 종료 후 플레이어가 바로 도망가버리면 IDLE 혹은 CHASE가 될거고, 안도망가고 있다면 ATTACK이나 CHOKE를 다시 할수도 있음.
        }
        return Status.Running;//실행 중
    }

}

