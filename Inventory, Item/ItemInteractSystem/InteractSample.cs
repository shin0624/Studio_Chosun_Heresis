using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 파일명 규칙 Interact어쩌고.cs
// E 키로 상호작용할 스크립트는 위와같이 맞추기

// 예시
// public class InteractSample : Interactable의 InteractSample 부분 상호작용 가능 오브젝트의 스크립트 이름으로 설정
// ex) public class InteractMedicineBottle : interactable{ //코드 }
public class InteractSample : Interactable
{
    public override void Interact()
    {
        // E 키로 상호작용 시 실행할 이벤트 코드 작성
        // 기존에 사용하던 코드 그대로 복붙해도 될듯 ?

        // 모든 E 키로 상호작용 가능 오브젝트 태그 Interactable로 설정
        // 태그의 add tag...에서 tags의 + 버튼눌러서 태그이름 Interactable 추가하기
    }
}
