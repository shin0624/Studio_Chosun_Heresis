using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventPhonecall : MonoBehaviour
{
    private AudioSource audioSource;

    // 전화벨 사운드
    public AudioClip phonecall;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = phonecall;
    }

    public void PlayPhonecall()
    {
        // 오브젝트의 태그를 'Interactable'로 변경
        gameObject.tag = "Interactable";

        audioSource.Play();
    }
}
