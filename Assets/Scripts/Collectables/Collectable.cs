using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    public CollectableType type;
    public Sprite icon;

    private void OnTriggerEnter2D(Collider2D collision) {
        Player player = collision.GetComponent<Player>();
        
        if (player) {
            player.Inventory.Add(this);
            Destroy(this.gameObject);
        }
    }
}

public enum CollectableType {
    NONE, 
    WOODEN_LOG,
    WOODEN_PLANK
}