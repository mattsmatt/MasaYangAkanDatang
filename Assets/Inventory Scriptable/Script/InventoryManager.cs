using UnityEngine;
using BayatGames.SaveGameFree;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public GameObject invmark;
    public Inventory inventory = new Inventory();

    [SerializeField] private ItemDatabase itemDatabase; // ScriptableObject holding all item data

    public string saveKey;

    public DisplayInventory displayInventory;
    

    private void Awake()
    {
        Debug.Log("inventory manager is awake");
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public int GetQuantity(int id) {
        return inventory.GetQuantity(id);
    }

    public void AddItem(int id, int quantity)
    {
        LoadInventory();
        inventory.AddItem(id, quantity);
        SaveInventory();
        displayInventory.UpdateDisplay();
        invmark.SetActive(true);
        // Debug.Log($"Added item {id} with quantity {quantity}");
    }

    public void RemoveItem(int id, int quantity)
    {
        LoadInventory();
        inventory.RemoveItem(id, quantity);
        SaveInventory();
        displayInventory.UpdateDisplay();
        // Debug.Log($"Removed item {id} with quantity {quantity}");
    }

    public InventoryItem GetInventoryItemById(int id)
    {
        return inventory.items.Find(item => item.Id == id);
    }

    public List<InventoryItem> GetInventory()
    {
        LoadInventory();
        return inventory.items;
    }

    public ItemObject GetItemById(int id)
    {
        return itemDatabase.items.Find(item => item.Id == id);
    }

    public bool InventoryContains(int id)
    {
        return inventory.items.Exists(item => item.Id == id);
    }

    public void SaveInventory()
    {
        SaveGame.Save(saveKey, inventory);
        Debug.Log("Inventory Saved!");
    }

    public void LoadInventory()
    {
        inventory = SaveGame.Load<Inventory>(saveKey, new Inventory());
        Debug.Log("Inventory Loaded!");
    }
}
