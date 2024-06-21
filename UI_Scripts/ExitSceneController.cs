using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitSceneController : MonoBehaviour
{
    //마지막 씬 컨트롤 스크립트. 진입 시 음악이 흐르고 ui가 표시되며 esc를 누르면 게임이 종료된다. 
    public AudioSource ado;

    void Start()
    {
        ado = GetComponent<AudioSource>();
        if(ado!=null && !ado.isPlaying)
        {
            ado.loop = true;
            ado.volume = 1.0f;
            ado.Play();
          
        }
    }

    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
