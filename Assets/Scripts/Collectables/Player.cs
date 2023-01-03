using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class that represents a player character in the game
public class Player : MonoBehaviour
{
    public InventoryManager Inventory;

    // Function that is called when the object is created
    private void Awake()
    {
        Inventory = GetComponent<InventoryManager>();
    }

    // Function that is called once per frame
    private void Update()
    {
        // Check if the player pressed the Space key
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Round the player's position to the nearest integer value
            Vector3Int position = Vector3Int.RoundToInt(transform.position);

            // Check if the tile the player is standing on is interactable
            if (GameManager.instance.tileManager.IsInteractable(position))
            {
                Debug.Log("Tile is interactable");
                GameManager.instance.tileManager.SetInteracted(position);
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            // Get a list of all trees within a certain radius of the player
            Collider2D[] trees = Physics2D.OverlapCircleAll(transform.position, 1.0f);

            // Loop through each tree
            foreach (Collider2D treeCollider in trees)
            {
                // Check if the collider is a box collider
                if (treeCollider is BoxCollider2D)
                {
                    // Get the Tree script component of the tree game object
                    Tree tree = treeCollider.GetComponent<Tree>();

                    // Check if the tree script component is not null
                    if (tree != null)
                    {
                        // Chop down the tree and instantiate wooden logs
                        tree.ChopDown(transform.position);
                    }
                }
            }
        }
    }

    // Method that drops an item from the player's inventory
    public void DropItem(Item item)
    {
        // Get the player's current position
        Vector3 spawnLocation = transform.position;

        // Generate random values between -1 and 1 for the x and y offsets
        float randX = Random.Range(-1f, 1f);
        float randY = Random.Range(-1f, 1f);

        // Calculate a normalized vector for the offset based on the random values
        Vector3 spawnOffset = new Vector3(randX, randY, 0f).normalized;

        // Create a new instance of the item at the calculated spawn location
        Item droppedItem = Instantiate(item, spawnLocation + spawnOffset, Quaternion.identity);

        // Add force to the item's Rigidbody2D to make it move away from the player
        droppedItem.rb2d.AddForce(spawnOffset * 2f, ForceMode2D.Impulse);
    }

    public void DropItem(Item item, int numToDrop)
    {
        for (int i = 0; i < numToDrop; i++)
        {
            DropItem(item);
        }
    }
}
