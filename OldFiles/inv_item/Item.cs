using UnityEngine;

public class Item : MonoBehaviour
{
    public string itemName;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Inventory inventory = other.GetComponent<Inventory>();
            if (inventory != null)
            {
                inventory.AddItem(this);
                gameObject.SetActive(false);
            }
        }
    }
}
