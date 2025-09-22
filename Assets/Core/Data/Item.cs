using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public int ID;
    public string itemName;
    public int quantity = 1;
    private TMP_Text quantityText;

    private void Awake()
    {
        quantityText = GetComponentInChildren<TMP_Text>();
        updateQuantityDisplay();
    }

    public void updateQuantityDisplay()
    {
        if (quantityText != null)
        {
            quantityText.text = quantity > 1 ?quantity.ToString() : "";
        }
    }
    public void AddToStack(int amount = 1)
    {
        quantity += amount;
        updateQuantityDisplay();
    }

    public int RemoveStack(int amount = 1)
    {
        int removed = Mathf.Min(amount, quantity);
        quantity -= removed;
        updateQuantityDisplay();
        return removed;
    }

    public GameObject CloneItem(int newQuantity)
    {
        GameObject clone = Instantiate(gameObject);
        Item cloneItem = clone.GetComponent<Item>();
        cloneItem.quantity = newQuantity;
        cloneItem.updateQuantityDisplay();
        return clone;
    }
    public virtual void PickUp()
    {
        Sprite itemIcon = GetComponent<Image>().sprite;
        if (ItemPickupUIController.Instance != null)
        {
            ItemPickupUIController.Instance.ShowItemPickup(itemName, itemIcon);
        }
    }

    public virtual void UseItem()
    {
        Debug.Log($"Using item: {itemName}");
    }
}
