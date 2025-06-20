using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryUIManager : MonoBehaviour
{
    public static InventoryUIManager instance;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescriptionText;
    public InventoryItemDisplayer itemDisplayer;
    public GameObject inventoryCamera;
    public GameObject useInformation;

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
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            itemDisplayer.NextItem();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            itemDisplayer.PreviousItem();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            itemDisplayer.UseItem();
        }
    }

    public void UpdateItemInfo(string itemName, string itemDescription, bool isUsable)
    {
        itemNameText.text = itemName;
        itemDescriptionText.text = itemDescription;
        useInformation.SetActive(isUsable);
    }

    public void OpenInventory(List<Item> items)
    {
        this.gameObject.SetActive(true);
        inventoryCamera.SetActive(true);

        itemDisplayer.ShowItems(items);
    }

    public void CloseInventory()
    {
        this.gameObject.SetActive(false);
        inventoryCamera.SetActive(false);
    }

    public void UpdateInventoryState()
    {

    }
}