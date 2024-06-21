using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//지하 씬에서 사용될 에너미 사운드 제어 스크립트. 에너미와 플레이어 간 거리가 가까워지면 음성이 재생된다.

public class EnemySoundScript : MonoBehaviour
{
    public AudioSource ado;
    public GameObject player;
    public float detectionRange = 2f;//최대 감지거리
    public float fieldOfViewAngle = 120f;//에너미 시야각


    private bool IsAroundPlayer;
    

    void Start()
    {
        IsAroundPlayer = false;
        
    }

   
    void Update()
    {
        CheckPlayerDistance();
    }
    
    private void CheckPlayerDistance()
    {
        float dis = Vector3.Distance(transform.position, player.transform.position);

        if(dis <=detectionRange)
        {
            Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

           if(angleToPlayer <= fieldOfViewAngle /2)//플레이어를 향한 각도가 시야각 이하일때
            {
                if (!IsAroundPlayer)
                {
                    Debug.Log("sound playing");
                    IsAroundPlayer = true;
                    if (!ado.isPlaying)
                    {
                        ado.Play();
                    }

                }
            }
            else
            {
                if (IsAroundPlayer)
                {
                    Debug.Log("sound not playing(angle)");
                    IsAroundPlayer = false;
                    if (ado.isPlaying)
                    {
                        ado.Stop();
                    }

                }
            }

        }
        else
        {
            if(IsAroundPlayer)
            {
                Debug.Log("sound not playing(distance)");
                IsAroundPlayer = false;
                if (ado.isPlaying)
                {
                    ado.Stop();
                }
            }
        }
      
    }

}
