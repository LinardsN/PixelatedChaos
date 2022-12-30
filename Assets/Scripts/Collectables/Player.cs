using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Inventory Inventory;

    private void Awake() {
        Inventory = new Inventory(21);
    }

    private void Update() {
    if(Input.GetKeyDown(KeyCode.Space)) {
        // Round the player's position to the nearest integer value
        Vector3Int position = Vector3Int.RoundToInt(transform.position);

        if(GameManager.instance.tileManager.IsInteractable(position)) {
            Debug.Log("Tile is interactable");
            GameManager.instance.tileManager.SetInteracted(position);
        }
    }
}

    public void DropItem(Item item) {
        Vector3 spawnLocation = transform.position;

        float randX = Random.Range(-1f, 1f);
        float randY = Random.Range(-1f, 1f);

        Vector3 spawnOffset = new Vector3(randX, randY, 0f).normalized;

        Item droppedItem = Instantiate(item, spawnLocation + spawnOffset, 
            Quaternion.identity);

        droppedItem.rb2d.AddForce(spawnOffset * 2f, ForceMode2D.Impulse);
    }
}
