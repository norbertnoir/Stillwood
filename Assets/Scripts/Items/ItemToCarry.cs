using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemToCarry : MonoBehaviour, IInteraction
{
    public string itemName;
    public string animationName;

    string IInteraction.alertText => "Press E to carry " + itemName;

    public void Interact()
    {
        Inventory.instance.TakeItemToHands(this);
        gameObject.SetActive(false);
    }
}
