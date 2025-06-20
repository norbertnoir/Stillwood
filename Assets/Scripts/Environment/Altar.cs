using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Altar : MonoBehaviour, IInteraction
{
    public GameObject RoeDeerInHands; // hotfix
    [SerializeField] private ItemToCarry altarKey;
    [SerializeField] private GameObject activeKey;

    string IInteraction.alertText => "You need to use the Altar Key to open this altar";
    bool isInteractable = true;
    bool IInteraction.isInteractable => isInteractable;

    public void Interact()
    {
        if (Inventory.instance.itemInHands == altarKey)
        {
            RoeDeerInHands.SetActive(false); // hotfix

            activeKey.SetActive(true);
            Inventory.instance.UseInHandsItem();
            isInteractable = false;

            LevelManager.instance.DecisionACompleted(false);
        }
        else
        {
            if (Inventory.instance.itemInHands != null)
            {
                Debug.Log("You need to use the Altar Key to open this altar.");
            }
            else
            {
                Debug.Log("You need to use an item to open this altar.");
            }
        }





    }
}
