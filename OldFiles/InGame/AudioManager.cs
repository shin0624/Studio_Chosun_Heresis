/*
 * AudioManager.cs
 * 이 스크립트는 게임 내에서 플레이어 오디오를 관리
 * 두 가지 유형의 AudioSource를 통해 단발성 및 루프 재생 사운드를 처리
 */
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // AudioSource 컴포넌트
    private AudioSource loopAudioSource;
    private AudioSource oneShotAudioSource;

    void Start()
    {
        // AudioSource 컴포넌트를 가져오기
        AudioSource[] audioSources = GetComponents<AudioSource>();
        if (audioSources.Length >= 2)
        {
            // 첫 번째 AudioSource는 루프 재생용으로 설정
            loopAudioSource = audioSources[0];
            // 두 번째 AudioSource는 단발성 재생용으로 설정
            oneShotAudioSource = audioSources[1];
        }
    }

    // 단발성 사운드를 재생하는 메서드
    public void PlaySound(AudioClip clip)
    {
        if (oneShotAudioSource != null && clip != null)
        {
            oneShotAudioSource.PlayOneShot(clip);
        }
    }

    // 사운드를 루프로 재생하는 메서드
    public void PlaySoundLoop(AudioClip clip)
    {
        if (loopAudioSource != null && clip != null)
        {
            loopAudioSource.clip = clip;
            loopAudioSource.loop = true;
            loopAudioSource.Play();
        }
    }

    // 현재 재생 중인 루프 사운드를 정지하는 메서드
    public void StopSoundLoop()
    {
        if (loopAudioSource != null)
        {
            loopAudioSource.Stop();
            loopAudioSource.loop = false;
        }
    }
}
