using UnityEngine;

[DisallowMultipleComponent]
public sealed class PlayerInventory : MonoBehaviour, IStorable
{
    // TODO: store items, stack amounts, etc.


    public void Store(IPickable item)
    {
        // TODO: implement inventory rules.
        
        
        // For now, just log.
        Debug.Log($"Picked: {item.DisplayName} ({item.Id})");
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null) return;

        if (!other.TryGetComponent<IPickable>(out var pickable))
            return;

        // The pickable decides what happens on pick.
        pickable.Pick(this);
    }
}
