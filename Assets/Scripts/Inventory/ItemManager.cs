using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class that manages the collection of items in the game
public class ItemManager : MonoBehaviour
{
    public Item[] items; // Array of all items in the game

    private Dictionary<string, Item> nameToItemDict = 
        new Dictionary<string, Item>(); // Dictionary that maps item names to item objects

    // Function that is called when the object is created
    private void Awake()
    {
        // Add each item to the dictionary
        foreach (Item item in items)
        {
            AddItem(item);
        }
    }

    // Method that adds an item to the dictionary
    private void AddItem(Item item)
    {
        // Only add the item if it doesn't already exist in the dictionary
        if (!nameToItemDict.ContainsKey(item.data.itemName))
        {
            nameToItemDict.Add(item.data.itemName, item);
        }
    }

    // Method that retrieves an item from the dictionary by its name
    public Item GetItemByName(string key)
    {
        // Check if the dictionary contains an item with the specified name
        if (nameToItemDict.ContainsKey(key))
        {
            // Return the item if it exists
            return nameToItemDict[key];
        }

        // Return null if the item does not exist
        return null;
    }
}
