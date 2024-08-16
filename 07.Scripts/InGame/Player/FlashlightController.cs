using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    // 손전등
    public Light flashlight;

    // 손전등의 초기 상태 (켜짐/꺼짐)
    private bool isOn = true;

    // 손전등 사운드
    [Header("Sounds")]
    public AudioClip flashlightOn;
    public AudioClip flashlightOff;

    void Start()
    {
        // 초기 상태 설정
        flashlight.enabled = isOn;
    }

    void Update()
    {
        // F 키를 누르면 손전등의 상태를 변경합니다.
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!isOn)
            {
                AudioManager.Instance.PlaySound(flashlightOn); // AudioManager를 통해 사운드 재생
            }
            else
            {
                AudioManager.Instance.PlaySound(flashlightOff); // AudioManager를 통해 사운드 재생
            }
            isOn = !isOn;  // 손전등 상태 토글
            flashlight.enabled = isOn;
        }
    }
}
