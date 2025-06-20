using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItemsAnimation : MonoBehaviour
{
    public Animator animator;

    public void StartItemAnimation(string itemName)
    {
        animator.Play(itemName + "_IDLE");
    }

    public void ResetItemAnimation()
    {
        animator.Play("IDLE");
    }
}
