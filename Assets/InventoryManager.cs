using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    // Public field for the InventoryScreen image
    public Image InventoryScreen;

    // Public field for the inventory slots
    public Image[] slots;

    void Update()
{
    // Check for the input key to open the inventory
    if (Input.GetKeyDown(KeyCode.I))
    {
        // Enable the InventoryScreen image and the child slots
        InventoryScreen.enabled = true;

        // Iterate through the inventory slots and enable each one
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].enabled = true;
        }
    }
    // Check for the input key to close the inventory
    else if (Input.GetKeyDown(KeyCode.I))
    {
        // Disable the InventoryScreen image and the child slots
        InventoryScreen.enabled = false;

        // Iterate through the inventory slots and disable each one
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].enabled = false;
        }
    }
}

}

