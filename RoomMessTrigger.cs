using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomMessTrigger : MonoBehaviour
{
    // 부모 오브젝트 (자식들을 찾을 부모)
    public GameObject parentObject;

    // 효과음 소스
    public AudioSource audioSource; // 효과음을 재생할 오디오 소스
    public AudioClip messSound;     // 어질러질 때 재생할 효과음

    private bool isTriggered = false;

    // 플레이어가 방에 들어오면 발생하는 함수
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PLAYER") && !isTriggered) // 플레이어 태그 확인
        {
            isTriggered = true;
            PlayMessSound(); // 효과음 재생
            MakeRoomMessy(); // 물건 어질러지기 (한 번에)
        }
    }

    // 물건들을 어질러지게 하는 함수 (한 번에)
    private void MakeRoomMessy()
    {
        // 부모 오브젝트에서 Rigidbody가 있는 모든 자식 오브젝트를 찾음
        Rigidbody[] messObjects = parentObject.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rb in messObjects)
        {
            if (rb != null)
            {
                // 물건이 날아가거나 굴러가게끔 힘을 가함
                Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(0.5f, 1.5f), Random.Range(-1f, 1f));
                rb.AddForce(randomDirection * Random.Range(300, 500)); // 랜덤한 방향으로 힘 적용
                rb.AddTorque(Random.insideUnitSphere * Random.Range(50, 150)); // 랜덤한 회전력 적용
            }
        }
    }

    // 효과음 재생
    private void PlayMessSound()
    {
        if (audioSource != null && messSound != null)
        {
            audioSource.PlayOneShot(messSound); // 한 번 효과음 재생
        }
    }
}
