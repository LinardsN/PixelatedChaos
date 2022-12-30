using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Inventory Inventory;

    private void Awake() {
        Inventory = new Inventory(21);
    }
}
