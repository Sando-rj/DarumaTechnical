using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controller to prevent overloading scenes
/// </summary>
public class SceneLoadingController : NetworkBehaviour
{
    public string initialScene = "TestRoomA";
    public static SceneLoadingController instance;
    List<string> sceneLoaded = new();

    public void Awake(){
        instance = this;
        sceneLoaded.Add(initialScene);
        DontDestroyOnLoad(this.gameObject);
    }

    //Scene Loading must be done only on server
    public override void OnNetworkSpawn(){
        if(!IsServer){
            this.enabled = false;
        }
    }

    public void LoadScene(string sceneName){
        if(!sceneLoaded.Contains(sceneName)){
            NetworkManager.SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            this.sceneLoaded.Add(sceneName);
        }
    }
}
