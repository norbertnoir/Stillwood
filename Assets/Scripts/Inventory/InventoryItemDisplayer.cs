using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryItemDisplayer : MonoBehaviour
{
    public Transform container;
    List<Vector3> itemsPositions = new List<Vector3>();
    List<Item> items = new List<Item>();
    int currentItem = 0;
    float finalRotation = 0f;
    public float rotationSpeed = 5f;
    public float radius = 0.5f;



    public static List<Vector3> GetPositionsOnCircle(int itemCount, float radius)
    {
        List<Vector3> positions = new List<Vector3>();
        float angleStep = 360f / itemCount;

        float startAngle = 270f * Mathf.Deg2Rad;

        for (int i = 0; i < itemCount; i++)
        {
            float angle = startAngle - i * angleStep * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            positions.Add(new Vector3(x, 0, z)); // układ XZ
        }

        return positions;
    }

    public void ShowItems(List<Item> _items)
    {
        items = _items;
        itemsPositions = GetPositionsOnCircle(_items.Count, radius);
        for (int i = 0; i < _items.Count; i++)
        {
            Item item = _items[i];
            item.ShowItem();
            item.transform.SetParent(container);
            item.transform.localPosition = itemsPositions[i];
        }
        ShowItemInfo();
    }
    void Update()
    {
        container.localEulerAngles = new Vector3(0, Mathf.LerpAngle(container.localEulerAngles.y, finalRotation, Time.deltaTime * rotationSpeed), 0);
    }

    public void NextItem()
    {
        currentItem++;
        if (currentItem >= itemsPositions.Count)
        {
            currentItem = 0;
        }

        finalRotation += 360f / itemsPositions.Count;
        ShowItemInfo();
    }

    public void PreviousItem()
    {
        currentItem--;
        if (currentItem < 0)
        {
            currentItem = itemsPositions.Count - 1;
        }

        finalRotation -= 360f / itemsPositions.Count;
        ShowItemInfo();
    }

    public void ShowItemInfo()
    {
        if (items.Count > 0)
        { 
            Item current = items[currentItem];
            bool isUsable = current is IUsable;
            InventoryUIManager.instance.UpdateItemInfo(current.itemName, current.itemDescription, isUsable);
        }
    }

    public void UseItem()
    {
        if (items.Count > 0)
        {
            Item current = items[currentItem];
            if (current is IUsable usableItem)
            {
                usableItem.Use();
            }
        }
    }
}
