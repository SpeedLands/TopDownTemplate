using System;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SaveData
{
    public Vector3 playerPosition;
    public string mabBoundary;
    public List<InventorySaveData> inventorySaveData;
    public List<InventorySaveData> hotBarSaveData;
    public List<ChestSaveData> chestSaveData;
    public List<QuestProgress> questProgressSaveData;
    public List<string> handinQuestIDs;
}

[System.Serializable]
public class ChestSaveData
{
    public string chestID;
    public bool isOpened;
}