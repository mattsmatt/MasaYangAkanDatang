using UnityEngine;

public enum ItemType
{
    Consumable,
    Equipment,
    Currency,
    KeyItem,
    Level
}

public enum Rarity
{
    Common,
    Rare,
    Legendary
}

[CreateAssetMenu(fileName = "Item", menuName = "Inventory/Item")]
public class ItemObject : ScriptableObject
{
    public int Id;
    public Sprite img;
    public ItemType type;
    [TextArea(15, 20)]
    public string description;
    public Rarity rarity;
}

[System.Serializable]
public class InventoryItem
{
    public int Id; // Reference to ItemObject by ID
    public int quantity;

    public InventoryItem(int id, int quantity)
    {
        Id = id;
        this.quantity = quantity;
    }
}