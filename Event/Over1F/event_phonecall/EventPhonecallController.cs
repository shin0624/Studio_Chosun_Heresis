using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventPhonecallController : MonoBehaviour
{
    // EventPhonecall를 참조
    public EventPhonecall eventPhonecall;

    void Start()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PLAYER"))// 플레이어 태그가 붙은 게임오브젝트와 충돌 시 
        {
            eventPhonecall.PlayPhonecall();
        }
    }
}
