using System.Collections; 
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

public class SaveController : MonoBehaviour
{
    [Header("UI Transiciones")]
    public CanvasGroup transitionGroup;
    public float fadeDuration = 1.0f;
    private string saveLocation;
    private InventoryController inventoryController;
    private HotBarController hotBarController;
    private Chest[] chests;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    IEnumerator Start()
    {
        if (transitionGroup != null)
        {
            transitionGroup.alpha = 1f; // Opacidad al máximo
            transitionGroup.blocksRaycasts = true; // Bloquear clicks del mouse
        }

        yield return null; 

        yield return new WaitForSeconds(0.5f);

        InitializeComponents();
        LoadGame();

        if (transitionGroup != null)
        {
            float timer = 0f;
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                // Va bajando el alpha de 1 a 0 poco a poco
                transitionGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
                yield return null;
            }

            // Aseguramos que quede invisible y desbloqueamos el mouse
            transitionGroup.alpha = 0f;
            transitionGroup.blocksRaycasts = false;
        }
    }

    public void InitializeComponents()
    {
        saveLocation = Path.Combine(Application.persistentDataPath, "saveData.json");
        inventoryController = FindAnyObjectByType<InventoryController>();
        hotBarController = FindAnyObjectByType<HotBarController>();
        chests = FindObjectsByType<Chest>(FindObjectsSortMode.None);
    }

    public void SaveGame()
    {
        SaveData saveData = new SaveData
        {
            playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position,
            mabBoundary = FindAnyObjectByType<CinemachineConfiner2D>().BoundingShape2D.name,
            inventorySaveData = inventoryController.GetInventoryItems(),
            hotBarSaveData = hotBarController.GetHotBarItems(),
            chestSaveData = GetChestsStates(),
            questProgressSaveData = QuestController.Instance.activeQuests,
            handinQuestIDs = QuestController.Instance.handingQuestIDs
        };

        File.WriteAllText(saveLocation, JsonUtility.ToJson(saveData));
    }

    private List<ChestSaveData> GetChestsStates()
    {
        List<ChestSaveData> chestsStates = new List<ChestSaveData>();

        foreach (Chest chest in chests)
        {
            ChestSaveData chestSaveData = new ChestSaveData
            {
                chestID = chest.ChestID,
                isOpened = chest.IsOpened
            };
            chestsStates.Add(chestSaveData);
        }
        return chestsStates;
    }

    public void LoadGame()
    {
        if (File.Exists(saveLocation))
        {
            SaveData saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(saveLocation));
            GameObject.FindGameObjectWithTag("Player").transform.position = saveData.playerPosition;
            PolygonCollider2D saveMapBoundary = GameObject.Find(saveData.mabBoundary).GetComponent<PolygonCollider2D>();
            FindAnyObjectByType<CinemachineConfiner2D>().BoundingShape2D = saveMapBoundary;
            MapController_Manual.Instance?.HighLightArea(saveData.mabBoundary);
            MapController_Dynamic.Instance?.GenerateMap(saveMapBoundary);
            inventoryController.SetInventoryItems(saveData.inventorySaveData);
            hotBarController.SetHotBarItems(saveData.hotBarSaveData);
            LoadChestsStates(saveData.chestSaveData);
            QuestController.Instance.LoadQuestProgress(saveData.questProgressSaveData);
            QuestController.Instance.handingQuestIDs = saveData.handinQuestIDs;
        }
        else
        {
            SaveGame();
            inventoryController.SetInventoryItems(new List<InventorySaveData>());
            hotBarController.SetHotBarItems(new List<InventorySaveData>());
            MapController_Dynamic.Instance?.GenerateMap();
        }
    }

    private void LoadChestsStates(List<ChestSaveData> chestStates)
    {
        foreach (Chest chest in chests)
        {
            ChestSaveData chestSaveData = chestStates.FirstOrDefault(c => c.chestID == chest.ChestID);
            if (chestSaveData != null)
            {
                chest.SetOpened(chestSaveData.isOpened);
            }
        }
    }
}
