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
    private Coroutine doorCoroutine;
    


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
        if(other.CompareTag("PLAYER")|| other.CompareTag("Player") && !IsOpen )
        {
            OpenDoor();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //플레이어가 트리거 영역 내에 있는 동안 문이 닫히지 않도록 함.
        if(other.CompareTag("PLAYER")|| other.CompareTag("Player"))
        {
            IsOpen = true;
            if(doorCoroutine!=null) 
                StopCoroutine(doorCoroutine); 
                doorCoroutine = null;//문닫기 코루틴 실행중이라면 중단
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("PLAYER") || other.CompareTag("Player") &&IsOpen && doorCoroutine==null)
        {
            doorCoroutine = StartCoroutine(CloseDoorWithDelay(2.0f));
        }

    }

    private void OpenDoor()
    {
       
        DoorAnimator.SetTrigger(OpenTriggerName);
        ado.Play();
        IsOpen = true;

    }

    private IEnumerator CloseDoorWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        //플레이어가 다시 들어오지 않았다면 문을 닫는다.
        if(!IsOpen)
        {
            yield break;//문이 이미 닫혀있따면 코루틴 종료
        }
            DoorAnimator.SetTrigger(CloseStringName);
            ado.Play();
            IsOpen=false;
            doorCoroutine = null;
    }




}
