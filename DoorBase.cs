using System.Collections;
using UnityEngine;

public abstract class DoorBase : MonoBehaviour
{
    public GameObject door; // ������ �� �� ������Ʈ
    public bool isOpen; // ���� ���ȴ��� ���θ� ����
    public GameObject player; // �÷��̾� ������Ʈ
    public float openCloseSpeed = 2.0f; // ���� ������ ������ �ӵ�
    protected bool isMoving = false; // ���� �����̰� �ִ��� ���θ� ����

    protected virtual void Start()
    {
        // �÷��̾� ��ü�� ã��
        player = GameObject.FindWithTag("PLAYER");
        if (player == null)
        {
            Debug.LogError("Player object not found. Make sure the player object has the 'Player' tag.");
        }

        if (door == null)
        {
            Debug.LogError("Door object not set. Assign a door GameObject in the inspector.");
        }
    }

    protected virtual void Update()
    {
        float distance = Vector3.Distance(player.transform.position, transform.position);
        if (distance < 3.0f && Input.GetKeyDown(KeyCode.E) && !isMoving)
        {
            if (isOpen)
            {
                CloseDoor();
            }
            else
            {
                OpenDoor();
            }
            isOpen = !isOpen;
        }
    }

    protected abstract void OpenDoor(); // �� ���� ���� ����
    protected abstract void CloseDoor(); // �� �ݱ� ���� ����
}
