using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [Header("Data")]
    public TextAsset itemsTextFile;

    [Header("Sprites")]
    public string spriteFolderPath = "ItemSprites"; // Resources/ItemSprites/

    [Header("UI")]
    public List<Button> characterButtons; // 3 buttons in scene (Arthur, John, Dutch)
    public GameObject itemButtonPrefab;   // prefab with Button + Image
    public Transform content;             // parent for item buttons

    private Sprite[] loadedSprites;

    [HideInInspector]
    public List<Item> allItems = new List<Item>();
    [HideInInspector]
    public List<Character> characters = new List<Character>();

    public GameObject InventoryUI;

    void Awake()
    {
        LoadSprites();
        LoadAllItems();
        CreateCharactersWithRandomItems();
        AssignCharacterButtons();
    }

    #region Load Sprites & Items
    void LoadSprites()
    {
        loadedSprites = Resources.LoadAll<Sprite>(spriteFolderPath);
        if (loadedSprites.Length == 0)
        {
            Debug.LogWarning("No sprites found in Resources/" + spriteFolderPath);
        }
        else
        {
            Debug.Log($"Loaded {loadedSprites.Length} sprites.");
        }
    }

    void LoadAllItems()
    {
        if (itemsTextFile == null)
        {
            Debug.LogError("Items text file not assigned!");
            return;
        }

        allItems.Clear();

        string[] lines = itemsTextFile.text.Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            string itemName = lines[i].Trim();
            if (string.IsNullOrEmpty(itemName)) continue;

            // assign sprite by index
            Sprite sprite = (i < loadedSprites.Length) ? loadedSprites[i] : null;

            Item newItem = new Item(i, itemName, sprite);
            allItems.Add(newItem);
        }

        Debug.Log($"Loaded {allItems.Count} items with sprites.");
    }
    #endregion

    #region Character Setup
    void CreateCharactersWithRandomItems()
    {
        characters.Clear();
        string[] characterNames = { "Arthur", "John", "Dutch" };

        foreach (string charName in characterNames)
        {
            Character character = new Character(charName);

            for (int i = 0; i < 60; i++)
            {
                Item randomItem = allItems[Random.Range(0, allItems.Count)];
                Item itemCopy = new Item(randomItem.id, randomItem.name, randomItem.image);
                character.inventory.Add(itemCopy);
            }

            characters.Add(character);
            Debug.Log($"Created {charName} with {character.inventory.Count} items.");
        }
    }
    #endregion

    #region UI
    void AssignCharacterButtons()
    {
        if (characterButtons.Count != characters.Count)
        {
            Debug.LogWarning("Number of buttons does not match number of characters!");
        }

        for (int i = 0; i < characters.Count; i++)
        {
            int index = i; // capture local variable for lambda
            characterButtons[i].onClick.AddListener(() => ShowCharacterInventory(characters[index]));
        }
    }
    public void ToggleInventory()
    {
        InventoryUI.SetActive(!InventoryUI.activeInHierarchy);
        Debug.Log("Skibidi");
    }
    void ShowCharacterInventory(Character character)
    {
        // Clear previous buttons
        foreach (Transform child in content)
            Destroy(child.gameObject);

        // Create buttons for each item
        foreach (Item item in character.inventory)
        {
            GameObject itemGO = Instantiate(itemButtonPrefab, content);
            Button btn = itemGO.GetComponent<Button>();

            // 1️⃣ Set parent Image alpha to 0 (fully transparent)
            Image parentImg = itemGO.GetComponent<Image>();
            if (parentImg != null)
            {
                Color c = parentImg.color;
                c.a = 0f;
                parentImg.color = c;
            }

            // 2️⃣ Assign item sprite to child Image
            Image childImg = itemGO.GetComponentInChildren<Image>();
            if (childImg != null && item.image != null)
            {
                childImg.sprite = item.image;
                childImg.color = Color.white; // ensure fully visible
            }

            // Optional: print item name when clicked
            btn.onClick.AddListener(() => Debug.Log($"{character.name} clicked {item.name}"));
        }
    }


    #endregion
}
