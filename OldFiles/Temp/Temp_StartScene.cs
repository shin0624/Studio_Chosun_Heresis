using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

//포트폴리오 제출용 미완성 Heresis UI 연결 스크립트

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
