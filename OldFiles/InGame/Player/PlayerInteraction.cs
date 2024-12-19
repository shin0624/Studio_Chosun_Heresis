/*
 * PlayerInteraction.cs
 * 이 스크립트는 플레이어가 상호작용 가능한 오브젝트와 상호작용을 처리
 * 카메라 시점을 기준으로 레이캐스트를 사용하여 상호작용 가능한 오브젝트를 탐지하고,
 * 상호작용 UI를 제어하며, E 키를 눌렀을 때 상호작용을 실행
 */
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Camera")]
    public Camera cam; // 플레이어 카메라
    
    [Header("UI Elements")]
    public GameObject focusUI;      // 포커스 UI
    public GameObject interactUI;   // 상호작용 UI

    [Header("Settings")]
    public float interactDistance = 3f;             // 상호작용 가능한 최대 거리를 설정
    public string interactableTag = "Interactable"; // 상호작용 가능한 오브젝트에 설정된 태그

    void Update()
    {
        RaycastHit hit;
        // 플레이어 카메라의 위치에서 정면 방향으로 레이캐스트
        // 레이캐스트는 지정된 거리 (interactDistance) 내에서 상호작용 가능한 오브젝트를 탐지
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, interactDistance))
        {
            // 레이캐스트가 히트한 오브젝트가 상호작용 가능한 태그를 가지고 있는지 확인
            if (hit.collider.CompareTag(interactableTag))
            {
                // 히트한 오브젝트에서 Interactable 컴포넌트를 찾기
                Interactable interactable = hit.collider.GetComponent<Interactable>();
                if (interactable != null)
                {
                    // 상호작용 가능한 오브젝트에 포커스가 맞춰졌을 경우 UI를 업데이트
                    UpdateUI(true);

                    // 플레이어가 E 키를 눌렀을 때 상호작용 메서드를 호출
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        interactable.Interact(); // Interactable 컴포넌트의 Interact 메서드를 호출
                    }
                }
            }
            else
            {
                // 레이캐스트가 상호작용 가능한 태그가 아닌 오브젝트에 히트한 경우 UI를 업데이트
                UpdateUI(false);
            }
        }
        else
        {
            // 레이캐스트가 아무 것도 히트하지 않은 경우 UI를 기본 상태로 업데이트
            UpdateUI(false);
        }
    }

    // UI 요소를 업데이트하여 상호작용 상태에 따라 포커스 UI와 상호작용 UI를 전환
    private void UpdateUI(bool isInteracting)
    {
        // 상호작용 중이면 상호작용 UI, 아니면 포커스 UI를 표시
        focusUI.SetActive(!isInteracting);
        interactUI.SetActive(isInteracting);
    }
}
