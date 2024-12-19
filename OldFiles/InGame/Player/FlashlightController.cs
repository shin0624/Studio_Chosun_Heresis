/*
 * FlashlightController.cs
 * 이 스크립트는 손전등의 상태를 제어하며,
 * 플레이어가 손전등을 켜고 끌 때 사운드를 재생
 * F 키를 눌러 손전등의 상태를 토글
 */
using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    [Header("Scripts")]
    public AudioManager audioManager;

    [Header("Flashlight")]
    public Light flashlight;    // 손전등의 Light 컴포넌트

    private bool isOn = true;   // 손전등의 초기 상태 (켜짐/꺼짐)

    // 손전등 사운드
    [Header("Sounds")]
    public AudioClip flashlightOn;  // 손전등 켜짐 사운드 클립
    public AudioClip flashlightOff; // 손전등 꺼짐 사운드 클립

    void Start()
    {
        // 손전등의 초기 상태를 설정
        flashlight.enabled = isOn;
    }

    void Update()
    {
        // F 키를 눌렀을 때 손전등 상태를 변경
        if (Input.GetKeyDown(KeyCode.F))
        {
            // 손전등 상태에 따라 적절한 사운드를 재생
            if (!isOn)
            {
                audioManager.PlaySound(flashlightOn);   // 손전등 켜짐 사운드 재생
            }
            else
            {
                audioManager.PlaySound(flashlightOff);  // 손전등 꺼짐 사운드 재생
            }

            // 손전등 상태를 토글
            isOn = !isOn;
            flashlight.enabled = isOn;
        }
    }
}
