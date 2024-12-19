using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public enum EnemyState//에너미 오브젝트 애니메이션 처리를 위한 열거체
    { 
        IDLE,
        WALKING,
        RUNNING,
        LOOK_AROUND,
        PRAYING,
        PRAYING_STANDUP,
        ATTACK,
        SEARCHING,
    
    }

}
