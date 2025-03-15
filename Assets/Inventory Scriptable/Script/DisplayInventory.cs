using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq; // Add this to use LINQ methods


public class DisplayInventory : MonoBehaviour
{
    public GameObject inventoryPrefab; // Prefab for displaying an inventory item
    public InventoryManager inventoryManager; // Reference to the InventoryManager
    private Dictionary<int, GameObject> itemDisplayed = new Dictionary<int, GameObject>(); // Using item ID as key

    public Button interactButton; // Button to open/close inventory
    public GameObject inventoryPanel; // Panel containing inventory UI

    public GameObject invmark;

    public GameObject inventoryUI;
    

    // public Toggle allToggle;
    // public Toggle consumableToggle;
    // public Toggle equipmentToggle;
    // public Toggle keyitemToggle;
    // public Toggle currencyToggle;

    public GameObject itemDetailPanel;
    public GameObject interactcanvas;

    void Start() {
        // allToggle.onValueChanged.AddListener(isOn => {if (isOn) ShowAllItems();});
        // consumableToggle.onValueChanged.AddListener(isOn => {if (isOn) SortItemsByType(ItemType.Consumable);});
        // equipmentToggle.onValueChanged.AddListener(isOn => {if (isOn) SortItemsByType(ItemType.Equipment);});
        // keyitemToggle.onValueChanged.AddListener(isOn => {if (isOn) SortItemsByType(ItemType.KeyItem);});
        // currencyToggle.onValueChanged.AddListener(isOn => {if (isOn) SortItemsByType(ItemType.Currency);});

        CreateDisplay();
        Debug.Log("display created");

        inventoryUI.SetActive(false);
        itemDetailPanel.SetActive(false);
        interactButton.transform.parent.gameObject.SetActive(true);
    }

    public void ShowAllItems() {
        CreateDisplay();
    }

    void Update() {
        // UpdateDisplay();
    }

    public void AddInventory() {
        InventoryManager.Instance.AddItem(0,1);
    }

    public void OpenInventory() {
        interactcanvas.SetActive(false);
        invmark.SetActive(false);
        interactButton.transform.parent.gameObject.SetActive(false);
        inventoryUI.SetActive(true);
        UpdateDisplay();
        itemDetailPanel.SetActive(false);
    }

    public void CloseInventory() {
        interactcanvas.SetActive(true);
        inventoryUI.SetActive(false);
        interactButton.transform.parent.gameObject.SetActive(true);
    }
    
    public void CreateDisplay() {
        // Create UI elements for each item in the inventory
        foreach (var slot in inventoryManager.GetInventory())
        {
            if (!itemDisplayed.ContainsKey(slot.Id)) {
                if (slot.quantity == 0) continue;
                var obj = Instantiate(inventoryPrefab, Vector3.zero, Quaternion.identity, inventoryPanel.transform);

                // Set item sprite
                obj.transform.GetChild(0).GetComponent<Image>().sprite = inventoryManager.GetItemById(slot.Id).img;

                // Set rarity color
                var rarityPanel = obj.transform.GetChild(1).GetComponent<Image>();
                string hexColor = inventoryManager.GetItemById(slot.Id).rarity switch
                {
                    Rarity.Common => "#0DF81A",    // Green
                    Rarity.Rare => "#AB0DF8",      // Purple
                    Rarity.Legendary => "#E5DE00", // Gold
                    _ => "#FFFFFF"                 // Default White
                };
                rarityPanel.color = ColorUtility.TryParseHtmlString(hexColor, out var color) ? color : Color.white;

                // Set quantity text
                obj.GetComponentInChildren<TextMeshProUGUI>().text = slot.quantity.ToString("n0");
                var button = obj.GetComponent<Button>();
                int itemId = slot.Id; // Capture the current item ID in a local variable
                button.onClick.AddListener(() => ShowItemDetails(itemId));

                itemDisplayed.Add(slot.Id, obj);
            }
        }
    }

    public void UpdateDisplay() {
        // Update or add new items in the inventory
        foreach (var slot in inventoryManager.GetInventory())
        {
            if (itemDisplayed.ContainsKey(slot.Id)) {
                // Update quantity
                itemDisplayed[slot.Id].GetComponentInChildren<TextMeshProUGUI>().text = slot.quantity.ToString("n0");
            } else {
                // Add new item to display
                var obj = Instantiate(inventoryPrefab, Vector3.zero, Quaternion.identity, inventoryPanel.transform);

                // Set item sprite
                obj.transform.GetChild(0).GetComponent<Image>().sprite = inventoryManager.GetItemById(slot.Id).img;

                // Set rarity color
                var rarityPanel = obj.transform.GetChild(1).GetComponent<Image>();
                string hexColor = inventoryManager.GetItemById(slot.Id).rarity switch
                {
                    Rarity.Common => "#0DF81A",    // Green
                    Rarity.Rare => "#AB0DF8",      // Purple
                    Rarity.Legendary => "#E5DE00", // Gold
                    _ => "#FFFFFF"                 // Default White
                };
                rarityPanel.color = ColorUtility.TryParseHtmlString(hexColor, out var color) ? color : Color.white;

                // Set quantity text
                obj.GetComponentInChildren<TextMeshProUGUI>().text = slot.quantity.ToString("n0");
                var button = obj.GetComponent<Button>();
                int itemId = slot.Id; // Capture the current item ID in a local variable
                button.onClick.AddListener(() => ShowItemDetails(itemId));

                itemDisplayed.Add(slot.Id, obj);
            }
        }

        // Remove UI elements for items no longer in inventory
        var keysToRemove = new List<int>();
        foreach (var key in itemDisplayed.Keys)
        {
            if (!inventoryManager.InventoryContains(key)) {
                Destroy(itemDisplayed[key]);
                keysToRemove.Add(key);
            }
        }
        foreach (var key in keysToRemove)
        {
            itemDisplayed.Remove(key);
        }
    }

    public void SortItemsByType(ItemType type)
    {
        // Clear existing UI
        foreach (var obj in itemDisplayed.Values)
        {
            if (obj != null)
            {
                Destroy(obj); // Destroy the UI element
            }
        }
        itemDisplayed.Clear(); // Clear the dictionary

        // Filter items by type
        var sortedItems = inventoryManager.GetInventory()
            .Where(slot => inventoryManager.GetItemById(slot.Id).type == type)
            .ToList();

        // Recreate UI with sorted items
        foreach (var slot in sortedItems)
        {
            if (itemDisplayed.ContainsKey(slot.Id)) continue; // Skip if already displayed

            var obj = Instantiate(inventoryPrefab, Vector3.zero, Quaternion.identity, inventoryPanel.transform);

            // Set item sprite
            obj.transform.GetChild(0).GetComponent<Image>().sprite = inventoryManager.GetItemById(slot.Id).img;

            // Set rarity color
            var rarityPanel = obj.transform.GetChild(1).GetComponent<Image>();
            string hexColor = inventoryManager.GetItemById(slot.Id).rarity switch
            {
                Rarity.Common => "#0DF81A",    // Green
                Rarity.Rare => "#AB0DF8",      // Purple
                Rarity.Legendary => "#E5DE00", // Gold
                _ => "#FFFFFF"                 // Default White
            };
            rarityPanel.color = ColorUtility.TryParseHtmlString(hexColor, out var color) ? color : Color.white;

            // Set quantity text
            obj.GetComponentInChildren<TextMeshProUGUI>().text = slot.quantity.ToString("n0");

            var button = obj.GetComponent<Button>();
            int itemId = slot.Id; // Capture the current item ID in a local variable
            button.onClick.AddListener(() => ShowItemDetails(itemId));

            // Add the new UI element to the dictionary
            itemDisplayed.Add(slot.Id, obj);
        }
    }

    public void ShowItemDetails(int itemId)
    {
        // Fetch the item details from InventoryManager
        var item = inventoryManager.GetItemById(itemId);

        if (item != null)
        {
            // Access child components of the parent panel dynamically
            var rarityPanel = itemDetailPanel.transform.GetChild(0).GetComponentInChildren<Image>();
            var image = itemDetailPanel.transform.GetChild(0).GetChild(0).GetComponentInChildren<Image>();
            var nameText = itemDetailPanel.transform.GetChild(0).GetChild(1).GetComponentInChildren<TextMeshProUGUI>();
            var typeText = itemDetailPanel.transform.GetChild(0).GetChild(2).GetComponentInChildren<TextMeshProUGUI>();
            var descriptionText = itemDetailPanel.transform.GetChild(0).GetChild(3).GetComponentInChildren<TextMeshProUGUI>();

            // Update UI with item details
            image.sprite = item.img;
            nameText.text = item.name;
            typeText.text = item.type.ToString(); // Assuming type is an enum or string
            descriptionText.text = item.description;
            string hexColor = item.rarity switch
            {
                Rarity.Common => "#0DF81A",    // Green
                Rarity.Rare => "#AB0DF8",      // Purple
                Rarity.Legendary => "#F5EB9C", // Gold
                _ => "#FFFFFF"                 // Default White
            };
            if (ColorUtility.TryParseHtmlString(hexColor, out var color))
            {
                color.a = 100f / 255f; // Set transparency to 100 out of 255
                rarityPanel.color = color;
            }
            else
            {
                rarityPanel.color = Color.white;
            }
            // Make sure the panel is active
            itemDetailPanel.SetActive(true);
        }
    }

}
