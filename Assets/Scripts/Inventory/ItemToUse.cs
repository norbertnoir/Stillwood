using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemToUse : Item, IUsable
{
    public int effectId;
    public float duration;
    public void Use()
    {
        ItemVolumeManager.instance.StartVolume(effectId, duration);
        PlayerController.instance.ShowInventory(false);
        Inventory.instance.RemoveItem(this);
        Destroy(this.gameObject);
    }
}
