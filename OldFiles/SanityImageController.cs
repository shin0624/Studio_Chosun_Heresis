using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SanityImageController : MonoBehaviour
{
    // 정신력에 따라서 플레이어 화면에 색이 바뀌며 패닉 상태가 되는 연출을 위한 스크립트. Sanity Manager의 Sanity를 가져와서, 값에 따라 패널 이미지의 Color, alpha값을 조절하여 나타낸다.

    [SerializeField]
    private GameObject Panel;//ui 패널
    [SerializeField]
    private Image PanelImage;//ui패널에 부착된 이미지 컴포넌트

    [SerializeField]
    private Color SanityColor03 = new Color(0f, 0f, 0f, 0f);//기본 ui 컬러 (알파값 0)
    [SerializeField]
    private Color SanityColor02 = new Color(0f, 123 / 255f, 0f, 27 / 255f);// 정신력 2일 때 : 녹색화면
    [SerializeField]
    private Color SanityColor01 = new Color(123 / 255f, 0f, 0f, 110 / 255f);//정신력 1일 때 : 붉은색 화면
    [SerializeField]
    private Color SanityColor00 = new Color(0f, 0f, 0f, 230 / 255f);//정신력 0일 때 : 검은색 화면

    [SerializeField]
    private AudioClip SanitySound00;
    [SerializeField]
    private AudioClip SanitySound01;
    [SerializeField]
    private AudioClip SanitySound02;

    public SanityManager sanityManager;// sanityManager 클래스의 sanity변수를 참조하기 위함

    private Coroutine alphaCoroutine; //알파값 애니메이션 코루틴.  sanity가 변할 때 알파값에 진동을 주어서 더욱 박진감 넘치는 효과를 줄 목적


    void Start()
    {
        
        PanelImage = Panel.GetComponent<Image>();
        SanityColor03 = PanelImage.color; //초기 sanity 값에 따른 색상 설정. 게임 시작 시 알파값 0의 기본 화면

        if (PanelImage==null)
        {
            Debug.Log("PanelImage is null!");
        }
        else
        {
            Debug.Log(PanelImage);
        }
        if(SanitySound00==null || SanitySound01==null || SanitySound02==null)
        {
            Debug.Log($"SanitySound is NULL");
        }

        sanityManager.OnSanityChanged += UpdatePanelColor;// SanityManager의 이벤트를 구독

       // UpdatePanelColor(sanityManager.SSanity);

    }

    // sanity 값에 따라 패널의 색상을 업데이트하는 이벤트 핸들러
    private void UpdatePanelColor(int sanity)
    {
       // Debug.Log("Sanity Changed : " + sanity);
       if (alphaCoroutine!=null)
        {
            StopAllCoroutines();//이전 코루틴이 있다면 중단
        }

        switch (sanity)
        {   
            case 3:
                PanelImage.color = SanityColor03;//알파값 0, 기본 화면
                AudioManager.Instance.StopSound(SanitySound02);
                break;
            case 2:
                PanelImage.color = SanityColor02;// 정신력 2칸, 알파값 0.3, 녹색
                //alphaCoroutine = StartCoroutine(AnimateAlpha(SanityColor02.a, 0.7f));
                alphaCoroutine = StartCoroutine(PulseAlpha(0.3f, 0.6f, 0.5f)); // 알파값이 0.3에서 0.6 사이로 진동
                AudioManager.Instance.StopSound(SanitySound01);
                AudioManager.Instance.LoopSound(SanitySound02);
                break;
            case 1:
                PanelImage.color = SanityColor01; // 정신력 1칸, 알파값 0.7, 붉은색
                //alphaCoroutine = StartCoroutine(AnimateAlpha(SanityColor01.a, 0.7f));
                alphaCoroutine = StartCoroutine(PulseAlpha(0.5f, 0.8f, 0.5f)); // 알파값이 0.5에서 0.8 사이로 진동
                AudioManager.Instance.StopSound(SanitySound00);
                AudioManager.Instance.StopSound(SanitySound02);
                AudioManager.Instance.LoopSound(SanitySound01);
                break;
            case 0:
                PanelImage.color= SanityColor00;//정신력 0칸, 알파값 1, 검은색
                //alphaCoroutine = StartCoroutine(AnimateAlpha(SanityColor00.a, 1.0f));
                alphaCoroutine = StartCoroutine(PulseAlpha(0.7f, 1.0f, 0.5f)); // 알파값이 0.7에서 1.0 사이로 진동
                AudioManager.Instance.StopSound(SanitySound01);
                AudioManager.Instance.PlaySound(SanitySound00);
                break;
            default:
                Debug.Log("Unexpected Sanity value " + sanity);
                break;
        }
    }

    private IEnumerator AnimateAlpha(float targetAlpha, float duration)//알파값을 변화시키는 코루틴 추가
    {
        Color currentColor = PanelImage.color; //현재 패널의 컬러
        float startAlpha = currentColor.a;//시작 알파값
        float elapsedTime = 0f;

        // 알파값이 targetAlpha로 변할 때까지 시간을 기준으로 변화
        while(elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime/duration);//알파값 선형보간
            PanelImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);//알파값 적용
            yield return null;// 한프레임 대기
        }
        PanelImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, targetAlpha);//마지막값
    }

    private IEnumerator PulseAlpha(float minAlpha, float maxAlpha, float duration)//알파값 진동 코루틴 추가
    {
        // 목표 알파값을 반복적으로 변경하는 코루틴. 특정 sanity값일 때 알파값이 왔다갔다하는 진동효과 가능
        while(true)
        {
            //알파값을 min에서 max로 증가
            yield return StartCoroutine(AnimateAlpha(maxAlpha, duration));

            //알파값을 max에서 min으로 감소
            yield return StartCoroutine(AnimateAlpha(minAlpha, duration));
        }
    }




    private void OnDestroy()// 메모리 누수 방지를 위해 오브젝트가 파괴될 때 이벤트 구독 해제
    {
        sanityManager.OnSanityChanged -= UpdatePanelColor;
    }




}
