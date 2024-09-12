using System.Collections;
using UnityEngine;

public class SlidingDoor : DoorBase
{
    private Vector3 closedPosition; // 닫힌 위치
    private Vector3 openPosition; // 열린 위치
    public float openMeter = 1.0f; // 문이 열리는 거리

    protected override void Start()
    {
        base.Start();
        closedPosition = door.transform.position; // 현재 위치를 닫힌 위치로 저장
        openPosition = closedPosition + new Vector3(0, 0, openMeter); // 열린 위치 계산
    }

    protected override void OpenDoor()
    {
        StartCoroutine(SlideDoor(openPosition)); // 문을 여는 코루틴 시작
    }

    protected override void CloseDoor()
    {
        StartCoroutine(SlideDoor(closedPosition)); // 문을 닫는 코루틴 시작
    }

    private IEnumerator SlideDoor(Vector3 targetPosition)
    {
        isMoving = true;
        // 문이 목표 위치에 도달할 때까지 움직임
        while (Vector3.Distance(door.transform.position, targetPosition) > 0.01f)
        {
            door.transform.position = Vector3.MoveTowards(door.transform.position, targetPosition, openCloseSpeed * Time.deltaTime);
            yield return null; // 다음 프레임까지 대기
        }
        door.transform.position = targetPosition; // 목표 위치에 도달 후 위치 고정
        isMoving = false;
    }
}
