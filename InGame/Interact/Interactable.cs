/*
 * Interactable.cs
 * 이 스크립트는 상호작용 가능한 오브젝트에 기본적으로 사용되는 클래스
 * 다른 상호작용 가능한 오브젝트들이 이 클래스를 상속받아 특정 상호작용 로직을 구현
 */
using UnityEngine;

public class Interactable : MonoBehaviour
{
    // 상호작용 시 호출되는 메서드
    // 이 메서드는 상속받는 클래스에서 오버라이드되어 특정 상호작용 로직을 구현
    public virtual void Interact()
    {
        // 기본 상호작용 로직 (필요 시)
        // 상속받는 클래스에서 이 부분을 확장 또는 대체할 수 있음
    }
}
