using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using BayatGames.SaveGameFree;
using System.Linq;


public class DisplayPuzzle : MonoBehaviour
{
    public GameObject puzzleprefab;
    public InventoryManager inventoryManager;
    Dictionary<int, GameObject> itemDisplayed = new Dictionary<int, GameObject>();
    public GameObject menu;
    public GameObject mainmenu;
    public GameObject ingamemenu;
    public ST_PuzzleDisplay st_puzzledisplay;

    public List<PuzzleLevelData> levelDataList = new List<PuzzleLevelData>();

    public Sprite lockImg;


    // Start is called before the first frame update
    void Start()
    {
        LoadLevelData();
        ingamemenu.SetActive(false);
        Debug.Log("starting display puzzle");
        CreateDisplay();
    }

    public void ExitPuzzle()
    {
        st_puzzledisplay.PlayMenuSFX();
        SceneManager.LoadScene(PlayerPrefs.GetInt("curr_scene"));
        Debug.Log("Puzzle Complete!");
        // levelCompletePanel.SetActive(true);
        // SceneManager.LoadScene("testing");
    }

    void Update()
    {
        // UpdateDisplay();
    }

    public void LoadLevelData()
    {
        levelDataList = SaveGame.Load<List<PuzzleLevelData>>("slidingLevel", new List<PuzzleLevelData>());
    }

    public void SaveLevelData()
    {
        SaveGame.Save("slidingLevel", levelDataList);
    }

    public void PlayPuzzle()
    {
        mainmenu.SetActive(false);
    }

    public void CreateDisplay()
    {
        Debug.Log("masuk");
        int levelIdx = 0;
        foreach (var slot in inventoryManager.GetInventory())
        {
            if (!itemDisplayed.ContainsKey(slot.Id))
            {
                var obj = Instantiate(puzzleprefab, Vector3.zero, Quaternion.identity, menu.transform);

                // Set item sprite
                obj.transform.GetChild(0).GetComponent<Image>().sprite = inventoryManager.GetItemById(slot.Id).img;

                // Set rarity color
                var rarityPanel = obj.transform.GetChild(1).GetComponent<Image>();
                rarityPanel.color = Color.black;

                // Set quantity text
                // obj.GetComponentInChildren<TextMeshProUGUI>().text = slot.quantity.ToString("n0");
                obj.GetComponentInChildren<TextMeshProUGUI>().text = "PB: " + levelDataList.Find(PuzzleLevelData => PuzzleLevelData.Id == inventoryManager.GetItemById(slot.Id).Id).bestTime.ToString("n2") + " s";

                var button = obj.GetComponent<Button>();
                int itemId = slot.Id; // Capture the current item ID in a local variable
                                      // button.onClick.AddListener(() => ShowItemDetails(itemId));

                InventoryItem targetItem = InventoryManager.Instance.inventory.items.Find(
                    delegate (InventoryItem item)
                    {
                        return item.Id == slot.Id;
                    }
                );
                int currIndex = InventoryManager.Instance.inventory.items.IndexOf(targetItem);

                // Debug.Log(currIndex);

                if ((currIndex + 1) > Save.instance.lastUnlockedSlidingTile)
                {
                    Debug.Log("Level " + (currIndex + 1) + ": locked");
                    button.interactable = false;
                    obj.transform.GetChild(0).GetComponent<Image>().sprite = lockImg;
                }
                else
                {
                    Debug.Log("Level " + (currIndex + 1) + ": unlocked");
                    button.onClick.AddListener(PlayPuzzle);
                    button.onClick.AddListener(() => st_puzzledisplay.Initpuzzle(SpriteToTexture2D(obj.transform.GetChild(0).GetComponent<Image>().sprite), inventoryManager.GetItemById(slot.Id).Id));
                }

                itemDisplayed.Add(slot.Id, obj);
            }
            levelIdx++;
        }
    }

    public void NextLevel()
    {
        int lastLevelIndex = levelDataList.Count - 1;
        PuzzleLevelData currElement = levelDataList.Find(
            delegate (PuzzleLevelData level)
            {
                return level.Id == st_puzzledisplay.puzzle_id;
            }
        );
        int currIndex = levelDataList.IndexOf(currElement);

        st_puzzledisplay.audioManager.PlaySFX(st_puzzledisplay.audioManager.OnMenuClick);

        if (currIndex == lastLevelIndex)
        {
            int buildIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(buildIndex);
        }
        else
        {
            st_puzzledisplay.levelCompletePanel.SetActive(false);

            st_puzzledisplay.audioManager.PlayMusic(st_puzzledisplay.audioManager.puzzleBGM);
            int nextIdx = currIndex + 1;
            var element = inventoryManager.GetItemById(levelDataList[nextIdx].Id);
            st_puzzledisplay.Initpuzzle(SpriteToTexture2D(element.img), element.Id);
        }
    }

    public void Retry()
    {
        st_puzzledisplay.audioManager.PlaySFX(st_puzzledisplay.audioManager.OnMenuClick);
        st_puzzledisplay.audioManager.PlayMusic(st_puzzledisplay.audioManager.puzzleBGM);

        st_puzzledisplay.levelCompletePanel.SetActive(false);

        var element = inventoryManager.GetItemById(st_puzzledisplay.puzzle_id);
        st_puzzledisplay.Initpuzzle(SpriteToTexture2D(element.img), element.Id);
    }

    public void UpdateDisplay()
    {
        int levelIdx = 0;
        foreach (var slot in inventoryManager.GetInventory())
        {
            if (itemDisplayed.ContainsKey(slot.Id))
            {
                // Update quantity
                itemDisplayed[slot.Id].GetComponentInChildren<TextMeshProUGUI>().text = slot.quantity.ToString("n0");
            }
            else
            {
                // Add new item to display
                var obj = Instantiate(puzzleprefab, Vector3.zero, Quaternion.identity, menu.transform);

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
                                      // button.onClick.AddListener(() => ShowItemDetails(itemId));

                button.onClick.AddListener(PlayPuzzle);
                button.onClick.AddListener(() => st_puzzledisplay.Initpuzzle(SpriteToTexture2D(obj.transform.GetChild(0).GetComponent<Image>().sprite), inventoryManager.GetItemById(slot.Id).Id));

                itemDisplayed.Add(slot.Id, obj);
            }
            levelIdx++;
        }

        // Remove UI elements for items no longer in inventory
        var keysToRemove = new List<int>();
        foreach (var key in itemDisplayed.Keys)
        {
            if (!inventoryManager.InventoryContains(key))
            {
                Destroy(itemDisplayed[key]);
                keysToRemove.Add(key);
            }
        }
        foreach (var key in keysToRemove)
        {
            itemDisplayed.Remove(key);
        }
    }

    public Texture2D SpriteToTexture2D(Sprite sprite)
    {
        if (sprite.rect.width != sprite.texture.width)
        {
            Texture2D newText = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            Color[] newColors = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                            (int)sprite.textureRect.y,
                                                            (int)sprite.textureRect.width,
                                                            (int)sprite.textureRect.height);
            newText.SetPixels(newColors);
            newText.Apply();
            return newText;
        }
        else
            return sprite.texture;
    }

}