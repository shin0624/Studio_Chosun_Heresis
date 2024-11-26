using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define 
{
    // 캐릭터 애니메이션 등 처리를 위한 열거체 클래스

    public enum EnemyState
    {
        IDLE,
        WALKING,
        RUNNING,
        ATTACK,
    }
    public enum DoctorState
    {
        IDLE,
        WALKING,
        RUNNING,
        ATTACK,
    }
    public enum ShamanState
    {
        IDLE,
        ROAR,
        CHOKE,
        RUNNING,
        ATTACK,
    }
}
