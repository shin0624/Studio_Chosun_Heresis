using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
//using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    Rigidbody rb;

    [Header("Rotate")]
    public float mouseSpeed;//개발 단계 Inspector창에서의 빠른 수정를 위해 public으로 선언하나, 코드 안정화 후 캡슐화를 위해 Serialized Field로 설정할 것.
    float yRotation;
    float xRotation;
    Camera cam;//CameraController를 따로 작성하지 않고, GameObject를 상속받는 Camera타입을 가져와 선언.

    [Header("Move")]
    public float moveSpeed;

    float smoothXRotation;
    float smoothYRotation;//카메라 회전 시 떨림 방지를 위한 입력 스무딩 적용.
    public float rotationSmoothTIme = 0.01f;//입력 스무딩에 사용될 시간. Inspector에서 조정가능.

    //24.05.06 점프 기능 추가
    public int JumpPower;//점프 높이 변수
    private bool IsJumping;//점프 유무 TF

    //24.05.06 사다리 수직이동 코드 추가
    [Header("Ladder")]
    public float climbSpeed;
    private bool isOnLadder;

    //24.05.18 손전등 오브젝트 추가-->카메라에 상속
    [Header("Flash Light")]
    public GameObject FlashLight;



    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;// 마우스 커서를 화면 안에서 고정
        Cursor.visible = false;                  // 마우스 커서를 보이지 않도록 설정

        rb = GetComponent<Rigidbody>();          // Rigidbody 컴포넌트 가져오기
        rb.freezeRotation = true;                // Rigidbody의 회전을 고정하여
                                                 // 물리 연산에 영향을 주지 않도록 설정

        cam = Camera.main;                       // 메인 카메라를 할당

        IsJumping = false;//점프 여부 판단-->맵 바닥의 태그를 Ground로 설정하여 Ground가 아닐 때는 점프 불가하도록(이중 점프 방지)
   
        if(FlashLight!=null)//hierarchy에 손전등이 있다면 카메라의 자식으로 설정
        {
            FlashLight.transform.SetParent(cam.transform);
            FlashLight.transform.localPosition = Vector3.zero;//손전등 위치를 카메라에 맞춘다-->플레이어가 상하로 시선을 옮겨도 손전등이 따라올 수 있게
            FlashLight.transform.localRotation = Quaternion.identity;//손전등의 회전을 초기화
        }
    
    
    
    }

    void Update()
    {

        Rotate();//매 프레임마다 회전 업데이트

        Jump();
    }

    void FixedUpdate()//Move()를 따로 처리하기 위한 FIxedUpdate()
    {
        Move();//매 프레임마다 이동 업데이트
        ClimbLadder();

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

        //입력스무딩을 이용하여 카메라 떨림을 보정. 0.1이 가장 적절.
        smoothXRotation = Mathf.SmoothDamp(smoothXRotation, mouseX, ref smoothXRotation, rotationSmoothTIme);
        smoothYRotation = Mathf.SmoothDamp(smoothYRotation, mouseY, ref smoothYRotation, rotationSmoothTIme);

        yRotation += smoothXRotation;
        xRotation -= smoothYRotation;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);  // 수직 회전 값을 -90도에서 90도 사이로 제한


        cam.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0); // 카메라의 회전을 조절
        transform.rotation = Quaternion.Euler(0, yRotation, 0);             // 플레이어 캐릭터의 회전을 조절

        transform.rotation = Quaternion.Euler(0, yRotation, 0);             // 플레이어 캐릭터의 회전을 조절
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
    }

    void Jump()//점프 메서드 추가
    {
        if(Input.GetKeyDown(KeyCode.Space))//스페이스바 입력 시
        {
            if(!IsJumping)//스페이스바 입력 && Ground태그가 붙은 맵으로부터 떨어졌을 때
            {
                IsJumping = true;
                rb.AddForce(Vector3.up * JumpPower, ForceMode.Impulse);//리지드바디에 힘을 추가해주는 메서드 사용-->ForceMode.Impulse : 리지드바디에 질량(mass)을 사용하여 힘 충격을 추가.
            }
            else//Ground일 경우 점프가 안되도록(무한점프, 이중점프 불가)
            {
                Debug.Log("Don`t Jump while IsJump = false");
                return;
            }
        }
    }
    public void OnCollisionEnter(Collision collision)//Ground 태그 유무 확인
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            IsJumping = false;
        }
    }

    void ClimbLadder()//사다리 수직이동 메서드
    {
        if(isOnLadder)//사다리를 만나면
        {
            rb.useGravity = false;//중력을 false로 설정
            float climbInput = Input.GetAxisRaw("Vertical");//수직 이동 값을 변수에 넣는다.

            if (climbInput > 0) // W 키를 눌렀을 때
            {
                rb.MovePosition(rb.position + Vector3.up * climbSpeed * Time.fixedDeltaTime);
            }
            else if (climbInput < 0) // S 키를 눌렀을 때
            {
                rb.MovePosition(rb.position - Vector3.up * climbSpeed * Time.fixedDeltaTime);
            }
            // float climbInput = Input.GetAxisRaw("Vertical");
            //Vector3 climbvec = transform.up * climbInput;
            //rb.MovePosition(rb.position + climbvec.normalized * climbSpeed * Time.fixedDeltaTime);  
        }
    }
   
    private void OnTriggerEnter(Collider other)//수직 이동 처리를 위해 컬라이더간의 충돌을 감지한다. 또한, jump와 마찬가지로 사다리의 tag를 Ladder로 구분.
    {
        if(other.CompareTag("Ladder"))//사다리와 플레이어의 컬라이더가 충돌하면 수직 이동 발생
        {
            isOnLadder = true;
            Debug.Log("touch!");
        }
        
    }
    private void OnTriggerExit(Collider other)//컬라이더 충돌이 종료되면 수직이동 종료. 
    {
        if(other.CompareTag("Ladder"))
        {
            rb.useGravity = true;
            isOnLadder = false;
            Debug.Log("Leaving Ladder!");
        }
    }
    
}
