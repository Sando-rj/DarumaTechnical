using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Script to setup a player belonging to its client
/// </summary>
public class FPSNetworkStart : MonoBehaviour
{
    public NetworkObject networkProperty;
    public List<Behaviour> PlayerScripts;
    private bool isSetup = false;

    void Update(){
        // If this is a local player, activate all relevant scripts
        if(!isSetup && networkProperty.IsLocalPlayer){
            foreach(Behaviour component in PlayerScripts){
                component.enabled = true;
            }
            isSetup = true;
        }
    }
}
