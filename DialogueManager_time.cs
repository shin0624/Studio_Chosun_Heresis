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
            dialogueText.text = dialogueDictionary[dialogueID];
            StartCoroutine(ClearDialogueAfterDelay(5f));
        }
    }

    private IEnumerator ClearDialogueAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
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
