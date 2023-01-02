using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attribute that allows this class to be serialized
[System.Serializable]
public class Inventory
{
    // Class that represents a single slot in the inventory
    [System.Serializable]
    public class Slot
    {
        public string itemName; // The name of the item stored in this slot
        public int count; // The number of items in this slot
        public int maxAllowed; // The maximum number of items that can be stored in this slot
        public Sprite icon; // The icon of the item stored in this slot

        // Constructor that initializes the slot with default values
        public Slot()
        {
            itemName = "";
            count = 0;
            maxAllowed = 99;
        }

        public bool IsEmpty {
            get {
                if(itemName == "" && count == 0) {
                    return true;
                }

                return false;
            }
        }

        // Method that returns whether this slot can accept more items
        public bool CanAddItem(string itemName)
        {
            if (this.itemName == itemName && count < maxAllowed)
            {
                return true;
            }
            return false;
        }

        // Method that adds an item to this slot
        public void AddItem(Item item)
        {
            this.itemName = item.data.itemName;
            this.icon = item.data.icon;
            count++;
        }

        public void AddItem(string itemName, Sprite icon, int maxAllowed)
        {
            this.itemName = itemName;
            this.icon = icon;
            count++;
            this.maxAllowed = maxAllowed;
        }

        // Method that removes an item from this slot
        public void RemoveItem()
        {
            if (count > 0)
            {
                count--;

                // If there are no more items in the slot, clear the item name and icon
                if (count == 0)
                {
                    icon = null;
                    itemName = "";
                }
            }
        }
    }

    public List<Slot> slots = new List<Slot>(); // The list of slots in the inventory

    // Constructor that creates an inventory with a specified number of slots
    public Inventory(int numSlots)
    {
        // Create a new slot for each slot in the inventory
        for (int i = 0; i < numSlots; i++)
        {
            Slot slot = new Slot();
            slots.Add(slot);
        }
    }

    // Method that adds an item to the inventory
    public void Add(Item item)
    {
        // Check each slot to see if it can accept the item being added
        foreach (Slot slot in slots)
        {
            if (slot.itemName == item.data.itemName && slot.CanAddItem(item.data.itemName))
            {
                slot.AddItem(item);
                return;
            }
        }

        // If no suitable slot is found, check for empty slots
        foreach (Slot slot in slots)
        {
            if (slot.itemName == "")
            {
                slot.AddItem(item);
                return;
            }
        }
    }

    // Method that removes an item from the inventory
    public void Remove(int index)
    {
        slots[index].RemoveItem();
    }

    public void Remove(int index, int numToRemove)
    {
        if (slots[index].count >= numToRemove)
        {
            for (int i = 0; i < numToRemove; i++)
            {
                Remove(index);
            }
        }
    }

    public void MoveSlot(int fromIndex, int toIndex, Inventory toInventory, int numToMove = 1) {
        Slot fromSlot = slots[fromIndex];
        Slot toSlot = toInventory.slots[toIndex];

        if(toSlot.IsEmpty || toSlot.CanAddItem(fromSlot.itemName)) {
            for(int i = 0; i < numToMove; i++) {
                toSlot.AddItem(fromSlot.itemName, fromSlot.icon, fromSlot.maxAllowed);
            fromSlot.RemoveItem();
            }
    
        }
    }
}
