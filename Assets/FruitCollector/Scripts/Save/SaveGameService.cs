using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement; // added for debug

public class DataToSave
{
    [Header("Dictionaries")]
    public Dictionary<string, int> inventoryDic_Save_Player = new Dictionary<string, int>();
    public Dictionary<string, int> inventoryDic_Save_Chest = new Dictionary<string, int>();
    public Dictionary<int, Vector2> fruitsInScene = new Dictionary<int, Vector2>();

    [Header("Other")]
    public Vector2 playerPos;
    public float playtime_ToBeSaved;
    public string currentDateTime;
}

public sealed class SaveGameService : MonoBehaviour
{
    // Added a bunch of variables and references
    [Header("References")]
    private PlayerInventory playerInventory;
    private Chest chest;
    private IntervalSpawner intervalSpawner;
    private UI_Text_Msg messageUI;

    [Header("File Paths")]
    private static string filePath => Path.Combine(Application.persistentDataPath, "saveDataUsed.json"); // "=>" is "readOnly"
    private static string filePathbackup => Path.Combine(Application.persistentDataPath, "saveDataBackup.json"); // "=>" is "readOnly"

    [Header("Data")]
    private DataToSave data = new DataToSave();
    public float playtime_Current;

    // Added awake function
    private void Awake()
    {
        playerInventory = FindFirstObjectByType<PlayerInventory>();
        chest = FindFirstObjectByType<Chest>();
        intervalSpawner = FindFirstObjectByType<IntervalSpawner>();
        messageUI = FindFirstObjectByType<UI_Text_Msg>();

        LoadGame();
    }

    // Added update function
    private void Update()
    {
        playtime_Current += Time.deltaTime;

        // DEBUG
        if (Input.GetKeyDown(KeyCode.Keypad0)) { SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    }

    // Added function to get current data to be saved
    public void GetData()
    {
        data.inventoryDic_Save_Player = new Dictionary<string, int>(playerInventory.inventoryDic_Player);

        data.inventoryDic_Save_Chest = new Dictionary<string, int>(chest.inventoryDic_Chest);

        data.fruitsInScene = new Dictionary<int, Vector2>();
        Fruit[] allFruits = FindObjectsByType<Fruit>(FindObjectsSortMode.None);
        for (int i = 0; i < allFruits.Length; i++)
        {
            // Use the fruit's instance ID as a unique key, and position as value
            int fruitId = allFruits[i].gameObject.GetInstanceID();
            Vector2 fruitPosition = allFruits[i].transform.position;
            data.fruitsInScene.Add(fruitId, fruitPosition);

            // Debug log to see what's being saved
            Debug.Log($"Saving fruit: {allFruits[i].DisplayName} at position {fruitPosition} with ID {fruitId}");
        }

        data.playerPos = playerInventory.transform.position;

        data.currentDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        data.playtime_ToBeSaved = playtime_Current;
    }

    public void SaveGame()
    {
        // TODO: Implement game saving logic here. // DONE

        // Create or overwrite backup file with old data if save file already exists
        if (File.Exists(filePath))
        {
            try
            {
                string json = JsonUtility.ToJson(data, true);
                File.WriteAllText(filePathbackup, json);
                Debug.Log($"Backup data saved to: {filePathbackup}");
            }
            catch (Exception error)
            {
                Debug.LogError($"Failed to save backup data: {error.Message}");
            }
        }

        GetData();

        // Create or overwrite save file
        try
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(filePath, json);
            Debug.Log($"Game data saved to: {filePath}");
        }
        catch (Exception error)
        {
            Debug.LogError($"Failed to save game data: {error.Message}");
        }

        // DEBUG
        Debug.Log("Save finished");
    }

    // Added load function
    public void LoadGame()
    {
        if (!File.Exists(filePath))
        {
            SaveGame();
        }
        else
        {
            try
            {
                string json = File.ReadAllText(filePath);
                data = JsonUtility.FromJson<DataToSave>(json);
            }
            catch (Exception error)
            {
                Debug.LogError($"Failed to load game data: {error.Message}");
            }

            // Apply to game
            foreach (var fruit in data.fruitsInScene)
            {
                intervalSpawner.SpawnFruitFromLoadFile(fruit.Key, fruit.Value);
            }
            playtime_Current = data.playtime_ToBeSaved;

            Debug.Log("Loaded save");
        }

        ShowPlayTimeAndTimeSinceLastSession();

        // DEBUG
        Debug.Log("Load finished");
    }

    private void ShowPlayTimeAndTimeSinceLastSession()
    {
        // Convert seconds to hours and minutes
        int hours = Mathf.FloorToInt(data.playtime_ToBeSaved / 3600);
        int minutes = Mathf.FloorToInt((data.playtime_ToBeSaved % 3600) / 60);

        DateTime lastSession = DateTime.Parse(data.currentDateTime);
        TimeSpan timeSinceLast = DateTime.Now - lastSession;

        string message = $"Tiempo total de juego: {hours} horas y {minutes} minutos\n" +
            $"Hace {timeSinceLast.Days} días, {timeSinceLast.Hours} horas y {timeSinceLast.Minutes}minutos desde tu última sesión de juego";

        messageUI.ChangeMessageText(message); // Send to UI_Text_Msg
    }

    public Vector2 GetSavedPlayerPosition()
    {
        return data.playerPos;
    }
}