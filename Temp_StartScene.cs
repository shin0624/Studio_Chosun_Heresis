using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;



public class Temp_StartScene : MonoBehaviour
{
    private Button StartButton;


    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        StartButton = root.Q<Button>("button-start");
        StartButton.clicked += StartButtonClicked;
    }

     void OnDisable()
    {
        StartButton.clicked -= StartButtonClicked;
    }

    private void StartButtonClicked()
    {
        SceneManager.LoadScene("UndergroundScene");
    }


}
