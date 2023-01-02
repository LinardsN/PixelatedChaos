using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class that represents a collectable item in the game
[RequireComponent(typeof(Item))]
public class Collectable : MonoBehaviour
{
    // Function that is called when another collider enters the trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Get the Player component of the collider
        Player player = collision.GetComponent<Player>();

        // If the collider has a Player component, add the collectable item to the player's inventory
        if (player)
        {
            // Get the Item component of this game object
            Item item = GetComponent<Item>();

            // If the item component exists, add it to the player's inventory and destroy the game object
            if (item != null)
            {
                player.Inventory.Add("Backpack", item);
                Destroy(this.gameObject);
            }
        }
    }
}
