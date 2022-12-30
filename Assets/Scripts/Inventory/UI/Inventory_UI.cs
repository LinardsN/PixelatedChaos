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
            Setup();
        } else {
            inventoryPanel.SetActive(false);
        }
    }

    void Setup() {
        if(slots.Count == player.Inventory.slots.Count) {
            for(int i = 0; i < slots.Count; i++) {
                if(player.Inventory.slots[i].type != CollectableType.NONE) {
                    slots[i].SetItem(player.Inventory.slots[i]);
                } else {
                    slots[i].SetEmpty();
                }
            }
        }
    }
}
