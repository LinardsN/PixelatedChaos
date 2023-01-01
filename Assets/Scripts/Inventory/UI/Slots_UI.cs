using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Required for Image and Text classes
using TMPro; // Required for TextMeshProUGUI class

// Class that represents the UI for a single inventory slot
public class Slots_UI : MonoBehaviour
{
    public Image itemIcon; // Image component for displaying the item's icon
    public TextMeshProUGUI quantityText; // Text component for displaying the item's quantity
    [SerializeField] public GameObject highlight;

    // Method that sets the UI to display a given inventory slot
    public void SetItem(Inventory.Slot slot)
    {
        // If the slot is not null, display the slot's information
        if (slot != null)
        {
            itemIcon.sprite = slot.icon; // Set the image to the slot's icon sprite
            itemIcon.color = new Color(1, 1, 1, 1); // Make the image visible
            quantityText.text = slot.count.ToString(); // Set the text to the slot's item count
        }
    }

    // Method that sets the UI to display an empty inventory slot
    public void SetEmpty()
    {
        itemIcon.sprite = null; // Clear the image
        itemIcon.color = new Color(1, 1, 1, 0); // Make the image transparent
        quantityText.text = ""; // Clear the text
    }

    public void SetHighlight(bool isOn) {
        highlight.SetActive(isOn);
    }
}
