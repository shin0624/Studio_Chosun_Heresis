using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class FadeInOutController : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Color startColor = Color.white;
    [SerializeField] private Color endColor = Color.black;
    [SerializeField] private float fadeDuration = 3f;
    [SerializeField] private float delay = 2f;

    private void Start()
    {
        image = GetComponent<Image>();
        image.color = startColor;

        StartCoroutine(FadeOutCoroutine());
    }

    IEnumerator FadeOutCoroutine() //페이드아웃 코루틴
    {
        
        yield return new WaitForSeconds(delay); // 영상 시작 후 잠시 딜레이를 준다.

        float timer = 0f;

        while (timer < fadeDuration) // 설정한 페이드아웃 시간동안 진행
        {
            timer += Time.deltaTime;
            float progress = timer / fadeDuration;
            image.color = Color.Lerp(startColor, endColor, progress);//설정한 페이드아웃 시간동안 white ~ black으로 서서히 변경됨
            yield return null; 
        }
        image.color = endColor;
    }
}
