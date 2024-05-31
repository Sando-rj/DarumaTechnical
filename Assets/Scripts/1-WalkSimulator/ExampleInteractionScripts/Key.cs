using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Example of a Key.
/// A key is an item that can be focused, picked, and interacted with.
/// Holding a key can open a corresponding door.
/// </summary>
public class Key : MonoBehaviour, IFocusable, IPickable, IInteractable {
    public string keyName = "key";

    void OnEnable(){
        EventManager.instance.Focus += FocusManager;
    }

    void OnDisable(){
        EventManager.instance.Focus -= FocusManager;
    }

    #region Focusable Implementation
    public void FocusManager(GameObject focusedObject)
    {
        if(gameObject == focusedObject) {
            Focus();
        }
        else{
            UnFocus();
        }
    }
    
    public void Focus()
    {
        EventManager.instance.Pick += Pick;
        EventManager.instance.Interact += Interact;
    }

    public void UnFocus()
    {
        EventManager.instance.Pick -= Pick;
        EventManager.instance.Interact -= Interact;
    }

    #endregion
 
    #region Interactable Implementation
    public string GetInteractionName()
    {
        return "Use";
    }
    public void Interact(GameObject item)
    {
        return;
    }
    #endregion

    #region Pickable Implementation
    public void Pick(ulong playerId, Vector3 playerHandPosition, GameObject item)
    {
       transform.localPosition = playerHandPosition; 
       transform.GetComponent<Collider>().enabled = false;
       transform.GetComponent<Rigidbody>().useGravity = false;

       EventManager.instance.Drop += Drop;
    }

    public void Drop(ulong playerId, GameObject item)
    {
        transform.GetComponent<Collider>().enabled = true;
        transform.GetComponent<Rigidbody>().useGravity = true;

        EventManager.instance.Drop -= Drop; 
    }
    #endregion

}
