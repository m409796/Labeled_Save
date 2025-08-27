using UnityEngine;

public class InventoryItemsBase : ScriptableObject
{
    public string ItemName;
    public string Description;
    public Sprite Icon;
    public int ID;

    // Add more properties as needed
    // For example, item type, weight, value, etc.

    // You might also want to add methods for item-specific actions
    //public virtual void Use()
    //{
    //    // Define what happens when the item is used
    //    Debug.Log("Using item: " + ItemName);
    //}
}
