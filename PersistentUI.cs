using UnityEngine;

public class PersistentUI : MonoBehaviour
{
    private static PersistentUI instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);  // 이미 인스턴스가 있다면 새로 생성된 오브젝트를 파괴
        }
    }
}
