using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourseManager : MonoBehaviour
{
    public GameObject LoadPrefab(string prefabName)
    {
        string prefabPath = "Prefabs/" + prefabName;

        GameObject prefab = Resources.Load<GameObject>(prefabPath);

        if (prefab == null)
        {
            Debug.LogError("Failed to load prefab with name: " + prefabName);
            return null;
        }

        return prefab;
    }

    public GameObject InstantiatePrefab(string prefabName)
    {
        GameObject prefab = LoadPrefab(prefabName);
        if (prefab == null)
        {
            Debug.LogError("Failed to load prefab : " + prefab.name);
            return null;
        }

        return Instantiate(prefab);
    }
}
