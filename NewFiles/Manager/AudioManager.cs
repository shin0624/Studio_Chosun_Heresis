using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    // AudioSource 컴포넌트
    private AudioSource audioSource;

    void Awake()
    {
        // 싱글톤 패턴
        // 코드 사용 편의성을 위해
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
        audioSource = GetComponent<AudioSource>();
    }

    // 사운드를 재생하는 메서드
    // AudioManager.Instance.PlaySound(AudioClip); 으로 사용
    public void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public void StopSound(AudioClip clip)//사운드 스탑
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop(); // 현재 재생 중인 사운드 중지
            audioSource.loop = false; // 루프 해제
            audioSource.clip = null; // 클립 해제
        }
    }

    public void LoopSound(AudioClip clip)//사운드 루프. playOnShout은 단발성이라 루프 안됨
    {
        if (audioSource != null && clip != null)
        {
           if(audioSource.isPlaying && audioSource.clip==clip)
            {
                return;//이미 해당 클립이 재생 중이면 재생을 중복으로 시작하지 않도록.
            }
            audioSource.clip = clip;//오디오소스에 클립 설정.
            audioSource.loop = true;//루프 설정
            audioSource.Play();
        }
    }
}
