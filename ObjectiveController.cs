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
    }
}
