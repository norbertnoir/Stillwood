using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteraction
{
    private bool isOpen = false;
    [SerializeField] float duration = 0.4f;
    Animator animator;

    string IInteraction.alertText => "Press E to open";

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }


    public void Interact()
    {
        isOpen = !isOpen;
        animator.SetBool("isOpen", isOpen);
    }
}