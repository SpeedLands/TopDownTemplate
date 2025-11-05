using UnityEngine;

public class RewardsController : MonoBehaviour
{
    public static RewardsController Instance { get; private set; }

    public void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void GiveQuestRewards(Quest quest)
    {
        if (quest?.rewards == null) return;

        foreach (var reward in quest.rewards)
        {
            switch (reward.type)
            {
                case RewardType.Item:
                    GiveItemReward(reward.rewardID, reward.amount);
                    break;
                case RewardType.Gold:
                    break;
                case RewardType.Experience:
                    break;
                case RewardType.Custom:
                    break;
            }
        }
    }

    public void GiveItemReward(int itemID, int amount)
    {
        var itemPreFab = FindAnyObjectByType<ItemDictionary>()?.GetItemPrefab(itemID);

        if (itemPreFab == null) return;

        for (int i = 0; i < amount; i++)
        {
            if (!InventoryController.Instance.AddItem(itemPreFab))
            {
                GameObject dropItem = Instantiate(itemPreFab, transform.position + Vector3.down, Quaternion.identity);
                dropItem.GetComponent<BounceEffect>().StartBounce();
            }
            else
            {
                itemPreFab.GetComponent<Item>().ShowPopUp();
            }
        }
    }
}
