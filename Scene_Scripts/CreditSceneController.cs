using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditSceneController : MonoBehaviour
{
    //크레딧 씬 컨트롤러. 배경음악이 나오고, esc를 누르면 스타트 씬으로 돌아간다.
    public AudioSource ado;

    void Start()
    {
        ado = GetComponent<AudioSource>();
        if (ado != null && !ado.isPlaying)
        {
            ado.loop = true;
            ado.volume = 1.0f;
            ado.Play();

        }
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("StartScene");
        }
    }
}
