using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// Example of UI interaction.
/// Script to show interaction feedback and feedforward messages.
/// </summary>
public class UserFeedback : MonoBehaviour
{
    public TMP_Text textField;
    string displayedMessage = "";
    string emergencyMessage = "";
    float emergencyDisplayTime = 0.7f;

    void OnEnable(){
        EventManager.instance.Focus += FocusFeedback;
        EventManager.instance.Unlock += UnlockMessage;
        EventManager.instance.Failure += EmergencyMessage;
    }

    void OnDisable(){
        EventManager.instance.Focus -= FocusFeedback;
        EventManager.instance.Unlock -= UnlockMessage;
        EventManager.instance.Failure -= EmergencyMessage;
    }

    void FocusFeedback(GameObject focusedObject){
        string displayedText = "";
        
        if(focusedObject?.GetComponent<IPickable>() != null)
            displayedText += "[G] Grab " + focusedObject.name + "\n";
        if(focusedObject?.GetComponent<IInteractable>() != null)
            displayedText += "[E] Use " + focusedObject.name + "\n";

        displayedMessage = displayedText;
        this.textField.text = displayedMessage;
    }

    void EmergencyMessage(string message){
        this.emergencyMessage = message;
        StartCoroutine(InterruptDisplay());
    }

    void UnlockMessage(Door door){
        this.emergencyMessage = door.name + " Unlocked !";
        StartCoroutine(InterruptDisplay());
    }

    IEnumerator InterruptDisplay(){
        float currentTime = Time.time;
        this.textField.text = emergencyMessage;

        while(currentTime + emergencyDisplayTime > Time.time){
            yield return null;
        }
        
        this.emergencyMessage = "";
        this.textField.text = displayedMessage;
    }
}
