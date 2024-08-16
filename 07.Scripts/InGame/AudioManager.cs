using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    // AudioSource 컴포넌트
    private AudioSource loopAudioSource;
    private AudioSource oneShotAudioSource;

    void Awake()
    {
        // 싱글톤 패턴
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시에도 오디오 매니저 유지
        }
    }

    void Start()
    {
        // AudioSource 컴포넌트를 가져오기
        AudioSource[] audioSources = GetComponents<AudioSource>();
        if (audioSources.Length >= 2)
        {
            loopAudioSource = audioSources[0];
            oneShotAudioSource = audioSources[1];
        }
    }

    // 사운드를 재생하는 메서드
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

    // 루프 사운드를 정지하는 메서드
    public void StopSoundLoop()
    {
        if (loopAudioSource != null)
        {
            loopAudioSource.Stop();
            loopAudioSource.loop = false;
        }
    }
}
