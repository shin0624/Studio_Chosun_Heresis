using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
//using UnityEngine.UIElements;

// 플레이어를 관리하는 플레이어 컨트롤러 스크립트. 원활한 개발을 위해 싱글톤 패턴으로 수정
public class PlayerController : MonoBehaviour
{
    private static PlayerController PlayerInstance;// 싱글톤 인스턴스

    Rigidbody rb;

    [Header("Move")]
    public float moveSpeed;

    //24.05.06 점프 기능 추가
    [Header("Jump")]
    public int JumpPower;//점프 높이 변수
    private bool IsJumping;//점프 유무 TF

    //24.05.06 사다리 수직이동 코드 추가
    [Header("Ladder")]
    public float climbSpeed;
    private bool isOnLadder;

    //24.06.18 대쉬 추가
    [Header("Dash")]
    public float dashSpeed;

    private void Awake()
    {
        if (PlayerInstance == null)
        {
            PlayerInstance = this;
            DontDestroyOnLoad(gameObject);//씬이 전환되어도 플레이어가 파괴되지 않도록 조치
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;// 마우스 커서를 화면 안에서 고정
        Cursor.visible = false;                  // 마우스 커서를 보이지 않도록 설정

        rb = GetComponent<Rigidbody>();          // Rigidbody 컴포넌트 가져오기
        rb.freezeRotation = true;                // Rigidbody의 회전을 고정하여 물리 연산에 영향을 주지 않도록 설정

        IsJumping = false;//점프 여부 판단-->맵 바닥의 태그를 Ground로 설정하여 Ground가 아닐 때는 점프 불가하도록(이중 점프 방지)
    }

    void Update()
    {
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

        float currentSpeed = moveSpeed;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed *= dashSpeed;//왼쪽 시프트 누르면 대쉬
        }

        //Rigidbody를 이용하여 이동하게 하여 벽 통과를 방지.
        rb.MovePosition(rb.position + moveVec.normalized * currentSpeed * Time.fixedDeltaTime);

    }

    void Jump()//점프 메서드 추가
    {
        if (Input.GetKeyDown(KeyCode.Space))//스페이스바 입력 시
        {
            //if(!IsJumping)//스페이스바 입력 && Ground태그가 붙은 맵으로부터 떨어졌을 때
            // {
            IsJumping = true;
            rb.AddForce(Vector3.up * JumpPower, ForceMode.Impulse);//리지드바디에 힘을 추가해주는 메서드 사용-->ForceMode.Impulse : 리지드바디에 질량(mass)을 사용하여 힘 충격을 추가.
                                                                   // }
                                                                   // else//Ground일 경우 점프가 안되도록(무한점프, 이중점프 불가)
                                                                   // {
            ///   Debug.Log("Don`t Jump while IsJump = false");
            //  return;
            // }
        }
    }
    /* public void OnCollisionEnter(Collision collision)//Ground 태그 유무 확인
     {
         if(collision.gameObject.CompareTag("Ground"))
         {
             IsJumping = false;
         }
     }*/

    void ClimbLadder()//사다리 수직이동 메서드
    {
        if (isOnLadder)//사다리를 만나면
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
        if (other.CompareTag("Ladder"))//사다리와 플레이어의 컬라이더가 충돌하면 수직 이동 발생
        {
            isOnLadder = true;
            Debug.Log("touch!");
        }
    }
    private void OnTriggerExit(Collider other)//컬라이더 충돌이 종료되면 수직이동 종료. 
    {
        if (other.CompareTag("Ladder"))
        {
            rb.useGravity = true;
            isOnLadder = false;
            Debug.Log("Leaving Ladder!");
        }
    }

}
