/*
 * PlayerController.cs
 * 이 스크립트는 플레이어의 움직임, 점프, 대쉬, 사다리 타기와 같은 행동을 관리
 * Rigidbody를 사용하여 물리 기반의 이동을 구현하며, 발소리 재생도 포함
 * 싱글톤 패턴을 적용하여 씬 전환 시에도 플레이어 오브젝트를 유지
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 플레이어를 관리하는 PlayerController 스크립트
public class PlayerController : MonoBehaviour
{
    // 싱글톤 인스턴스 선언
    private static PlayerController instance;

    Rigidbody rb;

    [Header("Scripts")]
    public AudioManager audioManager;

    [Header("Sounds")]
    public List<AudioClip> walkFootstepClips;   // 걸을 때 발소리 클립 리스트
    public List<AudioClip> runFootstepClips;    // 뛸 때 발소리 클립 리스트

    [Header("Settings")]
    public float moveSpeed;     // 이동 속도
    public float dashSpeed;     // 대쉬 시 이동 속도 배수
    public float JumpPower;     // 점프 파워
    public float climbSpeed;    // 사다리 오르기 속도

    private bool isJumping;     // 점프 중인지 여부
    private bool isOnLadder;    // 사다리에 있는지 여부
    private bool isMoving;      // 걷는 중인지 여부
    private bool isRunning;     // 달리는 중인지 여부
    private Coroutine footstepCoroutine; // 발소리 재생을 위한 코루틴

    // 스크립트가 처음 활성화될 때 호출되는 메서드
    private void Awake()
    {
        // 싱글톤 인스턴스 설정
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시에도 플레이어 유지
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 존재하면 파괴
        }
    }

    void Start()
    {
        // 마우스 커서를 화면에 고정하고 보이지 않도록 설정
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        rb = GetComponent<Rigidbody>(); // Rigidbody 컴포넌트 가져오기
        rb.freezeRotation = true;       // Rigidbody의 회전을 고정하여 물리 연산에 영향을 주지 않도록 설정

        isMoving = false;   // 걷기 상태 초기화
        isRunning = false;  // 달리기 상태 초기화
        isJumping = false;  // 점프 상태 초기화
    }

    void Update()
    {
        HandleJump(); // 점프 처리
    }

    // 물리 연산을 위한 메서드
    void FixedUpdate()
    {
        HandleMovement(); // 이동 처리
        HandleClimbing(); // 사다리 오르기 처리
    }

    // 플레이어 이동을 처리하는 메서드
    void HandleMovement()
    {
        // 수평, 수직 입력 값을 가져옴
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // 입력에 따른 이동 방향 벡터 계산
        Vector3 moveVec = transform.forward * v + transform.right * h;

        // 기본 이동 속도 설정
        float currentSpeed = moveSpeed;

        // 왼쪽 Shift 키를 누르면 대쉬
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed *= dashSpeed;
            isRunning = true;
        }
        else
        {
            isRunning = false;
        }

        // 플레이어가 움직일 때 발소리 재생 관리
        if (moveVec.magnitude > 0 && !isJumping)
        {
            if (!isMoving)
            {
                isMoving = true;
                // 걷거나 달릴 때 발소리 재생을 위한 코루틴 시작
                footstepCoroutine = StartCoroutine(PlayFootstepSounds());
            }
        }
        else
        {
            isMoving = false;
            // 플레이어가 움직이지 않을 때 발소리 코루틴 중지
            if (footstepCoroutine != null)
            {
                StopCoroutine(footstepCoroutine);
                footstepCoroutine = null;
            }
        }

        // Rigidbody를 이용하여 이동 처리
        rb.MovePosition(rb.position + moveVec.normalized * currentSpeed * Time.fixedDeltaTime);
    }

    // 플레이어 점프를 처리하는 메서드
    void HandleJump()
    {
        // 스페이스바 입력 시 점프 처리
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(!isJumping) // 플레이어가 점프 중이 아닐 때만 점프 가능
            {
                isJumping = true;
                rb.AddForce(Vector3.up * JumpPower, ForceMode.Impulse); // 위쪽으로 힘을 가해 점프
            }
        }
    }

    // 충돌이 발생할 때 호출되는 메서드 (Ground 태그 확인)
    public void OnCollisionEnter(Collision collision)
    {
        // 충돌한 오브젝트의 태그가 "Ground"인 경우
        if (collision.gameObject.CompareTag("Ground"))
         {
            isJumping = false; // 점프 상태 초기화
        }
    }

    // 사다리 오르기를 처리하는 메서드
    void HandleClimbing()
    {
        // 사다리에 있을 때만 실행
        if (isOnLadder)
        {
            rb.useGravity = false; // 중력을 끔
            float climbInput = Input.GetAxisRaw("Vertical"); // 수직 입력 값을 가져옴

            // W 키를 눌렀을 때 위로 이동
            if (climbInput > 0)
            {
                rb.MovePosition(rb.position + Vector3.up * climbSpeed * Time.fixedDeltaTime);
            }
            // S 키를 눌렀을 때 아래로 이동
            else if (climbInput < 0)
            {
                rb.MovePosition(rb.position - Vector3.up * climbSpeed * Time.fixedDeltaTime);
            }
        }
    }

    // 트리거에 진입할 때 호출되는 메서드 (사다리와의 충돌 감지)
    private void OnTriggerEnter(Collider other)
    {
        // 사다리와의 충돌을 감지
        if (other.CompareTag("Ladder"))
        {
            isOnLadder = true;
        }
    }

    // 트리거에서 나갈 때 호출되는 메서드 (사다리에서 벗어남)
    private void OnTriggerExit(Collider other)
    {
        // 사다리에서 벗어남을 감지
        if (other.CompareTag("Ladder"))
        {
            rb.useGravity = true; // 중력을 다시 활성화
            isOnLadder = false;
        }
    }

    // 걸을 때와 뛸 때 발소리를 일정 간격으로 재생하는 코루틴
    private IEnumerator PlayFootstepSounds()
    {
        while (isMoving)
        {
            if (isJumping)
            {
                // 점프 중에는 발소리를 재생하지 않음
                yield return null;
                continue;
            }

            AudioClip footstepClip = null;

            // 뛰는 상태일 때 발소리 재생
            if (isRunning && runFootstepClips.Count > 0)
            {
                // 달리기 발소리 리스트에서 무작위로 클립 선택 후 재생
                footstepClip = runFootstepClips[Random.Range(0, runFootstepClips.Count)];
                audioManager.PlaySound(footstepClip);
                yield return new WaitForSeconds(0.3f); // 발소리 간격
            }
            // 걷는 상태일 때 발소리 재생
            else if (!isRunning && walkFootstepClips.Count > 0)
            {
                // 걷기 발소리 리스트에서 무작위로 클립 선택 후 재생
                footstepClip = walkFootstepClips[Random.Range(0, walkFootstepClips.Count)];
                audioManager.PlaySound(footstepClip);
                yield return new WaitForSeconds(0.6f); // 발소리 간격
            }
        }
    }
}
