using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public Camera cam; // 플레이어 카메라
    public float interactDistance = 6f; // 상호작용 가능한 거리
    public string interactableTag = "Interactable"; // 상호작용 가능한 오브젝트의 태그 설정
    public GameObject focusUI; // 포커스 UI
    public GameObject interactUI; // 상호작용 UI (E 키 아이콘)

    private void Start()
    {
        focusUI = GameObject.Find("RawImage_Focus");
        interactUI = GameObject.Find("RawImage_ButtonE");

        focusUI.SetActive(true);
        interactUI.SetActive(false);
    }

    //수정해야 할 사항 : focusUI, InteractUI 둘 중 하나가 없으면(상호작용 오브젝트가 근처에 없으면) 둘중 하나가 무조건 오류를 출력
    //업데이트문에 작성된거 이벤트 트리거로 빼던지, 다른 방법으로 옮겨야 함.


    //수정사항
    // 1. 오류 로그 삭제 --> ui 둘 중하나가 null일 때 오류로그 출력 x (반복되는 로그출력은 성능에 영향)
    // 2. 의도적으로 null상태를 허용하여 setactive()를 호출하지 않게 함
    // 3. null체크 수행

    void Update()
    {
        RaycastHit hit;

        // 카메라 시점으로 레이캐스트
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, interactDistance))
        {
            // 상호작용 가능한 태그인지 확인
            if (hit.collider.CompareTag(interactableTag))
            {
                Interactable interactable = hit.collider.GetComponent<Interactable>();

                // 상호작용 가능한 오브젝트가 있다면
                if (interactable != null)
                {
                    
                    // 포커스 UI 비활성화, 상호작용 UI 활성화
                    if (focusUI != null && interactUI != null)
                    {
                        focusUI.SetActive(false);
                        interactUI.SetActive(true);
                    }

                    // E 키 입력 시 상호작용 실행
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        interactable.Interact();
                    }
                }
            }
            else
            {
                // 상호작용할 오브젝트가 근처에 없을 때, UI 초기 상태로 복귀
                if (focusUI != null && interactUI != null)
                {
                    focusUI.SetActive(true);
                    interactUI.SetActive(false);
                }
            }
        }
        else
        {
            // 레이캐스트가 아무것도 맞추지 않았을 때 UI 초기 상태로 복귀
            if (focusUI != null && interactUI != null)
            {
                focusUI.SetActive(true);
                interactUI.SetActive(false);
            }
        }
    }
}