
using Core.Scripts.Runtime.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShamanRoomSlidingDoor : MonoBehaviour
{
    //지하 무당집 슬라이딩 도어 스크립트 --> 왼쪽, 오른쪽 문을 한번에 제어하기 위해 문 두쪽의 부모 오브젝트에서 트리거 이벤트를 감지

    [SerializeField] private GameObject LeftDoor;//왼쪽 문
    [SerializeField] private GameObject RightDoor;//오른쪽 문
    [SerializeField] private AudioClip DoorSound01;//문 열리는 소리
    [SerializeField] private AudioClip DoorSound02;

    private Vector3 ClosedLeftPosition;//닫혀있는 왼쪽 문 좌표
    private Vector3 ClosedRightPosition;//닫혀있는 오른쪽 문 좌표
    private Vector3 OpenLeftPosition;//왼쪽 문 열린 후 좌표
    private Vector3 OpenRightPosition;//오른쪽 문 열린 후 좌표
    [SerializeField] private float OpenValue;

    private float SlidingSpeed = 0.1f;// 문 열리는 속도
    private bool isOpening = false;//문이 열렸는지 알려주는 플래그
    private bool PlayerInTrigger = false;// 플레이어가 트리거 영역에 있는지 알려주는 플래그

    void Start()
    {
        LeftDoor = transform.Find("ShamanRoomDoor01").gameObject;// 문 오브젝트의 이름으로 각각 탐색
        RightDoor = transform.Find("ShamanRoomDoor02").gameObject;

        ClosedLeftPosition = LeftDoor.transform.position;//현재 문 위치 정보를 저장
        ClosedRightPosition = RightDoor.transform.position;

        OpenLeftPosition = new Vector3(ClosedLeftPosition.x, ClosedLeftPosition.y, ClosedLeftPosition.z - OpenValue);//z축으로 슬라이딩 된 후의 문 위치값을 저장
        OpenRightPosition = new Vector3(ClosedRightPosition.x, ClosedRightPosition.y, ClosedRightPosition.z + OpenValue);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))//플레이어가 트리거 영역에 진입 시
        {
            PlayerInTrigger = true;//트리거 영역에 진입했다는 플래그를 true로 전달
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))//플레이어가 트리거 영역에서 벗어났을 시
        {
            PlayerInTrigger = false;//트리거 영역에 진입했다는 플래그를 false로 전달
        }
    }

    private void Update()// 업데이트에서 플레이어 트리거 진입 여부와 E키 입력을 체크. OnTriggerEnter에서 E키 입력을 즉시 확인하려 했으나, GetKeyDown이 짧은 시간동안만 동작하여 입력 처리에 오류가 발생했기 때문에 업데이트에서 처리
    {
        if (PlayerInTrigger && !isOpening && Input.GetKeyDown(KeyCode.E))
        {
            isOpening = true;// 플레이어가 트리거 영역에 존재 + 문이 닫힌 상태 + e키 입력 시 문 열림으로 상태 변경
            AudioManager.Instance.PlaySound(DoorSound02);
            AudioManager.Instance.PlaySound(DoorSound01);

        }
        if (isOpening)//문 열림 상태이면
        {

            LeftDoor.transform.position = Vector3.Lerp(LeftDoor.transform.position, OpenLeftPosition, Time.deltaTime * SlidingSpeed);//좌우측 문의 위치를 미리 설정해놓은 위치로 자연스럽게 옮긴다.
            RightDoor.transform.position = Vector3.Lerp(RightDoor.transform.position, OpenRightPosition, Time.deltaTime * SlidingSpeed);

            if (Vector3.Distance(LeftDoor.transform.position, OpenRightPosition) < 0.01f && Vector3.Distance(RightDoor.transform.position, OpenLeftPosition) < 0.01f)// 목표 위치까지 도달했다면
            {
                LeftDoor.transform.position = OpenLeftPosition;//좌우측 문이 자연스럽게 목표 위치로 도달하여 멈출 수 있도록 처리한다.
                RightDoor.transform.position = OpenRightPosition;
                isOpening = false;//문 닫힘상태로 변경
                
            }
        }
    }

}
