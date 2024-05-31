using System.Collections;
using NaughtyAttributes;
using UnityEngine;

/// <summary>
/// Door example implementation.
/// A door has 3 states: Locked, Closed, Opened.
/// A door can match a key that unlocks it, and Open/Close/Unlock actions subscribes to the respective Events.
/// Interaction is done inside the Interact() function.
/// </summary>
public class Door : MonoBehaviour, IFocusable, IInteractable
{
    public DoorState doorState = DoorState.Opened; 
    [ShowIf("doorState", DoorState.Locked)]
    public Key matchingKey = null;
    private float animationTime = 1.0f;

    public enum DoorState {
        Locked,
        Opened,
        Closed
    };

    void OnEnable(){
        EventManager.instance.Focus += FocusManager;
        EventManager.instance.Unlock += Unlock;
        EventManager.instance.Open += Open;
        EventManager.instance.Close += Close;
    }

    void OnDisable(){
        EventManager.instance.Focus -= FocusManager;
        EventManager.instance.Unlock -= Unlock;
        EventManager.instance.Open -= Open;
        EventManager.instance.Close -= Close;
    }

    #region Focus Implementation
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
        EventManager.instance.Interact += Interact;
    }

    public void UnFocus()
    {
        EventManager.instance.Interact -= Interact;
    }
    #endregion
  
    #region Interactable Implementation
    public string GetInteractionName()
    {
        return "Open";
    }

    public void Interact(GameObject item)
    {
        if(doorState == DoorState.Locked)
            UnlockWith(item);
        else if(doorState == DoorState.Closed)
            EventManager.instance.OnOpen(this);
        else if(doorState == DoorState.Opened)
            EventManager.instance.OnClose(this);   
    }
    #endregion

    #region Actions
    public void Open(Door door){
        if(door != this) return;

        StartCoroutine(OpeningDoorAnimation());
        this.doorState = DoorState.Opened;
    }

    public void Close(Door door){
        if(door != this) return;
        
        StartCoroutine(ClosingDoorAnimation());
        this.doorState = DoorState.Closed;
    }

    public void UnlockWith(GameObject item){
        Key isKey = item?.GetComponent<Key>();
        if(isKey == this.matchingKey){
            EventManager.instance.OnUnlock(this);
        }
        else{
            EventManager.instance.OnFailure("Door Locked");
        }
    }

    public void Unlock(Door door){
        if(door != this) return;
        
        this.doorState = DoorState.Closed;
    }
    #endregion

    #region Animation
    IEnumerator OpeningDoorAnimation(){
        float finalTime = Time.time + animationTime;
        Quaternion initialRotation = transform.rotation;
        Quaternion finalRotation = transform.rotation * Quaternion.Euler(0, -90, 0);
        
        while(finalTime > Time.time){
            float delta = (finalTime - Time.time) / animationTime;
            Quaternion movingRotation = Quaternion.Lerp(initialRotation, finalRotation, 1 - delta);
            transform.rotation = movingRotation; 
            yield return null;
        }

        transform.rotation = finalRotation;    
    }
    IEnumerator ClosingDoorAnimation(){
        float finalTime = Time.time + animationTime;
        Quaternion initialRotation = transform.rotation;
        Quaternion finalRotation = transform.rotation * Quaternion.Euler(0, 90, 0);
        
        while(finalTime > Time.time){
            float delta = (finalTime - Time.time) / animationTime;
            Quaternion movingRotation = Quaternion.Lerp(initialRotation, finalRotation, 1 - delta);
            transform.rotation = movingRotation; 
            yield return null;
        }

        transform.rotation = finalRotation;   
    }
    #endregion
}
