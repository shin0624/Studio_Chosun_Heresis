using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GameMessageManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText; // 대사 텍스트를 표시할 UI 요소
    public TextMeshProUGUI objectiveText; // 목표 텍스트를 표시할 UI 요소
    public TextMeshProUGUI newObjectiveText; // 새로운 목표 텍스트를 표시할 UI 요소

    private Dictionary<string, string> dialogueDictionary = new Dictionary<string, string>(); // 대사 딕셔너리
    private Dictionary<string, string> objectiveDictionary = new Dictionary<string, string>(); // 목표 딕셔너리

    void Start()
    {
        LoadDialoguesFromJSON();
        LoadObjectivesFromJSON();
    }

    void LoadDialoguesFromJSON()
    {
        // JSON 파일을 불러와 딕셔너리에 저장 (대사)
        TextAsset jsonTextAsset = Resources.Load<TextAsset>("Json/dialogue");
        if (jsonTextAsset != null)
        {
            string jsonString = jsonTextAsset.text;
            DialogueData dialogueData = JsonUtility.FromJson<DialogueData>(jsonString);

            foreach (Dialogue dialogue in dialogueData.dialogues)
            {
                dialogueDictionary.Add(dialogue.id, dialogue.content);
            }
        }
        else
        {
            Debug.LogWarning("Dialogue JSON 파일을 로드할 수 없습니다.");
        }
    }

    void LoadObjectivesFromJSON()
    {
        // JSON 파일을 불러와 딕셔너리에 저장 (목표)
        TextAsset jsonTextAsset = Resources.Load<TextAsset>("Json/objective");
        if (jsonTextAsset != null)
        {
            string jsonString = jsonTextAsset.text;
            ObjectiveData objectiveData = JsonUtility.FromJson<ObjectiveData>(jsonString);

            foreach (Objective objective in objectiveData.objective)
            {
                objectiveDictionary.Add(objective.id, objective.content);
            }
        }
        else
        {
            Debug.LogWarning("Objective JSON 파일을 로드할 수 없습니다.");
        }
    }

    public void DisplayDialogue(string dialogueID)
    {
        // 대사 텍스트 설정
        if (dialogueDictionary.ContainsKey(dialogueID))
        {
            dialogueText.text = dialogueDictionary[dialogueID];
            StartCoroutine(FadeInText(dialogueText, 1f)); // 텍스트 페이드 인
            StartCoroutine(ClearTextAfterDelay(dialogueText, 5f)); // 일정 시간 후 페이드 아웃
        }
    }

    public void DisplayObjectives(string newObjectiveID, string objectiveID)
    {
        // 새로운 목표 텍스트 설정
        if (objectiveDictionary.ContainsKey(newObjectiveID))
        {
            newObjectiveText.text = objectiveDictionary[newObjectiveID];
            StartCoroutine(FadeInText(newObjectiveText, 1f));
            StartCoroutine(ClearTextAfterDelay(newObjectiveText, 5f));
        }

        // 목표 텍스트 설정
        if (objectiveDictionary.ContainsKey(objectiveID))
        {
            objectiveText.text = objectiveDictionary[objectiveID];
        }
    }

    private IEnumerator ClearTextAfterDelay(TextMeshProUGUI text, float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(FadeOutText(text, 1f)); // 텍스트 페이드 아웃
    }

    private IEnumerator FadeInText(TextMeshProUGUI text, float duration)
    {
        yield return FadeText(text, duration, 0f, 1f);
    }

    private IEnumerator FadeOutText(TextMeshProUGUI text, float duration)
    {
        yield return FadeText(text, duration, 1f, 0f);
    }

    private IEnumerator FadeText(TextMeshProUGUI text, float duration, float startAlpha, float endAlpha)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            SetTextAlpha(text, alpha);
            yield return null;
        }

        SetTextAlpha(text, endAlpha);
    }

    private void SetTextAlpha(TextMeshProUGUI text, float alpha)
    {
        Color color = text.color;
        color.a = alpha;
        text.color = color;
    }
}

[System.Serializable]
public class DialogueData
{
    public List<Dialogue> dialogues; // 대사 리스트
}

[System.Serializable]
public class Dialogue
{
    public string id; // 대사 ID
    public string content; // 대사 내용
}

[System.Serializable]
public class ObjectiveData
{
    public List<Objective> objective; // 목표 리스트
}

[System.Serializable]
public class Objective
{
    public string id; // 목표 ID
    public string content; // 목표 내용
}
