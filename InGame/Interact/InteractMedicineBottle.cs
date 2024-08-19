/*
 * InteractMedicineBottle.cs
 * 이 스크립트는 약병 오브젝트와 상호작용할 때 발생하는 로직을 구현
 * 플레이어가 약병을 상호작용할 때 약이 추가되고, 사운드가 재생되며, 약병 오브젝트는 파괴
 */
using UnityEngine;

public class InteractMedicineBottle : Interactable
{
    [Header("Scripts")]
    public AudioManager audioManager;
    public SanityManager sanityManager;

    [Header("Sounds")]
    public AudioClip pickupItemMedicine; // 약 픽업 사운드

    // 상호작용이 발생했을 때 호출되는 메서드
    // 플레이어가 약병과 상호작용할 때 실행
    public override void Interact()
    {
        if (sanityManager != null)
        {
            // SanityManager의 AddMedicine 메서드를 호출하여 약을 추가
            sanityManager.AddMedicine();
        }

        // AudioManager를 사용해 약 픽업 사운드 재생
        audioManager.PlaySound(pickupItemMedicine);

        // 상호작용 후 오브젝트 파괴
        Destroy(gameObject);
    }
}
