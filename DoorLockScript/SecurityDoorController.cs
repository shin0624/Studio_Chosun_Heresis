using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SecurityDoorController : MonoBehaviour
{
    private Rigidbody rb;
    public DoorLockController doorLockController;

    private float OpenAngle = -4.4f;//비밀번호 정답 시 문이 벌컥 열리고 고정되는 각도
    private float CloseAngle = 88.156f;//문이 닫힌 상태의 각도
    private bool isOpening = false;//문이 열리는 중인지 여부
    private float doorSpeed = 2.0f;//문이 열리는 속도


    public AudioSource LockedDoorSound;//문이 잠긴 상태에서 E버튼을 눌렀을 때 
    public AudioSource OpenDoorSound;//비밀번호가 입력되어 문이 열렸을 때

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotationY; // 초기 상태: 문이 닫혀 있음
        }
        if(LockedDoorSound==null || OpenDoorSound == null)
        {
            Debug.Log("AudioSource is NULL (Security Door). ");
        }

    }

    void Update()
    {
        if(doorLockController ==null)//도어락 ui가 활성화되지 않은 상태.
        {
            doorLockController = FindObjectOfType<DoorLockController>();//DoorLockController스크립트가 있는 오브젝트를 탐색
        }
        if(doorLockController!=null)
        {
            if (doorLockController.OpenDoorFlag == false && Input.GetKeyDown(KeyCode.E))// 잠긴 문에 E 키를 누르면 잠긴 소리가 재생
            {
                LockedDoorSound.Play();
                LockedDoorSound.loop = false;
            }
            if (rb != null && doorLockController.OpenDoorFlag)//비밀번호가 맞아 문이 열리면
            {
                rb.constraints &= ~RigidbodyConstraints.FreezeRotationY; // 비밀번호가 맞으면 Y축 회전 고정 해제
                //비트 반전 -> y축 회전 고정 프로퍼티의 비트를 0으로 반전
                Debug.Log("Door rotation unlocked.");
                OpenDoorSound.Play();
                OpenDoorSound.loop = false;
                isOpening = true;//문 열림 시작
            }
        }
        if (isOpening)//문이 열리는 중일 때
        {
            float newYRotation = Mathf.LerpAngle(transform.eulerAngles.y, OpenAngle, Time.deltaTime * doorSpeed);
            //각도 선형보간->현재 y축각도와 문이 열렸을 때의 각도를 문 열리는 속도에 맞추어 조절
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, newYRotation, transform.eulerAngles.z);//문 로테이션 값 수정

            if(Mathf.Abs(transform.eulerAngles.y - OpenAngle) < 0.1f)//열린 후 각도 - 계획한 각도가 같으면 차가 0이 될 것이므로.
            {
                isOpening = false;
                rb.constraints = RigidbodyConstraints.FreezeRotationY; // 문이 열린 후 회전 고정
            }
            
        }
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        
    }
}