using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShamanRoomBuddaHeadEvent : MonoBehaviour
{
    //지하 무당집 이벤트 : 플레이어가 계단을 내려오면 부처 머리가 굴러옴
    [SerializeField] private GameObject BuddaHead; // 부처 머리 오브젝트
    [SerializeField] private AudioSource RollingSound; //굴러오는 소리
    private Rigidbody BuddaHeaRB;//부처 머리의 리지드바디
    private float Rbforce = 1.50f;// 힘 가하는 정도
    private bool HasRolled = false;//이벤트가 한번만 발생하도록 제어하는 트리거
    
    void Start()
    {
        BuddaHeaRB = BuddaHead.GetComponent<Rigidbody>();
        RollingSound = BuddaHead.GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (HasRolled) return;//이미 이벤트가 발생했다면 중복실행되지 않음

        if(other.CompareTag("Player"))
        {
            BuddaHeaRB.AddForce(Vector3.right * Rbforce, ForceMode.Impulse);//x방향으로 Impulse모드 힘을 가한다. 캡슐 컬라이더가 부착된 부처머리 오브젝트는 데굴데굴 구르는 것처럼 보일 것.
            RollingSound.Play();
            RollingSound.loop = false;
            HasRolled= true;
        }
    }
}
