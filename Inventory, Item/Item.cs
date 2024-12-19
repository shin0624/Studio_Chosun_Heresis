using UnityEngine;

public class Item : MonoBehaviour
{
    public string itemName;
    public string dialogueID;//아이템 습득 시 표시할 다이얼로그 아이디.
    public AudioSource ado;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("PLAYER"))
        {
            Inventory inventory = other.GetComponent<Inventory>();
            if (inventory != null && !ado.isPlaying)
            {
                ado.Play();
                Debug.Log("Item in Inventory!");
                inventory.AddItem(this);
                gameObject.SetActive(false);

                DialogueDisplayManager dm = FindAnyObjectByType<DialogueDisplayManager>();
                if (dm != null && !string.IsNullOrEmpty(dialogueID))
                {
                    dm.DisplayDialogueOnce(dialogueID);
                }


            }
        }
    }
}
