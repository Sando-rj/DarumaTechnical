using UnityEngine;

/// <summary>
/// Enable interaction on focusable items.
/// </summary>
public interface IInteractable {
    public string GetInteractionName();
    public void Interact(GameObject item);
}