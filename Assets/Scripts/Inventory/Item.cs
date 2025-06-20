using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, IInteraction
{
    public string itemName;
    public string itemDescription;
    public string animationName;


    string IInteraction.alertText => "Press E to take " + itemName;

    public void Interact()
    {
        Inventory.instance.AddItem(this); 
        gameObject.SetActive(false);
    }

    

    public void DropItem()
    {
        // Logic to drop the item
        Destroy(gameObject);
    }

    public void ShowItem()
    {
        gameObject.SetActive(true);
    }

}
