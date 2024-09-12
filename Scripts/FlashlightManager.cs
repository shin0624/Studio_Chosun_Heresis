using UnityEngine;
using TMPro;

public class FlashlightManager : MonoBehaviour
{
    public TextMeshProUGUI batteryText;  // 배터리 상태를 표시할 UI 텍스트
    public float batteryPercentage = 100f;  // 초기 배터리 용량
    public float drainInterval = 0.01f;  // 배터리 소모 간격 (초)
    public float drainAmount = 0.00556f;  // 배터리 소모량 // 0.00556 3분
    private bool isFlashlightOn = false;  // 손전등 상태 변수

    private float timeSinceLastDrain;

    void Update()
    {
        if (isFlashlightOn)
        {
            timeSinceLastDrain += Time.deltaTime;

            if (timeSinceLastDrain >= drainInterval)
            {
                timeSinceLastDrain -= drainInterval;  // 시간 누적값 초기화
                DrainBattery();
                UpdateBatteryText();
            }
        }
    }

    public void SetFlashlightState(bool isOn)
    {
        isFlashlightOn = isOn;
    }

    void DrainBattery()
    {
        if (batteryPercentage > 0)
        {
            batteryPercentage -= drainAmount;
            if (batteryPercentage < 0) batteryPercentage = 0;
        }
    }

    void UpdateBatteryText()
    {
        // 배터리 상태를 소수점 없이 표시
        batteryText.text = Mathf.FloorToInt(batteryPercentage).ToString() + "%";
    }
}
