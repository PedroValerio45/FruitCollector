using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D), typeof(Animator))]
public sealed class Chest : MonoBehaviour, IInteractable
{
    public static readonly int ANIMATOR_OPENED_HASH = Animator.StringToHash("Opened");

    [SerializeField] private string chestId = "chest_01";

    private EInteractionState InteractionState;
    private Collider2D triggerCollider;
    private Animator animator;

    public string ChestId => chestId;


    private void Awake()
    {
        triggerCollider = GetComponent<Collider2D>();
        triggerCollider.isTrigger = true;

        animator = GetComponent<Animator>();
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

        // TODO: Show and apply store logic.


    }


    private void Close()
    {
        InteractionState = EInteractionState.FINISHED;
        animator.SetBool(ANIMATOR_OPENED_HASH, false);
    }


    public EInteractionState GetInteractionState()
    {
        return InteractionState;
    }
}