using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    public GameObject myCamera;
    bool isEscapeOpen = false;
    bool isInventoryOpen = false;

    private void Awake()
    {
        instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isEscapeOpen = !isEscapeOpen;
            LockCursor(isEscapeOpen);
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isInventoryOpen = !isInventoryOpen;
            ShowInventory(isInventoryOpen);
        }

    }

    void LockCursor(bool locked)
    {
        if (locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void ShowInventory(bool isActive)
    {
        myCamera.SetActive(!isActive);
        if(isActive)
            InventoryUIManager.instance.OpenInventory(Inventory.instance.items);
        else
            InventoryUIManager.instance.CloseInventory();
    }
}
