using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShutterRaiseController : MonoBehaviour
{
    //1층 보안실 레버를 올리면 계단을 막고 있는 셔터가 올라가도록 하는 스크립트
    // 수행 과정 : 플레이어가 레버 앞에서 E키를 누르면 레버가 올라가는 애니메이션 작동( + 레버가 올라가는 소리) ->  셔터가 올라가는 애니메이션 작동(+ 셔터가 올라가는 소리)
    //(레버와 셔터 모두 원래 상태로 돌아오는 애니메이션은 넣지 않았음.)

    [SerializeField]
    private GameObject Shutter;
    [SerializeField]
    private GameObject Knob;
    [SerializeField]
    private AudioSource ShutterRaiseSound;
    [SerializeField]
    private AudioSource KnobSound;
    [SerializeField]
    private GameObject SecondFloorTrigger;//레버가 올라가면 다음 목표가 출력되는 트리가를 활성화

    private Animator ShutterAnimator;
    private Animator KnobAnimator;

    private bool isActivated = false;//레버가 이미 올라가 있는지 확인하는 플래그


    void Start()//각 오브젝트별 애니메이터 컴포넌트 가져오기
    {
        ShutterAnimator = Shutter.GetComponent<Animator>();
        KnobAnimator = Knob.GetComponent<Animator>();
        SecondFloorTrigger.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)//플레이어가 트리거 영역에 진입 시
    {
        if(other.CompareTag("PLAYER") && !isActivated)//플레이어가 레버와 상호작용할 수 있는 범위에 있을 때 + 레버가 올라가지 않은 상태일 때
        {
            InputManager.OnInteractKeyPressed += ActivateKnob;//E키 입력을 이벤트로 처리
            
        }
    }

    private void OnTriggerExit(Collider other)//플레이어가 트리거 영역을 벗어났을 시
    {
        if(other.CompareTag("PLAYER"))
        {
            InputManager.OnInteractKeyPressed-= ActivateKnob;
        }
    }

    private void ActivateKnob()
    {
        KnobAnimator.SetTrigger("Raise");
        KnobSound.Play();

        ShutterAnimator.SetTrigger("Raise");
        ShutterRaiseSound.Play();

        isActivated= true;//레버 활성화 표시

        SecondFloorTrigger.SetActive(true);


        InputManager.OnInteractKeyPressed -= ActivateKnob;//이후 입력 이벤트 해제

    }
}
