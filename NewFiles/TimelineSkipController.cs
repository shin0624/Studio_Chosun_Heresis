using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineSkipController : MonoBehaviour
{
    [SerializeField]
    private PlayableDirector timeline;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SkipCutscene();
        }
    }
    void SkipCutscene()
    {
        timeline.Stop();
    }
}
