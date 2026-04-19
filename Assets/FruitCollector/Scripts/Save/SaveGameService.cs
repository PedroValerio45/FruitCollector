using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class ChestData
{
    public string chestId;
    public List<string> itemIds = new List<string>();
    public List<int> itemAmounts = new List<int>();
}

[Serializable]
public class DataToSave
{
    // Player Inventory
    public List<string> player_item_ids = new List<string>();
    public List<int> player_item_amounts = new List<int>();

    // Multiple Chests
    public List<ChestData> chests = new List<ChestData>();

    // Fruits in Scene - Store fruit index instead of ID for your spawner
    public List<int> sceneFruits_indices = new List<int>(); // Changed from ID to index
    public List<Vector2> sceneFruits_pos = new List<Vector2>();

    // Other
    public Vector2 playerPos;
    public float playtime_ToBeSaved;
    public string currentDateTime;
}

public sealed class SaveGameService : MonoBehaviour
{
    [Header("References")]
    private PlayerInventory playerInventory;
    private Chest[] allChests;
    private IntervalSpawner intervalSpawner;
    private UI_Text_Msg messageUI;
    private IFruitSelector fruitSelector; // Need this to get fruit by index

    [Header("File Paths")]
    private static string filePath => Path.Combine(Application.persistentDataPath, "saveDataUsed.json");
    private static string filePathbackup => Path.Combine(Application.persistentDataPath, "saveDataBackup.json");

    [Header("Data")]
    private DataToSave data = new DataToSave();
    public float playtime_Current;

    private void Awake()
    {
        playerInventory = FindFirstObjectByType<PlayerInventory>();
        allChests = FindObjectsByType<Chest>(FindObjectsSortMode.None);
        intervalSpawner = FindFirstObjectByType<IntervalSpawner>();
        messageUI = FindFirstObjectByType<UI_Text_Msg>();

        // Get the selector from IntervalSpawner to access fruit by index
        if (intervalSpawner != null)
        {
            var selectorField = intervalSpawner.GetType().GetField("selector",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (selectorField != null)
            {
                fruitSelector = selectorField.GetValue(intervalSpawner) as IFruitSelector;
            }
        }

        LoadGame();
    }

    private void Update()
    {
        playtime_Current += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.F5))
        {
            SaveGame();
        }

        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void GetData()
    {
        // Save Player Inventory
        data.player_item_ids.Clear();
        data.player_item_amounts.Clear();
        foreach (var kvp in playerInventory.inventoryDic_Player)
        {
            data.player_item_ids.Add(kvp.Key);
            data.player_item_amounts.Add(kvp.Value);
        }

        // Save ALL Chests
        data.chests.Clear();
        foreach (var chest in allChests)
        {
            ChestData chestData = new ChestData();
            chestData.chestId = chest.ChestId;

            foreach (var kvp in chest.inventoryDic_Chest)
            {
                chestData.itemIds.Add(kvp.Key);
                chestData.itemAmounts.Add(kvp.Value);
            }

            data.chests.Add(chestData);
        }

        // Save Fruits in Scene - Store their indices instead of IDs
        data.sceneFruits_indices.Clear();
        data.sceneFruits_pos.Clear();

        Fruit[] allFruits = FindObjectsByType<Fruit>(FindObjectsSortMode.None);
        foreach (var fruit in allFruits)
        {
            // Find the index of this fruit in your selector
            int fruitIndex = GetFruitIndex(fruit.Id);
            if (fruitIndex >= 0)
            {
                data.sceneFruits_indices.Add(fruitIndex);
                data.sceneFruits_pos.Add(fruit.transform.position);
            }
        }

        data.playerPos = playerInventory.transform.position;
        data.currentDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        data.playtime_ToBeSaved = playtime_Current;
    }

    private int GetFruitIndex(string fruitId)
    {
        // You need to implement this based on how your IFruitSelector works
        // Assuming your selector has a method to get index by ID
        if (fruitSelector != null)
        {
            // Try to find the index by iterating through available fruits
            for (int i = 0; i < 100; i++) // You might want to store the count somewhere
            {
                var fruitData = GetFruitDataAtIndex(i);
                if (fruitData != null && fruitData.Id == fruitId)
                    return i;
            }
        }
        return -1;
    }

    private FruitData GetFruitDataAtIndex(int index)
    {
        // This depends on your IFruitSelector implementation
        // You might need to expose a method in your selector
        if (fruitSelector != null && fruitSelector is MonoBehaviour behaviour)
        {
            var method = behaviour.GetType().GetMethod("GetFruitData");
            if (method != null)
            {
                return method.Invoke(behaviour, new object[] { index }) as FruitData;
            }
        }
        return null;
    }

    public void SaveGame()
    {
        GetData();

        if (File.Exists(filePath))
        {
            try
            {
                string existingJson = File.ReadAllText(filePath);
                File.WriteAllText(filePathbackup, existingJson);
                Debug.Log($"Backup saved to: {filePathbackup}");
            }
            catch (Exception error)
            {
                Debug.LogError($"Failed to save backup: {error.Message}");
            }
        }

        try
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(filePath, json);
            Debug.Log($"Game saved to: {filePath}");
        }
        catch (Exception error)
        {
            Debug.LogError($"Failed to save game: {error.Message}");
        }
    }

    public void LoadGame()
    {
        if (!File.Exists(filePath))
        {
            Debug.Log("No save file found, creating new one...");
            SaveGame();
            return;
        }

        try
        {
            string json = File.ReadAllText(filePath);
            data = JsonUtility.FromJson<DataToSave>(json);

            // Restore Player Inventory
            playerInventory.inventoryDic_Player.Clear();
            for (int i = 0; i < data.player_item_ids.Count; i++)
            {
                playerInventory.inventoryDic_Player[data.player_item_ids[i]] = data.player_item_amounts[i];
            }

            // Restore ALL Chests
            foreach (var chest in allChests)
            {
                var savedChest = data.chests.FirstOrDefault(c => c.chestId == chest.ChestId);

                if (savedChest != null)
                {
                    chest.inventoryDic_Chest.Clear();
                    for (int i = 0; i < savedChest.itemIds.Count; i++)
                    {
                        chest.inventoryDic_Chest[savedChest.itemIds[i]] = savedChest.itemAmounts[i];
                    }
                }
                else
                {
                    chest.inventoryDic_Chest.Clear();
                }
            }

            // Restore Fruits - Using your existing SpawnFruitFromLoadFile method
            // First, clear existing fruits
            Fruit[] existingFruits = FindObjectsByType<Fruit>(FindObjectsSortMode.None);
            foreach (var fruit in existingFruits)
            {
                Destroy(fruit.gameObject);
            }

            // Spawn saved fruits using the index-based method
            for (int i = 0; i < data.sceneFruits_indices.Count; i++)
            {
                int fruitIndex = data.sceneFruits_indices[i];
                Vector2 position = data.sceneFruits_pos[i];

                if (intervalSpawner != null)
                {
                    // This matches your existing method signature
                    intervalSpawner.SpawnFruitFromLoadFile(fruitIndex, position);
                }
                else
                {
                    Debug.LogWarning("IntervalSpawner not found, can't restore fruits!");
                }
            }

            // Restore player position
            playerInventory.transform.position = data.playerPos;

            playtime_Current = data.playtime_ToBeSaved;

            Debug.Log($"Game loaded! Player position: {data.playerPos}, Playtime: {playtime_Current}s");
            Debug.Log($"Restored {data.sceneFruits_indices.Count} fruits, {data.chests.Count} chests");
        }
        catch (Exception error)
        {
            Debug.LogError($"Failed to load game: {error.Message}");
            TryLoadFromBackup();
        }

        ShowPlayTimeAndTimeSinceLastSession();
    }

    private void TryLoadFromBackup()
    {
        if (File.Exists(filePathbackup))
        {
            try
            {
                string json = File.ReadAllText(filePathbackup);
                data = JsonUtility.FromJson<DataToSave>(json);
                Debug.Log("Loaded from backup successfully!");

                // Re-apply restoration logic here (copy from LoadGame)
                // Consider extracting the restoration to a separate method
            }
            catch (Exception error)
            {
                Debug.LogError($"Failed to load backup: {error.Message}");
            }
        }
    }

    private void ShowPlayTimeAndTimeSinceLastSession()
    {
        int hours = Mathf.FloorToInt(data.playtime_ToBeSaved / 3600);
        int minutes = Mathf.FloorToInt((data.playtime_ToBeSaved % 3600) / 60);

        if (!string.IsNullOrEmpty(data.currentDateTime) && messageUI != null)
        {
            DateTime lastSession = DateTime.Parse(data.currentDateTime);
            TimeSpan timeSinceLast = DateTime.Now - lastSession;

            string message = $"Total playtime: {hours}h {minutes}m\n" +
                $"Last session: {timeSinceLast.Days}d {timeSinceLast.Hours}h {timeSinceLast.Minutes}m ago";

            messageUI.ChangeMessageText(message);
        }
    }

    public Vector2 GetSavedPlayerPosition()
    {
        return data.playerPos;
    }
}