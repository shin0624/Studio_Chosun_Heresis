using System.Collections;
using System.Collections.Generic;
using UnityEngine.Playables;
using UnityEngine;
using System.Security.Cryptography;

public class Under_TimelineController : MonoBehaviour
{
    public static Under_TimelineController Under_TimelineController_Instance {  get; private set; }

    //지하 스폰 시 사용할 타임라인 재생 스크립트.
    public PlayableDirector timeline;


    private void Awake()
    {
      if(Under_TimelineController_Instance==null)
        {
           Under_TimelineController_Instance = GetComponent<Under_TimelineController>();
           DontDestroyOnLoad(gameObject);
        }
       else
       {
           Destroy(gameObject);
       }

    }

    public void PlayTimeline()
    {
        if (timeline != null)
        {
            timeline.Play();
        }
        else
        {
            Debug.LogError("Timeline is not assigned");
        }
    }
    
}
