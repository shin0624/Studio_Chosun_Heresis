using System.Collections;
using UnityEngine;

public class SwingDoor : DoorBase
{
    // ���� ���� ���¿� ���� ������ ȸ�� ������ �����ϴ� Quaternion ������
    private Quaternion closedRotation;
    private Quaternion openRotation;

    // ������ �� ����Ǵ� �Լ�. ���� ���� ���¿� ���� ������ ȸ���� ����.
    protected override void Start()
    {
        base.Start();                                                           // �θ� Ŭ������ Start() �޼ҵ带 ȣ��
        closedRotation = door.transform.rotation;                               // ���� ���� ������ ȸ������ ����
        openRotation = closedRotation * Quaternion.Euler(0, 90, 0);             // ���� ���� ������ ȸ���� Y������ 90�� ȸ������ ����
    }

    // ���� ���� ������ �����ϴ� �Լ�
    protected override void OpenDoor()
    {
        StartCoroutine(RotateDoor(openRotation)); // ���� ���� ȸ�� �ִϸ��̼��� ����
    }

    // ���� �ݴ� ������ �����ϴ� �Լ�
    protected override void CloseDoor()
    {
        StartCoroutine(RotateDoor(closedRotation)); // ���� �ݴ� ȸ�� �ִϸ��̼��� ����
    }

    // ���� ȸ����Ű�� �ڷ�ƾ �Լ�
    private IEnumerator RotateDoor(Quaternion targetRotation)
    {
        isMoving = true; // ���� �����̰� �ִٴ� �÷��׸� true�� ����

        // ���� ȸ�� ���¿� ��ǥ ȸ�� ���� ������ ������ 0.01�� �̻��� ���� ȸ��
        while (Quaternion.Angle(door.transform.rotation, targetRotation) > 0.01f)
        {
            // ���� õõ�� ��ǥ ȸ�� ���·� ȸ����Ŵ
            door.transform.rotation = Quaternion.RotateTowards(door.transform.rotation, targetRotation, openCloseSpeed * Time.deltaTime);
            yield return null; // ���� �����ӱ��� ���
        }

        // ���� ��Ȯ�ϰ� ��ǥ ȸ�� ���¿� �����ϸ� ȸ�� ����
        door.transform.rotation = targetRotation;
        isMoving = false; // ���� �� �̻� �������� ������ ǥ��
    }
}
