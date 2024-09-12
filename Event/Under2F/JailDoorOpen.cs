using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class JailDoorOpen : MonoBehaviour
{
    //지하 2층 감옥문 열림 제어 스크립트. 모든 감옥 문의 y축 Rotation 값은 90 --> 180으로 만들어야 함.
    private GameObject JailDoor;
    private AudioSource JailDoorOpenSound;

    private float OpenDoorAngle = 180.0f;//문이 벌컥 열리는 각도
    private float OpenDoorSpeed = 5.0f;//문이 벌컥 열리는 시간
    private bool OpenDoorTag = false;

    void Start()
    {
        JailDoor = this.gameObject;
        JailDoorOpenSound = JailDoor.GetComponent<AudioSource>();

        if (JailDoor == null)
        {
            Debug.LogError("JailDoor is null");
        }

        if (JailDoorOpenSound == null)
        {
            Debug.LogError("AudioSource is null");
        }
    }

    void Update()
    {
        
    }

    public void StartOpenDoorBust()//문을 벌컥 여는 메서드
    {
        // float newDoorAngle = Mathf.LerpAngle(transform.eulerAngles.y, OpenDoorAngle, Time.deltaTime * OpenDoorSpeed);//각도 선형보간을 통해 (현재 y각, 열릴 때의 y각)을 OpenDoorSpeed에 맞게 조절
        if (OpenDoorTag == false)//T/F값을 사용해 두번 세번 호출될 일이 없게 한다.
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, OpenDoorAngle, transform.eulerAngles.z);//문 각도 설정. 
            

            JailDoorOpenSound.Play();
            JailDoorOpenSound.loop = false;
            OpenDoorTag = true;
        }

       
    }
}
