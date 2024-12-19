using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempTrigger : MonoBehaviour
{
    public bool isEnter = false;
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PLAYER"))
        {
            isEnter = true;
            Debug.Log("Player Enter");
        }
    }
    }
