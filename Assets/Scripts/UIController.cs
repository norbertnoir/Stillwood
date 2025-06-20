using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public static UIController instance;
    public GameObject interactionPanel;
    public TextMeshProUGUI alertText;

    public GameObject choicePanel;
    public TextMeshProUGUI optionTextA;
    public TextMeshProUGUI optionTextB;



    private void Awake()
    {
        if (UIController.instance == null)
        {
            UIController.instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowInteractionPanel(bool isOpen, string alertText = " ")
    {
        if (interactionPanel.activeSelf == isOpen)
            return;

        this.alertText.text = alertText;

        interactionPanel.SetActive(isOpen);
    }

    public void ShowChoicePanel(bool isOpen, string optionTextA = " ", string optionTextB = " ")
    {
        if (choicePanel.activeSelf == isOpen)
            return;

        this.optionTextA.text = optionTextA;
        this.optionTextB.text = optionTextB;

        choicePanel.SetActive(isOpen);
    }

    public void HidePanels()
    {
        interactionPanel.SetActive(false);
        choicePanel.SetActive(false);
    }
}
