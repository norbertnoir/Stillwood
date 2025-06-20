using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public RoeDeerManager roeDeerManager;
    public static LevelManager instance;

    bool decisionACompleted = false;
    bool decisionBCompleted = false;


    bool decisionAPositive = false;
    bool decisionBPositive = false;

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

    public void DecisionACompleted(bool isPositive)
    {
        decisionACompleted = true;
        decisionAPositive = isPositive;

        if (!isPositive)
        {
            // Zaczyna atakować nas sarna

            roeDeerManager.minDistanceFromPlayer = 15f;
            roeDeerManager.maxDistanceFromPlayer = 40f;
        }

        CheckDecisions();
    }

    public void DecisionBCompleted(bool isPositive)
    {
        decisionBCompleted = true;
        decisionBPositive = isPositive;

        if (isPositive)
        {
            // Szkielety zmieniają się w posągi
        }

        CheckDecisions();
    }

    void CheckDecisions()
    {
        if (decisionACompleted && decisionBCompleted)
        {
            
        }
    }
}
