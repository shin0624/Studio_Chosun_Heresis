using System.Collections;
using UnityEngine;

public abstract class DoorBase : MonoBehaviour
{
    public GameObject door; // 열리게 할 문 오브젝트
    public bool isOpen; // 문이 열렸는지 여부를 저장
    public GameObject player; // 플레이어 오브젝트
    public float openCloseSpeed = 2.0f; // 문이 열리고 닫히는 속도
    protected bool isMoving = false; // 문이 움직이고 있는지 여부를 저장

    protected virtual void Start()
    {
        // 플레이어 객체를 찾음
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

    protected abstract void OpenDoor(); // 문 열기 로직 구현
    protected abstract void CloseDoor(); // 문 닫기 로직 구현
}
