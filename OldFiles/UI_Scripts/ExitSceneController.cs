using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Video;

public class ExitSceneController : MonoBehaviour
{
    //마지막 씬 컨트롤 스크립트. 진입 시 음악이 흐르고 ui가 표시되며 esc를 누르면 게임이 종료된다. 
    public AudioSource ado;
    public VideoPlayer vdo;

    private VisualElement root;


    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        ado = GetComponent<AudioSource>();
        if(ado!=null && !ado.isPlaying)
        {
            ado.loop = true;
            ado.volume = 1.0f;
            ado.Play();
          
        }

        root.style.display = DisplayStyle.None;
        vdo = GetComponent<VideoPlayer>();
        if (vdo != null)
        {
            vdo.Play();
            vdo.loopPointReached += OnVideoFinished;
        }


    }

    private void Awake()
    {
        if(GetComponent<UIDocument>() == null)
        {
            Debug.LogError("UIDocument component(in ENDscene) not found!");
            return;
        }
    }

    private void OnVideoFinished(VideoPlayer vdo)
    {
        root.style.display = DisplayStyle.Flex;
    }


    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
