// Added UI script for player inv

using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class UI_Text_Inv : MonoBehaviour
{
    [Header("References")]
    private TextMeshProUGUI textMesh;
    private PlayerInventory playerInventory;

    void Start()
    {
        textMesh = GetComponentInChildren<TextMeshProUGUI>();
        playerInventory = FindFirstObjectByType<PlayerInventory>();
    }

    void Update()
    {
        StringBuilder sb = new StringBuilder();

        // Add title
        sb.AppendLine($"<b>Player's Inventory:</b>\n");

        if (playerInventory.inventoryDic_Player.Count == 0)
        {
            sb.AppendLine($"Empty");
        }
        else
        {
            // Add each inv item
            foreach (var item in playerInventory.inventoryDic_Player)
            {
                sb.AppendLine($"- {GetFruitName(item.Key)}: {item.Value}");
            }
        }

        textMesh.text = sb.ToString();
    }

    private string GetFruitName(string fruitId)
    {
        FruitData[] allFruitData = Resources.FindObjectsOfTypeAll<FruitData>();
        foreach (var fruitData in allFruitData)
        {
            if (fruitData.Id == fruitId) { return fruitData.DisplayName; }
        }

        return "Error";
    }
}