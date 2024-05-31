using System.IO;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Lobby script: Manage the network roles launch and asset bundling loading.
/// </summary>
public class Connect : MonoBehaviour
{
    public NetworkManager NetManager;
    public GameObject Lobby;
    public string sceneAssetBundle = "materialbundle";

    public string startSceneName = "TestRoomA";

    void Update(){
        if(Input.GetKeyDown(KeyCode.H))
            StartHost();
        else if(Input.GetKeyDown(KeyCode.C))
            StartClient();
    } 

    void StartHost(){
        LoadSceneAssets();
        Lobby.SetActive(false);
        NetManager.StartHost();
        SceneLoadingController.instance.LoadScene(startSceneName);
    }

    void StartClient(){
        LoadSceneAssets();
        Lobby.SetActive(false);
        NetManager.StartClient();
    }

    void LoadSceneAssets(){
        Debug.Log(Path.Combine(Application.streamingAssetsPath, sceneAssetBundle));
        AssetBundle localAssetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, sceneAssetBundle));

        if(localAssetBundle == null){
            Debug.LogError("Failed to load assets");
            return;
        }
    }
}
