using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class FadeOutController : MonoBehaviour
{
    //2층 기도실 애니메이션에 사용될 페이드아웃 스크립트. image의 알파값을 감소시키며 페이드아웃 효과를 준다.
    private Image image;
    private void Awake()
    {
       image = GetComponent<Image>();
    }

    private void Update()
    {
        Color color = image.color;

        if(color.a < 1)//알파값이 1 이하이면 알파값 ~255까지 증가. 검은 화면으로 서서이 변하는 페이드아웃 연출
        {
            color.a+=Time.deltaTime;
        }
     
        image.color = color;
    }
}
