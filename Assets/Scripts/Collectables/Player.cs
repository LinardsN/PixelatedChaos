using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Inventory Inventory;

    private void Awake() {
        Inventory = new Inventory(21);
    }

    public void DropItem(Collectable item) {
        Vector3 spawnLocation = transform.position;

        float randX = Random.Range(-1f, 1f);
        float randY = Random.Range(-1f, 1f);

        Vector3 spawnOffset = new Vector3(randX, randY, 0f).normalized;

        Collectable droppedItem = Instantiate(item, spawnLocation + spawnOffset, 
            Quaternion.identity);

        droppedItem.rb2d.AddForce(spawnOffset * 2f, ForceMode2D.Impulse);
    }
}
