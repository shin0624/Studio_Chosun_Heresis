using System.Collections;
using UnityEngine;

public class SlidingDoor : DoorBase
{
    private Vector3 closedPosition; // ���� ��ġ
    private Vector3 openPosition; // ���� ��ġ
    public float openMeter = 1.0f; // ���� ������ �Ÿ�

    protected override void Start()
    {
        base.Start();
        closedPosition = door.transform.position; // ���� ��ġ�� ���� ��ġ�� ����
        openPosition = closedPosition + new Vector3(0, 0, openMeter); // ���� ��ġ ���
    }

    protected override void OpenDoor()
    {
        StartCoroutine(SlideDoor(openPosition)); // ���� ���� �ڷ�ƾ ����
    }

    protected override void CloseDoor()
    {
        StartCoroutine(SlideDoor(closedPosition)); // ���� �ݴ� �ڷ�ƾ ����
    }

    private IEnumerator SlideDoor(Vector3 targetPosition)
    {
        isMoving = true;
        // ���� ��ǥ ��ġ�� ������ ������ ������
        while (Vector3.Distance(door.transform.position, targetPosition) > 0.01f)
        {
            door.transform.position = Vector3.MoveTowards(door.transform.position, targetPosition, openCloseSpeed * Time.deltaTime);
            yield return null; // ���� �����ӱ��� ���
        }
        door.transform.position = targetPosition; // ��ǥ ��ġ�� ���� �� ��ġ ����
        isMoving = false;
    }
}
