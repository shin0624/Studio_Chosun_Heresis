using UnityEngine;

public class ClueObject : MonoBehaviour
{
    public ClueData clueData;

    public void OnPickedUp()
    {
        Destroy(gameObject); // 아이템 오브젝트 삭제
    }
}
