using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndergroundEscapeController : MonoBehaviour
{
    // 지하주차장 -> 지상 1층으로 탈출을 위한 스크립트

    private BoxCollider boxCollier;//박스 콜라이더의 트리거를 받아야 하므로.

    void Start()
    {
        boxCollier = GetComponent<BoxCollider>();
        boxCollier.isTrigger = true;//트리거체크
        if(boxCollier==null || !boxCollier.isTrigger)
        {
            Debug.Log("BoxCollider is Error! Please Check Collider`s Trigger or Component");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("PLAYER"))
        {
            LoadingSceneManager.LoadScene("New1_2FloorScene");
        }
    }
}
