using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;
    public List<Item> items = new List<Item>();
    public ItemToCarry itemInHands;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddItem(Item itm)
    {
        items.Add(itm);
    }

    public void DropItem(Item itm)
    {
        if (items.Contains(itm))
        {
            items.Remove(itm);
            itm.DropItem();
        }
    }

    public void RemoveItem(Item itm)
    {
        items.Remove(itm);
    }

    public void TakeItemToHands(ItemToCarry itm)
    {
        if (itemInHands == null)
        {
            itemInHands = itm;
            // HandsAnimationManager.instance.PlayAnimation(itm.animationName);
        }
        else
        {
            Debug.Log("You already have an item in your hands.");
        }
    }


    public void UseInHandsItem()
    {
        if (itemInHands != null)
        {
            Destroy(itemInHands.gameObject);
            itemInHands = null;
        }
    }
}
