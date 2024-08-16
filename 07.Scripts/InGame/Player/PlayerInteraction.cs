using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public Camera cam; // 플레이어 카메라
    public float interactDistance = 3f; // 상호작용 가능한 거리
    public string interactableTag = "Interactable"; // 상호작용 가능한 오브젝트의 태그 설정
    public GameObject focusUI; // 포커스 UI
    public GameObject interactUI; // 상호작용 UI (E 키 아이콘)

    void Update()
    {
        RaycastHit hit;
        // 카메라 시점으로 레이캐스트
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, interactDistance))
        {
            if (hit.collider.CompareTag(interactableTag))
            {
                Interactable interactable = hit.collider.GetComponent<Interactable>();
                if (interactable != null)
                {
                    // 상호작용 가능
                    UpdateUI(true);

                    // E 키를 눌렀을 때 상호작용 실행
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        interactable.Interact();
                    }
                }
            }
            else
            {
                // 상호작용 불가
                UpdateUI(false);
            }
        }
        else
        {
            // 기본 상태
            UpdateUI(false);
        }
    }

    private void UpdateUI(bool isInteracting)
    {
        focusUI.SetActive(!isInteracting);
        interactUI.SetActive(isInteracting);
    }
}
