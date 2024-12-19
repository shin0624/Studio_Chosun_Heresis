using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TeleportSceneManager : MonoBehaviour
{
    public string targetSceneName = "1_2FloorScene"; //지상 씬 이름
    private bool playerInRange = false;//플레이어가 범위 내에 있는지 여부
    private float holdTime = 0f;// E 버튼을 누르고 있는 시간
    public float requireHoldTime = 3F;//버튼을 눌러야 하는 최소 시간

    //플레이어가 문 근처에 오면 상호작용 버튼을 3초간 클릭하라는 문구와 함께 눌린 시간이 게이지로 표시됨.
    public Text holdMessage; //지시 메시지
    public Slider holdProgressBar;// 버튼 눌린 시간에 따라 올라가는 게이지

    void Start()
    {
        //시작 시 ui요소 비활성화.
        holdMessage.gameObject.SetActive(false);
        holdProgressBar.gameObject.SetActive(false);
    }



    void Update()
    {
        try
        {
            if(playerInRange)
            {
                holdMessage.gameObject.SetActive(true);
                holdProgressBar.gameObject.SetActive(true);

                if(Input.GetKey(KeyCode.E))
                {
                    holdTime += Time.deltaTime; //플레이어가 범위 내에 있을 때 상호작용 버튼이 눌리면 holdtime이 증가함.
                    holdProgressBar.value = holdTime / requireHoldTime;

                    if(holdTime >= requireHoldTime)//holdtime이 requireHoldTime을 초과하면 지상 씬으로 이동.
                    {
                        SceneManager.LoadScene(targetSceneName);
                        Debug.Log("Open the Door");
                    }
                }
                else
                {
                    holdTime = 0f;//상호작용 버튼이 눌리지 않으면 holdtime을 0으로 초기화.
                    holdProgressBar.value = 0f;
                }
            }
            else
            {
                holdMessage.gameObject.SetActive(false);
                holdProgressBar.gameObject.SetActive(false);
            }
        }
        catch(SystemException e)
        {
            Debug.LogError($"Error In LastDoor : {e.Message}");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("PLAYER"))
        {
            playerInRange = true;//플레이어가 문 근처에 접근하면 playerInRange가 true로 설정됨.
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("PLAYER"))
        {
            playerInRange= false;//플레이어가 문을 벗어나면 playerInRange가 false가 되고 holdtime이 초기화됨.
            holdTime = 0f;
            holdProgressBar.value = 0f;
        }
    }

}
