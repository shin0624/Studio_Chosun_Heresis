using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AutoSlidingDoor : MonoBehaviour
{
    // 병원 슬라이딩 도어에 사용할 스크립트
    // 플레이어가 가까이 다가오면 문이 열리고, 3초 뒤 닫힌다.
   
    [SerializeField]
    private Animator DoorAnimator;//문을 열고 닫는 애니메이터
    [SerializeField]
    private string OpenTriggerName = "SlidingDoorOpen";//같은 이름으로 설정된 트리거
    [SerializeField]
    private string CloseStringName = "SlidingDoorClose";
    [SerializeField]
    private AudioSource ado;

    private bool IsOpen = false;//문이 열렸는지 여부
    


    void Start()
    {
        if (DoorAnimator == null)
        {
            DoorAnimator = gameObject.GetComponent<Animator>();
        }

        if (ado == null)
        {
            ado = gameObject.GetComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter(Collider other)// 문이 이미 열려있는 경우 OpenDoor()를 호출하지 않아야 함.
    {
        if(other.CompareTag("PLAYER") && !IsOpen )
        {
            OpenDoor();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //플레이어가 트리거 영역 내에 있는 동안 문이 닫히지 않도록 함.
        if(other.CompareTag("PLAYER"))
        {
            IsOpen = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //if(other.CompareTag("PLAYER") && IsOpen )
        //{
            CloseDoor();
       // }

    }

    private void OpenDoor()
    {
       
        DoorAnimator.SetTrigger(OpenTriggerName);
        ado.Play();
        IsOpen = true;

    }

    private void CloseDoor()
    {
     
        DoorAnimator.SetTrigger(CloseStringName);
        ado.Play();
        IsOpen = false;

    }




}
