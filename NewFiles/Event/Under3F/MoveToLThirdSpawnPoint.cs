using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToLThirdSpawnPoint : MonoBehaviour
{
    [SerializeField] BoxCollider boxCollider;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        boxCollider.isTrigger = true;//트리거체크
        if (boxCollider == null || !boxCollider.isTrigger)
        {
            Debug.Log("BoxCollider is Error! Please Check Collider`s Trigger or Component");
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") || other.CompareTag("PLAYER") )
        {
            LoadingSceneManager.LoadScene("NewUnderAfter");
        }
    }
}
