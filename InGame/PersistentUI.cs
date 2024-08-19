/*
 * PersistentUI.cs
 * 이 스크립트는 UI 요소가 게임 씬 간에 지속적으로 유지되도록 함
 * 싱글톤 패턴을 사용하여 UI 요소가 여러 씬에서 중복되지 않도록 하며,
 * 씬 전환 시에도 UI 오브젝트가 파괴되지 않도록 설정
 */
using UnityEngine;

public class PersistentUI : MonoBehaviour
{
    // 싱글톤 인스턴스 선언
    private static PersistentUI instance;

    // 스크립트가 처음 활성화될 때 호출되는 메서드
    private void Awake()
    {
        // 싱글톤 인스턴스 설정
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시에도 UI 유지
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 존재하면 파괴
        }
    }
}
