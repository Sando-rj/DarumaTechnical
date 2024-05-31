using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Example of a trigger for a door as a button.
/// </summary>
public class TriggerButton : MonoBehaviour, IFocusable, IInteractable
{   
    public Door triggerDoor;
    public bool triggerEnd = false;
    public Transform ButtonTransform;
    float animationTime = 0.5f;
    float lowerHeight = 2.0f;

    void OnEnable(){
        EventManager.instance.Focus += FocusManager; 
    }
    void OnDisable(){
        EventManager.instance.Focus -= FocusManager;
    }

#region Focus Interface
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

#region Interaction Interface
    public string GetInteractionName()
    {
        return "Push Button";
    }

    public void Interact(GameObject item)
    {
        if(triggerDoor) EventManager.instance.OnUnlock(triggerDoor);
        if(triggerEnd) EventManager.instance.OnEndGame();
        StartCoroutine(PushButton());
    }
#endregion

    IEnumerator PushButton(){
        float midTime = Time.time + animationTime / 2;
        float finalTime = Time.time + animationTime;
        Vector3 initialPosition = ButtonTransform.localPosition;
        Vector3 finalPosition = ButtonTransform.localPosition + new Vector3(0, - lowerHeight, 0);
        
        while(midTime > Time.time){
            float delta = (finalTime - Time.time) / animationTime;
            Vector3 movingPosition = Vector3.Lerp(initialPosition, finalPosition, 1 - delta);
            ButtonTransform.localPosition = movingPosition; 
            yield return null;
        }

        while(finalTime > Time.time){
            float delta = (finalTime - Time.time) / animationTime;
            Vector3 movingPosition = Vector3.Lerp(finalPosition, initialPosition, 1 - delta);
            ButtonTransform.localPosition = movingPosition; 
            yield return null;
        }

        ButtonTransform.localPosition = initialPosition;
    }
}
