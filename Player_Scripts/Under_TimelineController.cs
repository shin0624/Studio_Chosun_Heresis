using System.Collections;
using System.Collections.Generic;
using UnityEngine.Playables;
using UnityEngine;
using System.Security.Cryptography;

public class Under_TimelineController : MonoBehaviour
{
    //지하 스폰 시 사용할 타임라인 재생 스크립트.
    public PlayableDirector timeline;

    
    void Start()
    {
        if(timeline!=null)
        {
            timeline.Play();
        }
        else
        {
            Debug.LogError("Timeline is not assigned");
        }
    }
}
