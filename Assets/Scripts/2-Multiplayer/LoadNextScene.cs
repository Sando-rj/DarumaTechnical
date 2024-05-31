using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Door))]
public class LoadNextScene : NetworkBehaviour
{
    public string sceneName;
    void Awake(){
        EventManager.instance.Open += RequestLoadScene;
    }

    void RequestLoadScene(Door door){
        if(gameObject.GetComponent<Door>() == door)
            EventManager.instance.LoadSceneRpc(sceneName);
    }
}
