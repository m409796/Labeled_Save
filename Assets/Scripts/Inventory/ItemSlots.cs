using UnityEngine;
using UnityEngine.UI;

public class ItemSlots : MonoBehaviour
{
    [SerializeField] public Image image; 
    [SerializeField] public string itemName;
    public InventoryItemsBase Item;

    private void Start()
    {
        UpdateImageState();
    }

    private void Update()
    {
        UpdateImageState();
    }

    private void UpdateImageState()
    {
        if (Item == null)
        {
            image.enabled = false;
        }
        else
        {
            image.enabled = true;
            image.sprite = Item.Icon;
        }
    }
}
