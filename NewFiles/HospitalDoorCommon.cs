using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HospitalDoorCommon : MonoBehaviour
{
    //움직임을 주고자 하는 물체, 어느정도 움직일 것인지를 선언
    [SerializeField] private GameObject obj;
    [SerializeField] public float x, y, z;//벡터값 마다 연산할 수
    private Vector3 OpenPosition;//연산 후 위치
    private Vector3 ClosePosition;//현재 위치
    private float Speed = 0.1f;
    private bool Open = false;

    void Start()
    {
        obj = gameObject.GetComponent<GameObject>();
        ClosePosition = obj.transform.position;
        OpenPosition = new Vector3(OpenPosition.x +x, OpenPosition.y+y, OpenPosition.z + z);// 연산 후 위치는 현재 위치 각 xyz값 - 개발자가 지정한 xyz값만큼 연산. 오브젝트 좌표축에 따라 -값, +값을 할당

    }

    private void OnTriggerEnter(Collider other)
    {
        if (obj != null && other.CompareTag("Player"))
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                Open = true;
            }
        }
    }

    void CalcPosition()
    {
        if (Open)
        {
            obj.transform.position = Vector3.Lerp(ClosePosition, OpenPosition, Time.deltaTime * Speed);
            if(Vector3.Distance(ClosePosition, OpenPosition) < 0.01) { ClosePosition = OpenPosition; Open = false; }
        }
    }

}
