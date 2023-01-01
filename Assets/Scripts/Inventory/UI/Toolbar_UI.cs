using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toolbar_UI : MonoBehaviour
{
    [SerializeField] private List<Slots_UI> toolbarSlots = new List<Slots_UI>();

    private Slots_UI selectedSlot;

    private void Start() {
        SelectSlot(0);
    }

    private void Update() {
        CheckAlphaNumericKeys();
    }

    public void SelectSlot(int index) {
    if(toolbarSlots.Count > 0 && index >= 0 && index < toolbarSlots.Count) {
        if(selectedSlot != null) {
            selectedSlot.SetHighlight(false);
        }

        selectedSlot = toolbarSlots[index];
        if(selectedSlot != null) {
            selectedSlot.SetHighlight(true);
        }
    }
}

    private void CheckAlphaNumericKeys()
{
    for (int i = 0; i < 9; i++)
    {
        if (Input.GetKeyDown((KeyCode)((int)KeyCode.Alpha1 + i)))
        {
            SelectSlot(i);
        }
    }
}

}

