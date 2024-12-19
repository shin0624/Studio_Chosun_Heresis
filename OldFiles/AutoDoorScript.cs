using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDoorScript : MonoBehaviour
{
    bool DoorOpen;
    public GameObject door;
    public Vector3 closedPosition;
    public Vector3 openPosition;
    public float speed = 2f;
    
    void Start()
    {
        DoorOpen = false;
        closedPosition = door.transform.position;
        openPosition = closedPosition + new Vector3(0, 0, -3f);//x축으로 3f만큼 슬라이딩
    }

    
    void Update()
    {
        if(DoorOpen ==true)
        {
            door.transform.position = Vector3.Lerp(door.transform.position, openPosition, Time.deltaTime * speed);
        }
        if(DoorOpen==false)
        {
            door.transform.position = Vector3.Lerp(door.transform.position, closedPosition, Time.deltaTime * speed);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        DoorOpen = true;
    }
    private void OnTriggerExit(Collider other)
    {
        DoorOpen = false;
    }
}
