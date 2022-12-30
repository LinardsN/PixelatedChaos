using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_UI : MonoBehaviour
{

    public GameObject inventoryPanel;
    public Player player;
    public List<Slots_UI> slots = new List<Slots_UI>();

    // Update is called once per frame
    void Update() {
        if(Input.GetKeyDown(KeyCode.I)) {
            ToggleInventory();
        }
    }

    public void ToggleInventory() {
        if(!inventoryPanel.activeSelf) {
            inventoryPanel.SetActive(true);
            Refresh();
        } else {
            inventoryPanel.SetActive(false);
        }
    }

    void Refresh() {
        if(slots.Count == player.Inventory.slots.Count) {
            for(int i = 0; i < slots.Count; i++) {
                if(player.Inventory.slots[i].itemName != "") {
                    slots[i].SetItem(player.Inventory.slots[i]);
                } else {
                    slots[i].SetEmpty();
                }
            }
        }
    }

    public void Remove(int slotID) {
        Item itemToDrop = GameManager.instance.itemManager.GetItemByName(
            player.Inventory.slots[slotID].itemName);
        
        if(itemToDrop != null) {
            player.DropItem(itemToDrop);
            player.Inventory.Remove(slotID);
            Refresh();
        }
    }
}
