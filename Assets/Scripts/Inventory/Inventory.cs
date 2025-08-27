using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<InventoryItemsBase> items = new List<InventoryItemsBase>();
    public Transform itemsParent;
    public ItemSlots[] itemSlots;



    private void OnValidate()
    {
        // Only run this code if itemsParent is assigned
        if (itemsParent != null)
        {
            // Automatically detect all ItemSlots components within the itemsParent
            itemSlots = itemsParent.GetComponentsInChildren<ItemSlots>();

            // Ensure that changes made in the editor are reflected immediately
            Debug.Log($"{itemSlots.Length} item slots detected and assigned in the Editor.");
        }
        else
        {
            // do nothing
        }
    }
    private void Start()
    {
        // Initialize item slots to exactly 10 slots (horizontal layout)
        itemSlots = itemsParent.GetComponentsInChildren<ItemSlots>();
        Debug.Assert(itemSlots.Length == 10, "Inventory should have exactly 10 slots.");
    }

    public void AddItem(InventoryItemsBase newItem)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i].Item == null)
            {
                itemSlots[i].Item = newItem;
                itemSlots[i].image.sprite = newItem.Icon;
                items.Add(newItem);
                //UpdateUI();
                break;
            }
        }
    }

    public void RemoveItem(InventoryItemsBase itemToRemove)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i].Item == itemToRemove)
            {
                itemSlots[i].Item = null;
                items.Remove(itemToRemove);
                //UpdateUI();
                break;
            }
        }
    }

    private void UpdateUI()
    {
        // Iterate over each slot and update its item
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (i < items.Count)
            {
                itemSlots[i].Item = items[i];
            }
            else
            {
                itemSlots[i].Item = null;
            }
        }
    }
}