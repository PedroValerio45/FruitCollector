using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class PlayerInventory : MonoBehaviour, IStorable
{
    // TODO: store items, stack amounts, etc. // DONE
    public Dictionary<string, int> inventoryDic_Player = new Dictionary<string, int>(); // Chest and save need this

    public void Store(IPickable item)
    {
        // TODO: implement inventory rules. // DONE
        if(inventoryDic_Player.ContainsKey(item.Id)) { inventoryDic_Player[item.Id]++; }
        else { inventoryDic_Player[item.Id] = 1; }

        // DEBUG (unity cant show dictionaries in the inspector bruh)
        foreach (var invItem in inventoryDic_Player)
        {
            Debug.Log($"Fruit ID: {invItem.Key}, Count: {invItem.Value}");
        }
        // Debug.Log($"Picked: {item.DisplayName} ({item.Id})");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null) return;

        if (!other.TryGetComponent<IPickable>(out var pickable))
            return;
        else Debug.Log("has pickable");

            // The pickable decides what happens on pick.
            pickable.Pick(this);
    }
}
