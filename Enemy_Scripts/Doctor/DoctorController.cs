using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoctorController : MonoBehaviour
{
    [SerializeField] private GameObject Doctor;
    [SerializeField] private GameObject Player;
    [SerializeField] private float Speed;

    [SerializeField] private Animator Anim;
    [SerializeField] private const float ChaseRange = 12.0f;//플레이어 추격 가능 범위
    [SerializeField] private const float DetectionRange = 8.0f;// 플레이어 탐지 거리
    [SerializeField] private const float AttackRange = 1.0f;// 공격 가능 범위
    public JailDoorEventController Flag;

    private Define.DoctorState state;//닥터 상태변수
    private float DistanceToPlayer;//플레이어와의 거리를 저장할 변수

    void Start()
    {
        Doctor = GameObject.FindWithTag("Doctor");
        Player = GameObject.FindWithTag("Player");
        Anim = gameObject.GetComponent<Animator>();
        state = Define.DoctorState.IDLE;
        
    }


    void Update()
    {
        
    }

    void AwakeDoctor()
    {
        if(Flag.isEnter)
        {
            SetState(Define.DoctorState.WALKING, "WALKING");//플레이어가 지하 2층에 들어오면 움직이기 시작한다.
        }
    }

    private void SetState(Define.DoctorState NewState, string AnimationTrigger)// 상태변경 메서드
    {
        if (state != NewState) { state = NewState; Anim.SetTrigger(AnimationTrigger); }//불필요한 상태 변경을 최소화. 각각의 상태에 맞게 애니메이터의 트리거를 바꾸어준다
    }

}
