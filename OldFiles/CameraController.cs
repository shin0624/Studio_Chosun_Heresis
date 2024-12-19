using System.Collections;
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

    //--------공격받으면 카메라가 흔들리는 기능 관련 코드---------------
    private float shakeDuration = 0.5f;//흔들림 지속시간
    private float shakeAmount = 0.7f;//흔들림 강도
    private Vector3 originalPos;
    private float noiseOffsetX;//펄린노이즈용 변수 추가-->연속적인 난수를 생성하는 펄린노이즈를 사용하여 코루틴 내에서의 Random.Range 사용보다 자연스러운 흔들림 유도 가능
    private float noiseOffsetY;
    void Start()
    {
        cam.transform.localPosition = new Vector3(0, 0.5f, -0.1f); // 카메라를 플레이어 뒤로 이동
        cam.transform.localRotation = Quaternion.identity; // 카메라의 회전을 초기화
        originalPos = cam.transform.localPosition;//카메라 현재 위치
        //펄린노이즈 적용을 위해 랜덤한 시작점을 설정
        noiseOffsetX = Random.Range(0f, 1000f);
        noiseOffsetY = Random.Range(0f,1000f);
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

    public void StartShake()// 공격을 받으면 카메라 흔들림을 시작. DecreaseSanity()가 호출될 때 호출.
    {
        StopAllCoroutines();//이전 흔들림이 있다면 중지함.
        StartCoroutine(ShakeCoroutine());
    }

    private IEnumerator ShakeCoroutine()
    {
        float elapsed = 0.0f;
        while(elapsed < shakeDuration)
        {
            elapsed+=Time.deltaTime;
            float percentComplete = elapsed / shakeDuration;//흔들림이 완료되는 시간
            float damper = 1.0f - Mathf.Clamp(percentComplete, 0.0f, 1.0f);//시간이 지날 수록 흔들림이 자연스럽게 감소하도록 한다.
            
            //펄린 노이즈를 사용한 흔들림 적용. elapsed*10을 사용하여 노이즈의 속도를 조절한다. 이 값을 높여서 더 빠른 흔들림을 연출할 수 있을듯.
            float offsetX = Mathf.PerlinNoise(noiseOffsetX + elapsed*10f, 0f );
            float offsetY = Mathf.PerlinNoise(0f, noiseOffsetY + elapsed * 10f);

            //1-에서 1 사이의 값으로 변환
            offsetX = (offsetX * 2.0f - 1.0f) * shakeAmount * damper;
            offsetY = (offsetY * 2.0f - 1.0f) * shakeAmount * damper;
            cam.transform.localPosition = originalPos + new Vector3(offsetX, offsetY, 0);//흔들린 위치를 카메라 로컬포지션에 적용
            
            yield return null;
        }
        cam.transform.localPosition = originalPos;//카메라 원래 위치로 이동.
    }
}
