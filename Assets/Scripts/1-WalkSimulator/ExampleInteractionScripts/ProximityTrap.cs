using System.Collections;
using UnityEngine;

/// <summary>
/// Example of a proximity interaction.
/// </summary>
public class ProximityTrap : MonoBehaviour
{
    float animationTime = 1.0f;
    float raisedHeight = 3.0f;
    int numberOfPlayers = 0;

    void OnTriggerEnter(Collider collider){
        if(collider.GetComponent<FPSController>()){        
            numberOfPlayers += 1;
            if(numberOfPlayers == 1) OpenTrap();
        }
    }

    void OnTriggerExit(Collider collider){
        if(collider.GetComponent<FPSController>()){
            numberOfPlayers -= 1;
            if(numberOfPlayers == 0) CloseTrap();
        }
    }

    void OpenTrap(){
        StartCoroutine(RaiseStructure());
    }

    void CloseTrap(){
        StartCoroutine(LowerStructure());
    }

    IEnumerator RaiseStructure(){
        float finalTime = Time.time + animationTime;
        Vector3 initialPosition = transform.position;
        Vector3 finalPosition = initialPosition + new Vector3(0, raisedHeight, 0);

        while(finalTime > Time.time){
            float delta = (finalTime - Time.time) / animationTime;
            Vector3 movingPosition = Vector3.Lerp(initialPosition, finalPosition, 1 - delta);
            transform.position = movingPosition;
            yield return null;
        }

        transform.position = finalPosition;
    }

    IEnumerator LowerStructure(){
        float finalTime = Time.time + animationTime;
        Vector3 initialPosition = transform.position;
        Vector3 finalPosition = initialPosition + new Vector3(0, -raisedHeight, 0);

        while(finalTime > Time.time){
            float delta = (finalTime - Time.time) / animationTime;
            Vector3 movingPosition = Vector3.Lerp(initialPosition, finalPosition, 1 - delta);
            transform.position = movingPosition;
            yield return null;
        }

        transform.position = finalPosition;
    }
}
