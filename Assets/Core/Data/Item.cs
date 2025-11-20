using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public int ID;
    public string itemName;
    public int quantity = 1;
    private TMP_Text quantityText;
    private Collider2D itemColider;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        quantityText = GetComponentInChildren<TMP_Text>();
        updateQuantityDisplay();
        itemColider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Collect()
    {
        if (itemColider != null)
        {
            itemColider.enabled = false;
        }
        else
        {
            Debug.LogWarning("Se llamó a Collect() en un item sin Collider2D.");
        }

        StartCoroutine(CollectAnimationRoutine());
    }

    private IEnumerator CollectAnimationRoutine()
    {
        if (spriteRenderer == null)
        {
            Destroy(gameObject);
            yield break;
        }

        float duration = 0.3f;
        float timer = 0f;
        Vector3 origialScale = transform.localScale;
        Vector3 targetScale = origialScale * 1.5f;
        Color originalColor = spriteRenderer.color;
        Color targetColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

        while (timer < duration)
        {
            transform.localScale = Vector3.Lerp(origialScale, targetScale, timer / duration);
            spriteRenderer.color = Color.Lerp(originalColor, targetColor, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
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
    public virtual void ShowPopUp()
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
