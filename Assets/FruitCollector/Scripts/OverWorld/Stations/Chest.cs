using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Progress;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D), typeof(Animator))]
public sealed class Chest : MonoBehaviour, IInteractable
{
    public static readonly int ANIMATOR_OPENED_HASH = Animator.StringToHash("Opened");

    [SerializeField] private string chestId = "chest_01";

    private EInteractionState InteractionState;
    private Collider2D triggerCollider;
    private Animator animator;
    private PlayerInventory playerInventory; // I'm gonna need the player's inventory
    private UI_Text_Chest chestUI; // I need this for the UI of the chest

    public string ChestId => chestId;

    // New dictionary for stored items
    public Dictionary<string, int> inventoryDic_Chest = new Dictionary<string, int>(); // Save needs this
    private static int maxInvSize_Chest = 128; // Added max inventory size for chest

    private void Awake()
    {
        triggerCollider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        playerInventory = FindFirstObjectByType<PlayerInventory>(); // Added this
        chestUI = FindFirstObjectByType<UI_Text_Chest>(); // Added this

        triggerCollider.isTrigger = true;
    }

    public void Interact(IInteractor interactor)
    {
        if (interactor == null) return;

        Debug.Log($"Chest '{chestId}' interacted by {interactor.Transform.name}");
        
        if (!animator.GetBool(ANIMATOR_OPENED_HASH)) Open();
        else Close();
    }

    private void Open()
    {
        InteractionState = EInteractionState.INTERACTING;
        animator.SetBool(ANIMATOR_OPENED_HASH, true);

        // TODO: Show and apply store logic. // DONE (it tecnically works lol)

        // TEMP: just add items to chest on open, and if player inv is empty then take everything instead
        if (playerInventory.inventoryDic_Player.Count == 0 && inventoryDic_Chest.Count == 0)
        {
            Debug.Log("Both the player and chest inventories are empty");
        }
        
        if (playerInventory.inventoryDic_Player.Count == 0)
        {
            foreach (var chestItem in inventoryDic_Chest) { playerInventory.inventoryDic_Player[chestItem.Key] = chestItem.Value; }
            inventoryDic_Chest.Clear();
            Debug.Log("Player took everything from chest");
        }
        else
        {
            foreach (var invItem in playerInventory.inventoryDic_Player) { inventoryDic_Chest[invItem.Key] = invItem.Value; }
            playerInventory.inventoryDic_Player.Clear();
            Debug.Log("Player stored everything in chest");
        }

        chestUI.ChangeChestText(this, true);

        // DEBUG (unity cant show dictionaries in the inspector bruh)
        foreach (var invItem in playerInventory.inventoryDic_Player) { Debug.Log($"PLAYER Fruit ID: {invItem.Key}, Count: {invItem.Value}"); }
        foreach (var invItem in inventoryDic_Chest) { Debug.Log($" CHEST Fruit ID: {invItem.Key}, Count: {invItem.Value}"); }
    }

    private void Close()
    {
        InteractionState = EInteractionState.FINISHED;
        animator.SetBool(ANIMATOR_OPENED_HASH, false);

        chestUI.ChangeChestText(this, false);
    }

    public EInteractionState GetInteractionState()
    {
        return InteractionState;
    }

    // (UNUSED) Added function to get total amount of items in chest inv
    private int GetInventoryItemsAmount_Chest()
    {
        int totalItems = 0;
        foreach (var item in inventoryDic_Chest)
        {
            totalItems += item.Value;
        }
        return totalItems;
    }
}