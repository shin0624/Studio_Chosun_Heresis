using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class JailDoorEventController : MonoBehaviour
{
    // 지하 2층 감옥문 이벤트 컨트롤러 : 플레이어가 지하 2층에 도착하면 불이 꺼지고 모든 감옥문이 열리는 이상현상이 발생한다.
    private GameObject player;
    private JailNeonLampsSound[] JNLSList;//스크립트가 제어해야 하는 램프는 여러개이므로, 배열타입으로 선언
    private JailDoorOpen[] JDOPENList;//스크립트가 제어해야 하는 도어는 여러개 이므로, 배열 타입으로 선언

    private AudioSource Under2ndBGM;
    public bool isEnter = false;


    void Start()
    {
        //JNLS = FindObjectOfType<JailNeonLampsSound>();//스크립트가 붙어있는 네온램프를 찾는다.
        //-->실수 : 위와 같이 선언하면, 스크립트에서 첫 번째로 발견된 JNLS오브젝트만 제어하게 되어, 게임 시작 시 첫번째 네온램프만 깜빡거리게 됨.
        //이 문제를 해결하기 위해, 네온 램프를 모두 찾아 각각의 플리커링 메서드를 호출하도록 해야 함.

        JNLSList = FindObjectsOfType<JailNeonLampsSound>();//스크립트가 붙은 모든 네온램프를 찾는다.
        JDOPENList = FindObjectsOfType<JailDoorOpen>();
        Under2ndBGM = GetComponent<AudioSource>();
        //Find Object Of Type이 아니라, Find Objects Of Type임에 주의
    }

    
    void Update()
    {
  
    }

    public void OnTriggerEnter(Collider other)
    {
        Under2ndBGM.Play();
       if(other.CompareTag("PLAYER"))//플레이어 태그가 붙은 게임오브젝트와 충돌 시 
        {
            isEnter = true;
           
            foreach (JailNeonLampsSound JNLS in JNLSList)// 리스트 인자로 들어온 모든 램프들을 끝까지 순환하는 foreach 사용
            {
                JNLS.StartFlickering(0.2f);
            }   
            foreach(JailDoorOpen JDOPEN in JDOPENList)
            {
                JDOPEN.StartOpenDoorBust();       
            }
        }
    }

    // public void OnTriggerExit(Collider other)
    // {
    //     if(other.CompareTag("PLAYER"))
    //     {
    //     Under2ndBGM.Stop();
    //     isEnter = false;
           
    //         foreach (JailNeonLampsSound JNLS in JNLSList)// 리스트 인자로 들어온 모든 램프들을 끝까지 순환하는 foreach 사용
    //         {
    //             JNLS.StopFlickering();
    //         }   
    //     }

    // }
}
