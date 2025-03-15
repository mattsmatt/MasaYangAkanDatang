using System.Collections.Generic;

[System.Serializable]
public class Inventory
{
    public List<InventoryItem> items = new List<InventoryItem>();

    public void AddItem(int id, int quantity)
    {
        var item = items.Find(i => i.Id == id);
        if (item != null)
        {
            item.quantity += quantity;
        }
        else
        {
            items.Add(new InventoryItem(id, quantity));
        }
    }

    public void RemoveItem(int id, int quantity)
    {
        var item = items.Find(i => i.Id == id);
        if (item != null)
        {
            item.quantity -= quantity;
            if (item.quantity <= 0)
            {
                items.Remove(item);
            }
        }
    }

    public int GetQuantity(int id) {
        var item = items.Find(i => i.Id == id);
        if (item != null)
        {
            return item.quantity;
        }
        return 0;
    }
}
