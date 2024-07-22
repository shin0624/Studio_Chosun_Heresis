using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class ObjectiveManager : MonoBehaviour
{
    public TextMeshProUGUI newObjectiveText;
    public TextMeshProUGUI objectiveText;
    public TextMeshProUGUI text_NewObjective;
    private Dictionary<string, string> objectiveDictionary = new Dictionary<string, string>();

    void Start()
    {
        LoadObjectivesFromJSON();
        Color initialColor = text_NewObjective.color;
        initialColor.a = 0;
        text_NewObjective.color = initialColor;
    }

    void LoadObjectivesFromJSON()
    {
        TextAsset jsonTextAsset = Resources.Load<TextAsset>("Objective/objective");
        if (jsonTextAsset != null)
        {
            string jsonString = jsonTextAsset.text;
            ObjectiveData objectiveData = JsonUtility.FromJson<ObjectiveData>(jsonString);
            foreach (Objective objective in objectiveData.objective)
            {
                objectiveDictionary.Add(objective.id, objective.content);
            }
        }
    }

    public void DisplayObjectives(string newObjectiveID, string objectiveID)
    {
        if (objectiveDictionary.ContainsKey(objectiveID))
        {
            objectiveText.text = objectiveDictionary[objectiveID];
        }

        if (objectiveDictionary.ContainsKey(newObjectiveID))
        {
            StopAllCoroutines();
            newObjectiveText.text = objectiveDictionary[newObjectiveID];
            StartCoroutine(FadeInTexts(newObjectiveText, text_NewObjective, 1f));
            StartCoroutine(ClearTextAfterDelay(newObjectiveText, text_NewObjective, 5f));
        }
    }

    private IEnumerator ClearTextAfterDelay(TextMeshProUGUI newObjectiveText, TextMeshProUGUI text_NewObjective, float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(FadeOutTexts(newObjectiveText, text_NewObjective, 1f));
    }

    private IEnumerator FadeInTexts(TextMeshProUGUI newObjectiveText, TextMeshProUGUI text_NewObjective, float duration)
    {
        Color colorNewObjective = newObjectiveText.color;
        Color colorNewText = text_NewObjective.color;
        colorNewObjective.a = 0;
        colorNewText.a = 0;
        newObjectiveText.color = colorNewObjective;
        text_NewObjective.color = colorNewText;

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / duration);
            colorNewObjective.a = alpha;
            colorNewText.a = alpha;
            newObjectiveText.color = colorNewObjective;
            text_NewObjective.color = colorNewText;
            yield return null;
        }
    }

    private IEnumerator FadeOutTexts(TextMeshProUGUI newObjectiveText, TextMeshProUGUI text_NewObjective, float duration)
    {
        Color colorNewObjective = newObjectiveText.color;
        Color colorNewText = text_NewObjective.color;
        float startAlphaNewObjective = colorNewObjective.a;
        float startAlphaNewText = colorNewText.a;

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(startAlphaNewObjective - (elapsedTime / duration));
            colorNewObjective.a = alpha;
            colorNewText.a = alpha;
            newObjectiveText.color = colorNewObjective;
            text_NewObjective.color = colorNewText;
            yield return null;
        }

        newObjectiveText.text = string.Empty;
        Color finalColor = text_NewObjective.color;
        finalColor.a = 0;
        text_NewObjective.color = finalColor;
    }
}

[System.Serializable]
public class ObjectiveData
{
    public List<Objective> objective;
}

[System.Serializable]
public class Objective
{
    public string id;
    public string content;
}
