using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseInit : MonoBehaviour
{
    [SerializeField] private GameObject m_PauseUI;

    void Awake()
    {
        m_PauseUI.SetActive(false); // ÃÊ±â UI ¼û±è
    }

   
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            m_PauseUI.SetActive(true);
        }
    }
}
