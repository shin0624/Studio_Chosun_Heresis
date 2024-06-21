using UnityEngine;

// 각 상황에 맞는 다이얼로그를 출력할 수 있도록 추상 클래스로 선언. 이렇게 하면 다른 스크립트에서 원하는 순간에 메서드를 오버라이드하여 다이얼로그 출력 가능
public abstract class DialogController : MonoBehaviour
{
    protected DialogueManager dialogueManager;

    void Start()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();
        OnStart();
    }

    void Update()
    {
            // StartDialogue();
            OnUpdate();
    }

    protected abstract void OnStart();
    protected abstract void OnUpdate();

    protected void StartDialogue(string dialogueID)// 오버라이드한 클래스에서 특정 상황의 dialogueID를 입력하면 그에 맞는 대사 출력.
    {
        dialogueManager.DisplayDialogue(dialogueID);
    }
}
