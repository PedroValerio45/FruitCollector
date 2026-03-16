using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class PlayerInventory : MonoBehaviour, IStorable
{
    // TODO: store items, stack amounts, etc. // DONE
    public Dictionary<string, int> inventoryDic_Player = new Dictionary<string, int>(); // Chest and save need this
    public static int maxInvSize_Player = 32; // Added max inventory size for player // Chest needs this

    public void Store(IPickable item)
    {
        // TODO: implement inventory rules. // DONE
        if (inventoryDic_Player.ContainsKey(item.Id)) { inventoryDic_Player[item.Id]++; }
        else { inventoryDic_Player[item.Id] = 1; }

        // DEBUG (unity cant show dictionaries in the inspector bruh)
        foreach (var invItem in inventoryDic_Player)
        {
            Debug.Log($"Fruit ID: {invItem.Key}, Count: {invItem.Value}");
        }
        // Debug.Log($"Picked: {item.DisplayName} ({item.Id})");
    }

    // Added function to get total amount of items in player inv
    public int GetInventoryItemsAmount_Player() // Chest needs this
    {
        int totalItems = 0;
        foreach (var item in inventoryDic_Player)
        {
            totalItems += item.Value;
        }
        return totalItems;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null) return;

        if (!other.TryGetComponent<IPickable>(out var pickable))
            return;
        else Debug.Log("has pickable");

        // Check maxStack before picking up item
        if (inventoryDic_Player.ContainsKey(pickable.Id) && inventoryDic_Player[pickable.Id] < pickable.MaxStack
            || GetInventoryItemsAmount_Player() < maxInvSize_Player) { pickable.Pick(this); }
        else { Debug.Log($"Cannot pick - stack full! (Or it's fine and we just don't have any of this type) Max: {pickable.MaxStack}"); }
    }
}
