using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody rb;

    [Header("Rotate")]
    public float mouseSpeed;//개발 단계 Inspector창에서의 빠른 수정를 위해 public으로 선언하나, 코드 안정화 후 캡슐화를 위해 Serialized Field로 설정할 것.
    float yRotation;
    float xRotation;
    Camera cam;//CameraController를 따로 작성하지 않고, GameObject를 상속받는 Camera타입을 가져와 선언.

    float smoothXRotation;
    float smoothYRotation;//카메라 회전 시 떨림 방지를 위한 입력 스무딩 적용.
    public float rotationSmoothTIme = 0.1f;//입력 스무딩에 사용될 시간. Inspector에서 조정가능.

    [Header("Move")]
    public float moveSpeed;


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;   // 마우스 커서를 화면 안에서 고정
        Cursor.visible = false;                     // 마우스 커서를 보이지 않도록 설정

        rb = GetComponent<Rigidbody>();             // Rigidbody 컴포넌트 가져오기
        rb.freezeRotation = true;                   // Rigidbody의 회전을 고정하여 물리 연산에 영향을 주지 않도록 설정

        cam = Camera.main;                          // 메인 카메라를 할당
        
    }

    void Update()
    {
        Rotate();//매 프레임마다 회전 업데이트
    }

    void FixedUpdate()//Move()를 따로 처리하기 위한 FIxedUpdate()
    {
        Move();//매 프레임마다 이동 업데이트
    }


    void Move()
    {
        //h,v를 지역변수로 설정하여 move호출 시에만 메모리를 할당하므로 메모리 사용 최적화 가능
       float h = Input.GetAxisRaw("Horizontal"); // 수평 이동 입력 값
       float v = Input.GetAxisRaw("Vertical");   // 수직 이동 입력 값

        // 입력에 따라 이동 방향 벡터 계산
        Vector3 moveVec = transform.forward * v + transform.right * h;

        //Rigidbody를 이용하여 이동하게 하여 벽 통과를 방지.
        rb.MovePosition(rb.position + moveVec.normalized * moveSpeed * Time.fixedDeltaTime);
    }


    void Rotate()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSpeed * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSpeed * Time.deltaTime;
#if 카메라떨림보정_수정전
        yRotation += mouseX;    // 마우스 X축 입력에 따라 수평 회전 값을 조정
        xRotation -= mouseY;    // 마우스 Y축 입력에 따라 수직 회전 값을 조정
#endif
        //입력스무딩을 이용하여 카메라 떨림을 보정. 0.1이 가장 적절.
        smoothXRotation = Mathf.SmoothDamp(smoothXRotation, mouseX, ref smoothXRotation, rotationSmoothTIme);
        smoothYRotation = Mathf.SmoothDamp(smoothYRotation, mouseY, ref smoothYRotation, rotationSmoothTIme);

        yRotation += smoothXRotation;
        xRotation -= smoothYRotation;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);  // 수직 회전 값을 -90도에서 90도 사이로 제한

        cam.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0); // 카메라의 회전을 조절
        transform.rotation = Quaternion.Euler(0, yRotation, 0);             // 플레이어 캐릭터의 회전을 조절
    }
}
