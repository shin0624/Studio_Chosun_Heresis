using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class ObjectiveManager : MonoBehaviour
{
    public TextMeshProUGUI newObjective;
    public TextMeshProUGUI objective;
    
    private Dictionary<string, string> objectiveDictionary = new Dictionary<string, string>();

    void Start()
    {
        LoadObjectivesFromJSON();
    }

    void LoadObjectivesFromJSON()
    {
        // Resources 폴더에서 JSON 파일 로드
        TextAsset jsonTextAsset = Resources.Load<TextAsset>("Objective/objective");
        if (jsonTextAsset != null)
        {
            string jsonString = jsonTextAsset.text;
            // JSON 파싱
            try
            {
                ObjectiveData objectiveData = JsonUtility.FromJson<ObjectiveData>(jsonString);
                foreach (Objective objective in objectiveData.objective)
                {
                    // 딕셔너리에 목표 ID와 내용 추가
                    objectiveDictionary.Add(objective.id, objective.content);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("JSON 파싱 오류: " + ex.Message);
            }
        }
        else
        {
            Debug.LogError("Objective JSON 파일을 찾을 수 없습니다.");
        }
    }

    public void DisplayObjectives(string newObjectiveID, string objectiveID)
    {
        if (objectiveDictionary.ContainsKey(objectiveID))
        {
            objective.text = objectiveDictionary[objectiveID];
        }

        if (objectiveDictionary.ContainsKey(newObjectiveID))
        {
            StopAllCoroutines();
            newObjective.text = objectiveDictionary[newObjectiveID];
            StartCoroutine(FadeInTexts(newObjective, 1f));

        }
    }

    private IEnumerator ClearTextAfterDelay(TextMeshProUGUI newObjective, TextMeshProUGUI newObjectiveText, float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(FadeOutTexts(newObjective, newObjectiveText, 1f));
    }

    private IEnumerator FadeInTexts(TextMeshProUGUI newObjective,  float duration)
    {
        Color colorNewObjective = newObjective.color;
        
        colorNewObjective.a = 0;
       
        newObjective.color = colorNewObjective;
    

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / duration);
            colorNewObjective.a = alpha;
        
            newObjective.color = colorNewObjective;
    
            yield return null;
        }
    }

    private IEnumerator FadeOutTexts(TextMeshProUGUI newObjective, TextMeshProUGUI newObjectiveText, float duration)
    {
        Color colorNewObjective = newObjective.color;
        Color colorNewText = newObjectiveText.color;
        float startAlphaNewObjective = colorNewObjective.a;
        float startAlphaNewText = colorNewText.a;

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(startAlphaNewObjective - (elapsedTime / duration));
            colorNewObjective.a = alpha;
            colorNewText.a = alpha;
            newObjective.color = colorNewObjective;
            newObjectiveText.color = colorNewText;
            yield return null;
        }

        newObjective.text = string.Empty;
        Color finalColor = newObjectiveText.color;
        finalColor.a = 0;
        newObjectiveText.color = finalColor;
    }
}

[System.Serializable]
public class ObjectiveData
{
    public List<Objective> objective; // JSON의 배열을 받는 리스트
}

[System.Serializable]
public class Objective
{
    public string id;
    public string content;
}
