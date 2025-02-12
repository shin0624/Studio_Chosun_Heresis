using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomMessTrigger : MonoBehaviour
{
    // �θ� ������Ʈ (�ڽĵ��� ã�� �θ�)
    public GameObject parentObject;

    // ȿ���� �ҽ�
    public AudioSource audioSource; // ȿ������ ����� ����� �ҽ�
    public AudioClip messSound;     // �������� �� ����� ȿ����

    private bool isTriggered = false;

    // �÷��̾ �濡 ������ �߻��ϴ� �Լ�
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PLAYER") && !isTriggered) // �÷��̾� �±� Ȯ��
        {
            isTriggered = true;
            PlayMessSound(); // ȿ���� ���
            MakeRoomMessy(); // ���� ���������� (�� ����)
        }
    }

    // ���ǵ��� ���������� �ϴ� �Լ� (�� ����)
    private void MakeRoomMessy()
    {
        // �θ� ������Ʈ���� Rigidbody�� �ִ� ��� �ڽ� ������Ʈ�� ã��
        Rigidbody[] messObjects = parentObject.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rb in messObjects)
        {
            if (rb != null)
            {
                // ������ ���ư��ų� �������Բ� ���� ����
                Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(0.5f, 1.5f), Random.Range(-1f, 1f));
                rb.AddForce(randomDirection * Random.Range(300, 500)); // ������ �������� �� ����
                rb.AddTorque(Random.insideUnitSphere * Random.Range(50, 150)); // ������ ȸ���� ����
            }
        }
    }

    // ȿ���� ���
    private void PlayMessSound()
    {
        if (audioSource != null && messSound != null)
        {
            audioSource.PlayOneShot(messSound); // �� �� ȿ���� ���
        }
    }
}
