using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    GameObject _player = null;

    // 카메라와 플레이어 사이의 초기 오프셋 (수정 필요)
    Vector3 offset = new Vector3(0.0f, 1.5f, -3.0f);

    void LateUpdate()
    {
        if (_player != null)
        {
            // 플레이어의 위치에 오프셋을 더해 카메라의 위치를 설정
            transform.position = _player.transform.position + offset;

            // 카메라가 플레이어를 바라보도록 회전
            transform.LookAt(_player.transform);
        }
    }
}