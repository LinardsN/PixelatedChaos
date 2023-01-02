using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Class that represents the UI for the player's inventory
public class Inventory_UI : MonoBehaviour
{
    public string inventoryName;
    public List<Slots_UI> slots = new List<Slots_UI>(); // List of UI elements for each inventory slot

    [SerializeField]
    private Canvas canvas;
    private bool dragSingle;

    private Inventory inventory;

    private void Awake()
    {
        canvas = FindObjectOfType<Canvas>();
    }

    private void Start()
    {
        inventory = GameManager.instance.player.Inventory.GetInventoryByName(inventoryName);

        SetupSlots();
        Refresh();
    }

    // Method that updates the UI elements for each inventory slot
    public void Refresh()
    {
        // If the number of UI elements matches the number of inventory slots
        if (slots.Count == inventory.slots.Count)
        {
            // Update each UI element with the corresponding inventory slot
            for (int i = 0; i < slots.Count; i++)
            {
                // If the inventory slot is not empty, set the UI element to display the slot's information
                if (inventory.slots[i].itemName != "")
                {
                    slots[i].SetItem(inventory.slots[i]);
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
    public void Remove()
    {
        // Get the item in the specified inventory slot
        Item itemToDrop = GameManager.instance.itemManager.GetItemByName(
            inventory.slots[UI_Manager.draggedSlot.slotID].itemName
        );

        // If the item exists, drop it and remove it from the inventory
        if (itemToDrop != null)
        {
            if (UI_Manager.dragSingle)
            {
                GameManager.instance.player.DropItem(itemToDrop); // Drop the item
                inventory.Remove(UI_Manager.draggedSlot.slotID); // Remove the item from the inventory
            }
            else
            {
                GameManager.instance.player.DropItem(itemToDrop, inventory.slots[UI_Manager.draggedSlot.slotID].count); // Drop the item
                inventory.Remove(
                    UI_Manager.draggedSlot.slotID,
                    inventory.slots[UI_Manager.draggedSlot.slotID].count
                ); // Remove the item from the inventory
            }
            Refresh(); // Refresh the inventory UI
        }

        UI_Manager.draggedSlot = null;
    }

    public void SlotBeginDrag(Slots_UI slot)
    {
        UI_Manager.draggedSlot = slot;
        UI_Manager.draggedIcon = Instantiate(UI_Manager.draggedSlot.itemIcon);
        UI_Manager.draggedIcon.transform.SetParent(canvas.transform);
        UI_Manager.draggedIcon.raycastTarget = false;
        UI_Manager.draggedIcon.rectTransform.sizeDelta = new Vector2(70, 70);
        MoveToMousePosition(UI_Manager.draggedIcon.gameObject);
    }

    public void SlotDrag()
    {
        MoveToMousePosition(UI_Manager.draggedIcon.gameObject);
    }

    public void SlotEndDrag()
    {
        Destroy(UI_Manager.draggedIcon.gameObject);
        UI_Manager.draggedIcon = null;
    }

    public void SlotDrop(Slots_UI slot)
    {
        if(UI_Manager.dragSingle) {
            UI_Manager.draggedSlot.inventory.MoveSlot(UI_Manager.draggedSlot.slotID, slot.slotID, slot.inventory);
        } else {
            UI_Manager.draggedSlot.inventory.MoveSlot(UI_Manager.draggedSlot.slotID, slot.slotID, slot.inventory, UI_Manager.draggedSlot.inventory.slots[UI_Manager.draggedSlot.slotID].count);
        }
        GameManager.instance.uiManager.RefreshAll();
        
    }

    private void MoveToMousePosition(GameObject toMove)
    {
        if (canvas != null)
        {
            Vector2 position;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                Input.mousePosition,
                null,
                out position
            );

            toMove.transform.position = canvas.transform.TransformPoint(position);
        }
    }

    void SetupSlots()
    {
        int counter = 0;

        foreach (Slots_UI slot in slots)
        {
            slot.slotID = counter;
            counter++;
            slot.inventory = inventory;
        }
    }
}
