using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class that represents an item in the game
[RequireComponent(typeof(Rigidbody2D))]
public class Item : MonoBehaviour
{
   public ItemData data; // Data for this item

   // The Rigidbody2D component of this game object
   [HideInInspector] public Rigidbody2D rb2d;

    // Function that is called when the object is created
    private void Awake()
    {
        // Get the Rigidbody2D component of this game object
        rb2d = GetComponent<Rigidbody2D>();
    }
}
