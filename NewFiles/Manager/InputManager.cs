using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    //플레이어의 입력을 감지하고 특정 키가 눌렸을 때 이벤트를 발생시키는 매니저
    // 플레이어 입력을 update에서 받으면 불필요한 반복 연산이 수행되므로, 이를 방지하고 효율적인 이벤트 기반 처리를 위함

    public static event Action OnInteractKeyPressed;// Action 델리게이트 타입으로 정의. E 키가 눌렸을 때 발생하며, 다른 스크립트에서 이 이벤트를 구독하고 특정 행동 실행 가능

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            
            OnInteractKeyPressed?.Invoke();//E키 입력을 감지하여 이벤트 발생. 구독자가 있는 경우에만 이벤트 호출
        }
    }
}
