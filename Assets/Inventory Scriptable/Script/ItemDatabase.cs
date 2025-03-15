using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/Item Database")]
public class ItemDatabase : ScriptableObject
{
    public List<ItemObject> items;

    public ItemObject GetItemById(int id)
    {
        return items.Find(item => item.Id == id);
    }
}
