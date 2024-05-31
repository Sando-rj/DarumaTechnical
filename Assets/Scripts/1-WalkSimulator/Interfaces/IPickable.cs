using UnityEngine;

/// <summary>
/// Enables focusable items to be picked.
/// </summary>
public interface IPickable {
    public void Pick(ulong playerId, Vector3 playerHand, GameObject item);
    public void Drop(ulong playerId, GameObject item);
}