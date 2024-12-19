using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorHinjiController : MonoBehaviour
{
    // 문 오브젝트에 플레이어가 접근해 E 키를 누르면 문이 열리는 스크립트
    //  ** 중요** 문이 정상적으로 열리려면 문 오브젝트의 피벗을 경첩에 해당하는 위치로 옮기고, 리지드바디 컴포넌트(중력사용 x, 키네마틱으로)와 박스컬라이더(트리거로 설정, 문 크기와 위치에 맞추기), Interactable 스크립트, 태그를 Interactable로 바꾸는 작업 필요



    [SerializeField]
    private float OpenAngle;// 문이 열리는 각도
    [SerializeField]
    private float CloseAngle; //문이 닫힌 상태의 각도
    private bool isOpening = false; //문이 열려있는지 여부
    private float DoorSpeed = 2.0f; //문이 열리는 속도

    private Rigidbody rb; //문의 리지드바디 --> 문이 한변 열린 후 상태가 변하는 오류를 방지하기 위해 일단 열어놓은 채로 고정시킴. 후에 닫는 모션 추가

    [SerializeField]
    private AudioSource OpenDoorSound;//문이 열리는 소리

    void Start()
    {
        CloseAngle = gameObject.transform.rotation.eulerAngles.y;

        // 리지드바디와 오디오소스를 붙인다.
        rb = GetComponent<Rigidbody>();
        if(rb==null)
        {
            Debug.Log("Door Rigidbody is NULL");
        }

        OpenDoorSound = GetComponent<AudioSource>();
        if(OpenDoorSound ==null)
        {
            Debug.Log("OpenDoorSound is NULL");
        }
    }

    private void OnTriggerEnter(Collider other)//플레이어가 트리거 영역 진입 시 + 문이 닫힌 상태이면
    {
        if(other.CompareTag("PLAYER") && !isOpening)
        {
            InputManager.OnInteractKeyPressed += OpenDoor;// E키 입력을 이벤트로 처리
        }
    }
    private void OnTriggerExit(Collider other)//플레이어가 트리거 영역을 벗어나면 이벤트 구독 해제
    {
        if (other.CompareTag("PLAYER"))
        {
            InputManager.OnInteractKeyPressed -= OpenDoor;
        }
    }

    private void OpenDoor()// 문의 각도가 변하며 열리는 이벤트 메서드
    {
        isOpening = true; 
        OpenDoorSound.Play();
        OpenDoorSound.loop = false;

        // 선형보간으로 자연스러운 여닫음을 표현
        float newYRotation = Mathf.LerpAngle(transform.eulerAngles.y, OpenAngle, Time.deltaTime * DoorSpeed);
        //문의 새로운 오일러각을 적용
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, newYRotation, transform.eulerAngles.z);

        //기존에 설정했던 열림 각도와 현재 각도가 일치한다면 0이 될 것이므로, 문이 열린 것으로 간주하고 Y각도를 고정
        if(Mathf.Abs(transform.eulerAngles.y - OpenAngle) < 0.1f)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotationY;
        }

    }


}
