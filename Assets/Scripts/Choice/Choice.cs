using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Choice : MonoBehaviour
{
    public string choiceTextA;
    public string choiceTextB;

    public ItemToCarry itemToCarry;

    public GameObject RoeDeerInHands; // hotfix 

    public void MakeChoiceA()
    {
        gameObject.SetActive(false);
        LevelManager.instance.DecisionACompleted(true);
    }

    public void MakeChoiceB()
    {
        itemToCarry.Interact();
        gameObject.SetActive(false);
        RoeDeerInHands.SetActive(true);
        LevelManager.instance.DecisionACompleted(false);
    }
}
