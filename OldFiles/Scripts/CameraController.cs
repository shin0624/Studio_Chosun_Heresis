using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;  // 플레이어의 Transform
    public Camera cam;        // 카메라
    public float mouseSpeed = 100f; // 마우스 이동 속도
    public float rotationSmoothTime = 0.1f; // 회전 부드러움 조정 시간

    private float xRotation = 0f;  // 카메라의 수직 회전 각도
    private float yRotation = 0f;  // 카메라의 수평 회전 각도
    private float currentXRotation = 0f; // 현재 카메라의 수직 회전 값
    private float currentYRotation = 0f; // 현재 카메라의 수평 회전 값
    private float xRotationVelocity = 0f; // 수직 회전 값 변화 속도
    private float yRotationVelocity = 0f; // 수평 회전 값 변화 속도

    void Start()
    {
        // 카메라를 플레이어의 자식으로 설정
        cam.transform.SetParent(player);
        cam.transform.localPosition = new Vector3(0, 0, -0.1f); // 카메라를 플레이어 뒤로 이동
        cam.transform.localRotation = Quaternion.identity; // 카메라의 회전을 초기화
    }

    void Update()
    {
        Rotate();
    }

    void Rotate()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSpeed * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSpeed * Time.deltaTime;

        // 목표 회전 값 계산
        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // 수직 회전 값을 -90도에서 90도 사이로 제한

        // 현재 회전 값을 목표 회전 값으로 부드럽게 이동
        currentXRotation = Mathf.SmoothDamp(currentXRotation, xRotation, ref xRotationVelocity, rotationSmoothTime);
        currentYRotation = Mathf.SmoothDamp(currentYRotation, yRotation, ref yRotationVelocity, rotationSmoothTime);

        // 카메라의 회전 적용
        cam.transform.localRotation = Quaternion.Euler(currentXRotation, 0, 0);
        player.transform.rotation = Quaternion.Euler(0, currentYRotation, 0); // 플레이어 회전 조절
    }
}
