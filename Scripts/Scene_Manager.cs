using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene_Manager : MonoBehaviour
{
    private Scene previousScene;

    public void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        // 다음 씬이 존재하는지 확인하고 로드
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning("No next scene available!");
        }
    }

    public void LoadPreviousScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        int previousSceneBuildIndex = currentScene.buildIndex - 1;

        if (previousSceneBuildIndex >= 0)
        {
            previousScene = SceneManager.GetSceneByBuildIndex(previousSceneBuildIndex);
            SceneManager.LoadScene(previousSceneBuildIndex);
        }
        else
        {
            Debug.LogWarning("No previous scene available!");
        }
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
