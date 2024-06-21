using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
   
    public List<Item> items = new List<Item>();
   

    public void AddItem(Item item)
    {
            Debug.Log("Item get!");
            items.Add(item);
       
    }

    public bool HasItem(string itemName)//열쇠 아이템 습득 유무 체크 메서드
    {
        foreach(Item item in items)
        {
            if(item.itemName==itemName)//아이템 이름을 받아 미리 설정한 이름과 동일하면 true를 반환
            {
                return true;
            }
        }
        return false;
    }
}
