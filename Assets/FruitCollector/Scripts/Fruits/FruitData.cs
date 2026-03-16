using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Fruit Picker/Fruits/Fruit Data", fileName = "FruitData")]
public sealed class FruitData : ScriptableObject
{
    private const string KEY_NAME = "fruit:";

    [SerializeField] private string id;
    [SerializeField] private string displayName;
    [SerializeField] private Sprite sprite;
    // Added max stack ammount
    [SerializeField] private int maxStack;

    public string Id => id;
    public string DisplayName => displayName;
    public Sprite Sprite => sprite;

    // Added this
    public int MaxStack => maxStack;

    private void OnValidate()
    {
        if (string.IsNullOrWhiteSpace(id))
            id = KEY_NAME + name.Trim().ToLower();

        if (string.IsNullOrWhiteSpace(displayName))
            displayName = name.Trim();

        // Added fallback
        if (maxStack == 0)
            maxStack = 16; // Default 
    }
}