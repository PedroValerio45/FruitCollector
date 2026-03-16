// Added UI script for chest inv

using System.Collections;
using System.Text;
using NUnit.Compatibility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Text_Chest : MonoBehaviour
{
    [Header("References")]
    private TextMeshProUGUI textMesh;
    private Image image;

    void Start()
    {
        textMesh = GetComponentInChildren<TextMeshProUGUI>();
        image = GetComponent<Image>();
    }

    public void ChangeChestText(Chest chest, bool open) // if true, we just opened the chest
    {
        StringBuilder sb = new StringBuilder();

        // Add title
        sb.AppendLine($"<b>Chest Inventory:</b>\n");

        if (open)
        {
            textMesh.enabled = true;
            image.enabled = true;

            if (chest.inventoryDic_Chest.Count == 0)
            {
                sb.AppendLine($"Empty");
            }
            else
            {
                // Add each inv item
                foreach (var item in chest.inventoryDic_Chest)
                {
                    sb.AppendLine($"- {GetFruitName(item.Key)}: {item.Value}");
                }
            }

            textMesh.text = sb.ToString();
        }
        else
        {
            textMesh.enabled = false;
            image.enabled = false;

            // Add closed text
            // sb.AppendLine("<b>No chest selected</b>");
        }
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