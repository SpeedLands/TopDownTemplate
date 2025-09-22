using System;
using Unity.VisualScripting;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    public bool IsOpened { get; set; }
    public String ChestID { get; set; }
    public GameObject itemPrefab;
    public Sprite openedSprite;
    void Start()
    {
        ChestID ??= GlobalHelper.GenerateUniqueID(gameObject);
    }
    public bool CanInteract()
    {
        return !IsOpened;
    }

    public void Interact()
    {
        if (!CanInteract()) return;
        OpenChest();
    }

    private void OpenChest()
    {
        SetOpened(true);
        SoundEffectManager.Play("Chest");
        if (itemPrefab != null)
        {
            GameObject dropItem = Instantiate(itemPrefab, transform.position + Vector3.down, Quaternion.identity);
            dropItem.GetComponent<BounceEffect>()?.StartBounce();
        }
    }

    public void SetOpened(bool opened)
    {
        if (IsOpened = opened)
        {
            GetComponent<SpriteRenderer>().sprite = openedSprite;
        }
    } 
}
