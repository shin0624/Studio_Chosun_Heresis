using System.Collections;
using UnityEngine;

public class SwingDoor : DoorBase
{
    // 문의 닫힌 상태와 열린 상태의 회전 정보를 저장하는 Quaternion 변수들
    private Quaternion closedRotation;
    private Quaternion openRotation;

    // 시작할 때 실행되는 함수. 문이 닫힌 상태와 열린 상태의 회전을 설정.
    protected override void Start()
    {
        base.Start();                                                           // 부모 클래스의 Start() 메소드를 호출
        closedRotation = door.transform.rotation;                               // 문이 닫힌 상태의 회전값을 저장
        openRotation = closedRotation * Quaternion.Euler(0, 90, 0);             // 문이 열린 상태의 회전을 Y축으로 90도 회전시켜 설정
    }

    // 문을 여는 동작을 시작하는 함수
    protected override void OpenDoor()
    {
        StartCoroutine(RotateDoor(openRotation)); // 문을 여는 회전 애니메이션을 시작
    }

    // 문을 닫는 동작을 시작하는 함수
    protected override void CloseDoor()
    {
        StartCoroutine(RotateDoor(closedRotation)); // 문을 닫는 회전 애니메이션을 시작
    }

    // 문을 회전시키는 코루틴 함수
    private IEnumerator RotateDoor(Quaternion targetRotation)
    {
        isMoving = true; // 문이 움직이고 있다는 플래그를 true로 설정

        // 현재 회전 상태와 목표 회전 상태 사이의 각도가 0.01도 이상인 동안 회전
        while (Quaternion.Angle(door.transform.rotation, targetRotation) > 0.01f)
        {
            // 문을 천천히 목표 회전 상태로 회전시킴
            door.transform.rotation = Quaternion.RotateTowards(door.transform.rotation, targetRotation, openCloseSpeed * Time.deltaTime);
            yield return null; // 다음 프레임까지 대기
        }

        // 문이 정확하게 목표 회전 상태에 도달하면 회전 종료
        door.transform.rotation = targetRotation;
        isMoving = false; // 문이 더 이상 움직이지 않음을 표시
    }
}
