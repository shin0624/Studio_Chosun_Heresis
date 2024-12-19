using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    private Dictionary<string, string> dialogueDictionary = new Dictionary<string, string>();

    void Start()
    {
        LoadDialoguesFromJSON();
    }

    void LoadDialoguesFromJSON()
    {
        string jsonString = Resources.Load<TextAsset>("Dialogue/dialogue").text;
        DialogueData dialogueData = JsonUtility.FromJson<DialogueData>(jsonString);
        foreach (Dialogue dialogue in dialogueData.dialogues)
        {
            dialogueDictionary.Add(dialogue.id, dialogue.content);
        }
    }

    public void DisplayDialogue(string dialogueID)
    {
        if (dialogueDictionary.ContainsKey(dialogueID))
        {
            StopAllCoroutines();
            dialogueText.text = dialogueDictionary[dialogueID];
            StartCoroutine(FadeInDialogue(1f));
            StartCoroutine(ClearDialogueAfterDelay(5f));
        }
    }

    private IEnumerator ClearDialogueAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(FadeOutDialogue(1f));
    }

    private IEnumerator FadeInDialogue(float duration)
    {
        Color color = dialogueText.color;
        color.a = 0;
        dialogueText.color = color;

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / duration);
            dialogueText.color = color;
            yield return null;
        }
    }

    private IEnumerator FadeOutDialogue(float duration)
    {
        Color color = dialogueText.color;
        float startAlpha = color.a;

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(startAlpha - (elapsedTime / duration));
            dialogueText.color = color;
            yield return null;
        }

        dialogueText.text = string.Empty;
    }
}

[System.Serializable]
public class DialogueData
{
    public List<Dialogue> dialogues;
}

[System.Serializable]
public class Dialogue
{
    public string id;
    public string content;
}
