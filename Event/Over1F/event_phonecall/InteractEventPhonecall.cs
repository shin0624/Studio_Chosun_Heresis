using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class InteractEventPhonecall : Interactable
{
    private AudioSource audioSource;

    // 전화 픽업 후 재생 사운드
    public AudioClip afterPickupPhone;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public override void Interact()
    {
        // 기존에 재생 중인 오디오를 멈춤
        audioSource.Stop();

        // 새로운 클립을 설정하고 재생
        audioSource.loop = false;
        audioSource.clip = afterPickupPhone;
        audioSource.Play();

        // 오브젝트의 태그를 'Untagged'로 변경
        gameObject.tag = "Untagged";
    }
}
