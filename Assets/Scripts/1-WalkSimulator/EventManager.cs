using System;
using NaughtyAttributes;
using NaughtyAttributes.Test;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Event Manager that triggers all actions and inform all listening scripts.
/// Contains Local actions played only on the local player, Global actions informing a change in the system state and RPC calls for those actions.
/// Local actions contain: Focus, Interact and Failure.
/// 
/// Contains Actions Rpc for Server and Clients
/// </summary>
public class EventManager : NetworkBehaviour
{
    public static EventManager instance;
    public event Action<GameObject> Focus;
    public event Action<ulong, Vector3, GameObject> Pick;
    public event Action<GameObject> Interact;
    public event Action<ulong, GameObject> Drop;
    public event Action<Door> Unlock;
    public event Action<Door> Open;
    public event Action<Door> Close;
    public event Action<string> Failure;
    public event Action EndGame;

    void Awake(){
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    #region Local Calls
    public void OnFocus(GameObject gameObject){
        Focus?.Invoke(gameObject);
    }

    public void OnInteract(GameObject item){
        Interact?.Invoke(item);
    }

    public void OnFailure(string message){
        Failure?.Invoke(message);
    }

    public void OnEndGame(){
        OnEndGameRpc();
    }
    #endregion 

    #region Global Actions
    public void OnPick(ulong playerId, Vector3 pickPosition, GameObject item){
        OnPickServerRpc(playerId, pickPosition, item.name);
    }
    
    public void OnDrop(ulong playerId, GameObject item){
        OnDropServerRpc(playerId, item.name);
    }

    public void OnUnlock(Door door){
        OnUnlockServerRpc(door.gameObject.name);
    }

    public void OnOpen(Door door){
        OnOpenServerRpc(door.gameObject.name);
    }

    public void OnClose(Door door){
        OnCloseServerRpc(door.gameObject.name);
    }
    #endregion

    #region Network Client Action Calls
    [Rpc(SendTo.ClientsAndHost)]
    public void OnPickRpc(ulong playerId, Vector3 pickerPosition, string itemName){
        GameObject foundItem = GameObject.Find(itemName);
        
        Pick?.Invoke(playerId, pickerPosition, foundItem);
    }

    [Rpc(SendTo.ClientsAndHost)]   
    public void OnDropRpc(ulong playerId, string itemName){
        GameObject foundItem = GameObject.Find(itemName);

        Drop?.Invoke(playerId, foundItem);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void OnUnlockRpc(string doorName){
        Door foundDoor = GameObject.Find(doorName)?.GetComponent<Door>();
        Unlock?.Invoke(foundDoor);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void OnOpenRpc(string doorName){
        Door foundDoor = GameObject.Find(doorName)?.GetComponent<Door>();
        Open?.Invoke(foundDoor);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void OnCloseRpc(string doorName){
        Door foundDoor = GameObject.Find(doorName)?.GetComponent<Door>();
        Close?.Invoke(foundDoor);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void OnEndGameRpc(){
        EndGame?.Invoke();
    }
    #endregion

    #region Network Server Action Calls
    [Rpc(SendTo.Server)]
    public void OnPickServerRpc(ulong playerId, Vector3 pickerPosition, string itemName){
        GameObject foundItem = GameObject.Find(itemName);
        FPSController[] controllers = FindObjectsByType<FPSController>(FindObjectsSortMode.None);

        foreach(FPSController component in controllers){
            if(component.GetPlayerId() == playerId)
                foundItem.transform.parent = component.transform;
        }

        OnPickRpc(playerId, pickerPosition, itemName);
    }

    [Rpc(SendTo.Server)]
    public void OnDropServerRpc(ulong playerId, string itemName){
        GameObject foundItem = GameObject.Find(itemName);
        foundItem.transform.parent = null;

        OnDropRpc(playerId, itemName);
    }

    [Rpc(SendTo.Server)]
    public void OnUnlockServerRpc(string doorName){
        OnUnlockRpc(doorName);
    }

    [Rpc(SendTo.Server)]
    public void OnOpenServerRpc(string doorName){
        OnOpenRpc(doorName);
    }

    [Rpc(SendTo.Server)]
    public void OnCloseServerRpc(string doorName){
        OnCloseRpc(doorName);
    }

    [Rpc(SendTo.Server)]
    public void LoadSceneRpc(string sceneName){
        OnLoadSceneRpc(sceneName);
    }

    public void OnLoadSceneRpc(string sceneName){
        SceneLoadingController.instance.LoadScene(sceneName);   
    }
    #endregion
}
