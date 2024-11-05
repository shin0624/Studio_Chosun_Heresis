using UnityEngine;

public abstract class ObjectiveController : MonoBehaviour
{
    protected ObjectiveManager objectiveManager;

    void Start()
    {
        objectiveManager = FindObjectOfType<ObjectiveManager>();
        OnStart();
    }

    void Update()
    {
        OnUpdate();
    }

    protected abstract void OnStart();
    protected abstract void OnUpdate();

    protected void DisplayObjective(string newObjectiveID, string objectiveID)
    {
        objectiveManager.DisplayObjectives(newObjectiveID, objectiveID);
        //newObjectiveID : 가운데에 출력된 후 페이드아웃되는 목표
        //objectiveID : 오른쪽에 계속 출력되는 목표
        //JSON 파일에서 : 숫자 + a가ㅣ newObjectiveID, 숫자 + B가 objectiveID
    }
}
