using UnityEngine;

public class InteractMedicineBottle : Interactable
{
    // SanityManager를 참조
    public SanityManager sanityManager;
    
    // 약 픽업 사운드
    public AudioClip pickupItemMedicine;


    public override void Interact()
    {
        if (sanityManager != null)
        {
            sanityManager.AddMedicine(); // AddMedicine 호출
        }

        AudioManager.Instance.PlaySound(pickupItemMedicine); // AudioManager를 통해 사운드 재생

        Destroy(gameObject); // 오브젝트 파괴
    }
}
