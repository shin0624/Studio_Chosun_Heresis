using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class JailNeonLampsSound : MonoBehaviour
{
    //지하 2층 이벤트 중 네온램프 이벤트 제어

    private GameObject NeonLamp;
    private AudioSource zizizi;
    private Light PointLight;
    private Coroutine flickerCoroutine;//반복하며 깜빡깜빡하는 효과를 내는 코루틴

    void Start()
    {
        NeonLamp = this.gameObject;//본 스크립트가 붙어있는 오브젝트를 NeonLamp로 설정
        PointLight = NeonLamp.GetComponentInChildren<Light>();//NeonLamp의 자식인 pointLight를 지정
        zizizi = NeonLamp.GetComponent<AudioSource>();//NeonLamp에 붙은 오디오소스를 찾음

        // 포인트 라이트와 오디오 소스가 제대로 설정되었는지 확인
        if (PointLight == null)
        {
            Debug.LogError("Point Light is null");
        }
        else
        {
            Debug.Log("Point Light SET");
        }

        if (zizizi == null)
        {
            Debug.LogError("AudioSource is null");
        }
        else
        {
            Debug.Log("AudioSource SET");
        }
    }

    public void SetLightIntensity(float intensity)//포인트라이트의 강도를 0 ~ 1로 조절하여 깜빡거리는 효과를 내는 메서드
    {
        if(PointLight!=null)
        {
            PointLight.intensity = Mathf.Clamp(intensity, 0.0f, 1.5f);

        }
    }

    public void PlayziziziSound()//지지직 소리 재생 메서드
    {
        if (zizizi != null)
        {
            zizizi.Play();
        }
    }
    public void StopziziziSound()//지지직 소리 종료 메서드
    {
        if (zizizi != null && zizizi.isPlaying)
        {
            zizizi.Stop();
        }
    }

    private IEnumerator FlickerLight(float interval)//포인트라이트의 강도를 0과 1로 반복하며 깜빡이는 코루틴
    {
        while(true)
        {
            SetLightIntensity(0.0f);
            PlayziziziSound();
            //Debug.Log($"{gameObject} Light is Flickered");
            yield return new WaitForSeconds(interval);
            SetLightIntensity(1.5f);
            StopziziziSound();
            yield return new WaitForSeconds(interval);
        }
    }

    public void StartFlickering(float interval)//깜빡이기 시작 
    {
        if(flickerCoroutine==null)
        {
            flickerCoroutine = StartCoroutine(FlickerLight(interval));
        }
    }

    public void StopFlickering()//깜빡이기 멈춤
    {
        if(flickerCoroutine!=null)
        {
            StopCoroutine(flickerCoroutine);
            flickerCoroutine = null;
            SetLightIntensity(0);
            StopziziziSound();

        }
    }



}
