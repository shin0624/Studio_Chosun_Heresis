using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI dialogue;
    private Dictionary<string, string> dialogueDictionary = new Dictionary<string, string>();

    void Start()
    {
        LoadDialoguesFromJSON();
    }

    void LoadDialoguesFromJSON()
    {
        //Resources폴더에서 json파일 로드
        TextAsset jsonTextAsset = Resources.Load<TextAsset>("Dialogue/dialogue");
        if(jsonTextAsset!=null)
        {
            string jsonString = jsonTextAsset.text;//json파일의 텍스트를 스트링변수에 저장
            //json파싱
            try
            {
                DialogueData dialogueData = JsonUtility.FromJson<DialogueData>(jsonString);
                foreach(Dialogue dialogue in dialogueData.dialogues)
                {
                    dialogueDictionary.Add(dialogue.id, dialogue.content);//딕셔너리에 다이얼로그 id와 내용 추가
                }
            }
            catch(System.Exception ex)
            {
                Debug.LogError("JSON 파싱 오류 (DIALOGUE) : " + ex.Message + "\n" + ex.StackTrace);
            }
        }
        else
        {
            Debug.LogError("Dialogue.json 파일을 찾을 수 없음.");
        }


        //string jsonString = Resources.Load<TextAsset>("Dialogue/dialogue").text;
       // DialogueData dialogueData = JsonUtility.FromJson<DialogueData>(jsonString);
        //foreach (Dialogue dialogue in dialogueData.dialogues)
        //{
         //   dialogueDictionary.Add(dialogue.id, dialogue.content);
        //}
    }

    public void DisplayDialogue(string dialogueID)
    {
        if (dialogueDictionary.ContainsKey(dialogueID))
        {
            StopAllCoroutines();
            dialogue.text = dialogueDictionary[dialogueID];
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
        Color color = dialogue.color;
        color.a = 0;
        dialogue.color = color;

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / duration);
            dialogue.color = color;
            yield return null;
        }
    }

    private IEnumerator FadeOutDialogue(float duration)
    {
        Color color = dialogue.color;
        float startAlpha = color.a;

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(startAlpha - (elapsedTime / duration));
            dialogue.color = color;
            yield return null;
        }

        dialogue.text = string.Empty;
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
