using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class photonConnect : MonoBehaviour {
    public string versionName = "0.1";

    public void Awake()
    {
        PhotonNetwork.ConnectUsingSettings(versionName);
        Debug.Log("CONNECTING...");
    }

    private void OnConnectedToMaster(){
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        Debug.Log("CONNECTED TO MASTER");
    }

    private void OnJoinedLobby(){
        Debug.Log("JOINED LOBBY");

        // in case of disconnect
        //if (LocalStorageManager.GetConnectedWorldName().Length > 0)
        //{
        //    //gameObject.GetComponent<WorldManager>().JoinWorld(LocalStorageManager.GetConnectedWorldName());
        //}
    }

    private void OnDisconnectedFromPhoton(){
        Debug.Log("DISCONNECTED");
        // BRING THIS BACK
        //PhotonNetwork.ConnectUsingSettings(versionName);
        //Debug.Log("TRYING TO RECONNECT...");
    }
}
