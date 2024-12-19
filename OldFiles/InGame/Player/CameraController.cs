/*
 * CameraController.cs
 * 이 스크립트는 플레이어와 카메라의 회전을 제어
 * 마우스 입력에 따라 카메라의 수직 및 수평 회전을 조정하며,
 * 부드러운 회전을 위해 `Mathf.SmoothDamp`를 사용
 */
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Player")]
    public Transform player;  // 플레이어의 Transform, 카메라가 따라갈 대상 

    [Header("Camera")]
    public Camera cam;        // 플레이어 카메라

    [Header("Settings")]
    public float mouseSpeed = 100f;         // 마우스 이동 속도, 카메라 회전 속도 조정
    public float rotationSmoothTime = 0.1f; // 회전 값의 부드러운 조정을 위한 시간

    // 카메라의 회전 값을 저장하는 변수들
    private float xRotation = 0f;           // 카메라의 수직 회전 각도
    private float yRotation = 0f;           // 카메라의 수평 회전 각도
    private float currentXRotation = 0f;    // 현재 카메라의 수직 회전 값
    private float currentYRotation = 0f;    // 현재 카메라의 수평 회전 값
    private float xRotationVelocity = 0f;   // 수직 회전 값 변화 속도
    private float yRotationVelocity = 0f;   // 수평 회전 값 변화 속도

    void Start()
    {
        // 카메라의 초기 위치와 회전을 설정
        // cam.transform.SetParent(player); // 카메라를 플레이어의 자식으로 설정
        cam.transform.localPosition = new Vector3(0, 0.5f, -0.1f);  // 카메라를 플레이어 뒤로 이동시킴
        cam.transform.localRotation = Quaternion.identity;          // 카메라의 회전을 초기화하여 기본 상태로 설정
    }

    void Update()
    {
        // 매 프레임마다 카메라 회전 처리
        Rotate();
    }

    // 카메라와 플레이어의 회전을 업데이트하는 메서드
    void Rotate()
    {
        // 마우스 입력을 받아서 카메라 회전 값 계산
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSpeed * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSpeed * Time.deltaTime;

        // 목표 회전 값을 업데이트
        yRotation += mouseX;
        xRotation -= mouseY;

        // 수직 회전 값을 -90도에서 90도 사이로 제한하여 카메라가 넘어가지 않도록 함
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // 현재 회전 값을 목표 회전 값으로 부드럽게 이동
        currentXRotation = Mathf.SmoothDamp(currentXRotation, xRotation, ref xRotationVelocity, rotationSmoothTime);
        currentYRotation = Mathf.SmoothDamp(currentYRotation, yRotation, ref yRotationVelocity, rotationSmoothTime);

        // 카메라의 회전을 적용
        cam.transform.localRotation = Quaternion.Euler(currentXRotation, 0, 0);

        // 플레이어의 회전을 조절
        player.transform.rotation = Quaternion.Euler(0, currentYRotation, 0);
    }
}
