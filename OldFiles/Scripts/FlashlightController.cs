using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    public GameObject flashlight;  // 손전등 오브젝트
    private bool isFlashlightOn = false;  // 손전등이 켜져 있는지 여부

    private Transform playerTransform; // 플레이어의 Transform을 저장할 변수

    void Start()
    {
        if (flashlight != null)
        {
            flashlight.SetActive(isFlashlightOn);  // 초기 상태로 손전등 비활성화
            playerTransform = transform.parent;    // 손전등의 부모의 Transform을 저장
        }
    }

    void Update()
    {
        if (flashlight != null && playerTransform != null)
        {
            // 플레이어 위치와 회전에 따라 손전등 위치 및 회전 업데이트
            flashlight.transform.position = playerTransform.position;
            flashlight.transform.rotation = playerTransform.rotation;
        }

        ToggleFlashlight();
    }

    void ToggleFlashlight()
    {
        if (Input.GetKeyDown(KeyCode.F))  // F키를 누르면 손전등 켜기/끄기
        {
            isFlashlightOn = !isFlashlightOn;
            flashlight.SetActive(isFlashlightOn);  // 손전등 활성화/비활성화
        }
    }
}
