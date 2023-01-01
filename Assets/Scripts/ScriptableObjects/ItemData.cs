using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class that stores data for an item in the game
[CreateAssetMenu(fileName = "Item Data", menuName = "Item Data", order = 50)]
public class ItemData : ScriptableObject
{
    public string itemName = "Item Name"; // The name of the item
    public Sprite icon; // The sprite used to represent the item's icon
}
