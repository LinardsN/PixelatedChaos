using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class that represents the UI for the player's inventory
public class Inventory_UI : MonoBehaviour
{
    public GameObject inventoryPanel; // GameObject for the inventory panel
    public Player player; // The player character
    public List<Slots_UI> slots = new List<Slots_UI>(); // List of UI elements for each inventory slot
    private Slots_UI draggedSlot;

    // Function that is called once per frame
    void Update()
    {
        Refresh();
        // Check if the player pressed the "I" key
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory(); // Toggle the visibility of the inventory panel
        }
    }

    // Method that toggles the visibility of the inventory panel
    public void ToggleInventory()
    {
        // If the panel is not active, set it to active and refresh the inventory slots
        if (!inventoryPanel.activeSelf)
        {
            inventoryPanel.SetActive(true);
            Refresh();
        }
        // If the panel is active, set it to inactive
        else
        {
            inventoryPanel.SetActive(false);
        }
    }

        // Method that updates the UI elements for each inventory slot
    void Refresh()
    {
        // If the number of UI elements matches the number of inventory slots
        if (slots.Count == player.Inventory.slots.Count)
        {
            // Update each UI element with the corresponding inventory slot
            for (int i = 0; i < slots.Count; i++)
            {
                // If the inventory slot is not empty, set the UI element to display the slot's information
                if (player.Inventory.slots[i].itemName != "")
                {
                    slots[i].SetItem(player.Inventory.slots[i]);
                }
                // If the inventory slot is empty, set the UI element to display an empty slot
                else
                {
                    slots[i].SetEmpty();
                }
            }
        }
    }

    // Method that removes an item from the specified inventory slot
    public void Remove(int slotID)
    {
        // Get the item in the specified inventory slot
        Item itemToDrop = GameManager.instance.itemManager.GetItemByName(
            player.Inventory.slots[slotID].itemName);

        // If the item exists, drop it and remove it from the inventory
        if (itemToDrop != null)
        {
            player.DropItem(itemToDrop); // Drop the item
            player.Inventory.Remove(slotID); // Remove the item from the inventory
            Refresh(); // Refresh the inventory UI
        }
    }

    public void SlotBeginDrag(Slots_UI slot) {
        draggedSlot = slot;
        Debug.Log("Start Drag "+ draggedSlot.name);
    }

    public void SlotDrag() {
        Debug.Log("Dragging: " + draggedSlot.name);
    }

    public void SlotEndDrag() {
        Debug.Log("Dragging finished: " + draggedSlot.name);
    }

    public void SlotDrop(Slots_UI slot) {
        Debug.Log("Dropped " + draggedSlot.name + " on " + slot.name);
    }
}
