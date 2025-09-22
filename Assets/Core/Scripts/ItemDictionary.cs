using System.Collections.Generic;
using UnityEngine;

public class ItemDictionary : MonoBehaviour
{
    public List<Item> itemsPrefabs;
    public Dictionary<int, GameObject> itemDictionary;

    private void Awake()
    {
        itemDictionary = new Dictionary<int, GameObject>();
        for (int i = 0; i < itemsPrefabs.Count; i++)
        {
            if (itemsPrefabs[i] != null)
            {
                itemsPrefabs[i].ID = i + 1;
            }
        }

        foreach (Item item in itemsPrefabs)
        {
            itemDictionary[item.ID] = item.gameObject;
        }
    }

    public GameObject GetItemPrefab(int itemID)
    {
        itemDictionary.TryGetValue(itemID, out GameObject prefab);
        if (prefab == null)
        {
            Debug.LogWarning($"Item with ID {itemID} not found in the dictionary.");
            return null;
        }
        else
        {
            return prefab;
        }
    }
}
