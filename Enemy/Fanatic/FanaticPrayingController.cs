using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Analytics;
/*
지하 3층 무당집 광신도 제어용 스크립트
처음 플레이어 스폰 시에 기도하는 에너미를 보이고, 플레이어가 일정 간격 이상 떨어지면 메모리 낭비를 막기 위해 전부 비활성화
대표 에너미 하나에만 스크립트를 달아놓고, 리스트로 관리. 
기도소리 클립도 하나에만 달아놓음.
*/

public class FanaticPrayingController : MonoBehaviour 
{

    [SerializeField] private AudioSource ado;// 기도소리 클립
    [SerializeField] private List<GameObject> Fanatics;//광신도 묶음
    [SerializeField] private Transform Player;//플레이어 트랜스폼
    private float distance;//플레이어와의 거리
    [SerializeField] private float Elimination = 10.0f;//광신도 비활성화 기준 거리
    [SerializeField] private AudioManager audioManager;//오디오 매니저
    
    void Start()
    {
        if (Player == null)// 플레이어 객체 찾기
        {
            Player = GameObject.FindGameObjectWithTag("PLAYER").transform;
            if (Player == null)
            {
                Debug.LogError("Player not found. Make sure the Player has the 'Player' tag.");
                return;
            }
        }
        distance = 0.0f;
        PlayPrayingSound();
    }

    void FixedUpdate()
    {
        distance = DistanceCheck();//플레이어와의 거리 측정
        
        if(distance >=Elimination)//거리 조건에 따라 광신도 비활성화
        {
            StopPraying();
        }
    }

    private void StopPraying()// 플레이어가 일정 거리 이상 멀어지면 비활성화 시킴.
    {
        if (Player != null)
        {
            foreach(GameObject enemy in Fanatics)
            {
                if(enemy!=null && enemy.activeSelf)//이미 비활성화된 경우는 제외한다.
                {
                    enemy.SetActive(false);
                }
            }
            StopPrayingSound();// 기도 소리 정지
        }
    }

    private float DistanceCheck()// 플레이어와 광신도 간 거리 계산
    { 
        return Vector3.Distance(transform.position, Player.position);// 기도하는 Fanatic 위치 - 플레이어 위치 
    }

    private void PlayPrayingSound()
    {
        if(ado!=null)
        {
            ado.Play();
            ado.loop = true;
        }

    }

    private void StopPrayingSound()
    {
        if(ado!=null)
        {
            ado.Stop();
            ado.loop = false;
        }
    }
}
