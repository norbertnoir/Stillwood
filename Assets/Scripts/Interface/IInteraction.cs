using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteraction
{
    string alertText { get; }
    bool isInteractable => true;

    void Interact();

}
