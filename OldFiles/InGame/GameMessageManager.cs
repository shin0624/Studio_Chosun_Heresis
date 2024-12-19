/*
 * GameMessageManager.cs
 * 이 스크립트는 게임 내에서 대사 및 목표 메시지를 관리
 * JSON 파일에서 대사 및 목표 데이터를 로드하고, 이를 처리하여 UI 요소를 업데이트
 * Assets/Resources/JSON 경로에 dialogue.json, objective.json 파일 필요
 */
using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GameMessageManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI dialogueText;            // 대사 텍스트 UI 요소
    public TextMeshProUGUI ObjectiveText;           // 목표 텍스트 UI 요소
    public TextMeshProUGUI currentObjectiveText;    // 현재 목표 텍스트 UI 요소

    // 대사와 목표 ID와 내용을 매핑하는 딕셔너리
    private Dictionary<string, string> dialogueDictionary = new Dictionary<string, string>();
    private Dictionary<string, string> objectiveDictionary = new Dictionary<string, string>();

    // 대사와 목표를 관리하는 큐
    private Queue<string> dialogueQueue = new Queue<string>();
    private Queue<(string, string)> objectiveQueue = new Queue<(string, string)>();

    // 대사와 목표를 처리하는 코루틴에 대한 참조
    private Coroutine dialogueCoroutine;
    private Coroutine objectiveCoroutine;

    void Start()
    {
        // JSON 파일에서 대사 및 목표 데이터를 로드
        LoadData();
    }

    // JSON 파일에서 대사와 목표 데이터를 로드하는 메서드
    void LoadData()
    {
        LoadDialoguesFromJSON();
        LoadObjectivesFromJSON();
    }

    // JSON 파일에서 대사 데이터를 로드하여 딕셔너리에 저장하는 메서드
    void LoadDialoguesFromJSON()
    {
        TextAsset jsonTextAsset = Resources.Load<TextAsset>("JSON/dialogue");
        if (jsonTextAsset != null)
        {
            DialogueData dialogueData = JsonUtility.FromJson<DialogueData>(jsonTextAsset.text);
            foreach (Dialogue dialogue in dialogueData.dialogues)
            {
                dialogueDictionary[dialogue.id] = dialogue.content;
            }
        }
        else
        {
            // Debug.LogWarning("Dialogue JSON 파일을 로드할 수 없습니다.");
        }
    }

    // JSON 파일에서 목표 데이터를 로드하여 딕셔너리에 저장하는 메서드
    void LoadObjectivesFromJSON()
    {
        TextAsset jsonTextAsset = Resources.Load<TextAsset>("JSON/objective");
        if (jsonTextAsset != null)
        {
            ObjectiveData objectiveData = JsonUtility.FromJson<ObjectiveData>(jsonTextAsset.text);
            foreach (Objective objective in objectiveData.objective)
            {
                objectiveDictionary[objective.id] = objective.content;
            }
        }
        else
        {
            // Debug.LogWarning("Objective JSON 파일을 로드할 수 없습니다.");
        }
    }

    // 대사를 큐에 추가하고, 대사가 처리되고 있지 않을 경우 코루틴을 시작
    public void DisplayDialogue(string dialogueID)
    {
        if (dialogueDictionary.TryGetValue(dialogueID, out string dialogueContent))
        {
            dialogueQueue.Enqueue(dialogueID);
            if (dialogueCoroutine == null)
            {
                dialogueCoroutine = StartCoroutine(ProcessDialogueQueue());
            }
        }
    }

    // 새 목표와 현재 목표를 큐에 추가하고, 목표가 처리되고 있지 않을 경우 코루틴을 시작
    public void DisplayObjective(string objectiveID, string currentObjectiveID)
    {
        if (objectiveDictionary.TryGetValue(objectiveID, out string objectiveContent))
        {
            objectiveQueue.Enqueue((objectiveID, currentObjectiveID));
            if (objectiveCoroutine == null)
            {
                objectiveCoroutine = StartCoroutine(ProcessObjectiveQueue());
            }
        }
    }

    // 대사 큐를 처리하는 코루틴. 대사 내용을 표시하고, 페이드 효과를 적용
    private IEnumerator ProcessDialogueQueue()
    {
        while (dialogueQueue.Count > 0)
        {
            string dialogueID = dialogueQueue.Dequeue();
            dialogueText.text = dialogueDictionary[dialogueID];
            yield return StartCoroutine(FadeInText(dialogueText, 1f));
            yield return new WaitForSeconds(3f);
            yield return StartCoroutine(FadeOutText(dialogueText, 1f));
        }
        dialogueCoroutine = null;
    }

    // 목표 큐를 처리하는 코루틴. 새 목표와 현재 목표를 표시하고, 페이드 효과를 적용
    private IEnumerator ProcessObjectiveQueue()
    {
        while (objectiveQueue.Count > 0)
        {
            var (objectiveID, currentObjectiveID) = objectiveQueue.Dequeue();
            if (objectiveDictionary.TryGetValue(objectiveID, out string objectiveContent))
            {
                ObjectiveText.text = objectiveContent;
                yield return StartCoroutine(FadeInText(ObjectiveText, 1f));
                yield return new WaitForSeconds(3f);
                yield return StartCoroutine(FadeOutText(ObjectiveText, 1f));
            }

            if (objectiveDictionary.TryGetValue(currentObjectiveID, out string currentObjectiveContent))
            {
                currentObjectiveText.text = currentObjectiveContent;
            }
        }
        objectiveCoroutine = null;
    }

    // 텍스트의 페이드인 효과를 적용하는 코루틴
    private IEnumerator FadeInText(TextMeshProUGUI text, float duration)
    {
        yield return FadeText(text, duration, 0f, 1f);
    }

    // 텍스트의 페이드아웃 효과를 적용하는 코루틴
    private IEnumerator FadeOutText(TextMeshProUGUI text, float duration)
    {
        yield return FadeText(text, duration, 1f, 0f);
    }

    // 텍스트의 페이드 효과를 적용하는 코루틴
    private IEnumerator FadeText(TextMeshProUGUI text, float duration, float startAlpha, float endAlpha)
    {
        float elapsedTime = 0f;
        Color color = text.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            SetTextAlpha(text, alpha);
            yield return null;
        }

        SetTextAlpha(text, endAlpha);
    }

    // 텍스트의 알파 값을 설정하는 메서드
    private void SetTextAlpha(TextMeshProUGUI text, float alpha)
    {
        Color color = text.color;
        color.a = alpha;
        text.color = color;
    }
}

// 대화 데이터의 JSON 형식에 대응하는 클래스
[System.Serializable]
public class DialogueData
{
    public List<Dialogue> dialogues; // 대사 목록
}

// 대화 항목을 나타내는 클래스
[System.Serializable]
public class Dialogue
{
    public string id;       // 대사 ID
    public string content;  // 대사 내용
}

// 목표 데이터의 JSON 형식에 대응하는 클래스
[System.Serializable]
public class ObjectiveData
{
    public List<Objective> objective;   // 목표 목록
}

// 목표 항목을 나타내는 클래스
[System.Serializable]
public class Objective
{
    public string id;       // 목표 ID
    public string content;  // 목포 내용 
}