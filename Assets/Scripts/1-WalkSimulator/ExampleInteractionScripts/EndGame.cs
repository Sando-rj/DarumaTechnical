using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// End the demo.
/// </summary>
public class EndGame : MonoBehaviour
{
    void OnEnable(){
        EventManager.instance.EndGame += ShutDownGame;
    }

    void OnDisable(){
        EventManager.instance.EndGame -= ShutDownGame;
    }

    void ShutDownGame(){
        Application.Quit();
    }
}
