using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempTrigger : MonoBehaviour
{
    public static bool isEnter = false;
    public DoctorController dc;
    private void Start() {
        dc = GameObject.Find("Doctor").GetComponent<DoctorController>();
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PLAYER") || other.CompareTag("Player"))
        {
            isEnter = true;
            dc.AwakeDoctor();

        }
    }
    }
