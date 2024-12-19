using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInController : MonoBehaviour
{
    //지하층 예배실 애니메이션에 사용될 페이드아웃 스크립트. image의 알파값을 증가시키며 페이드인 효과를 준다.
    private Image image;
    private void Awake()
    {
        image = GetComponent<Image>();
        Color color = image.color;
        color.a = 1f;
        image.color = color;
    }

    private void Update()
    {
        Color color = image.color;

        if (color.a > 0)//알파값이 0 이상이면 알파값 0까지 감소. 흰 화면으로 서서이 변하는 페이드인 연출
        {
            color.a -= Time.deltaTime / 2f;//페이드 인 속도 조절
            image.color = color;
        }

        
    }
}
