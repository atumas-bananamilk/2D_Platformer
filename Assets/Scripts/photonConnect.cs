﻿using System.Collections;
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
        //SceneManager.LoadScene("Lobby");
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        Debug.Log("CONNECTED TO MASTER");
    }

    private void OnJoinedLobby(){
        Debug.Log("JOINED LOBBY");
    }

    private void OnDisconnectedFromPhoton(){
        Debug.Log("DISCONNECTED");
        PhotonNetwork.ConnectUsingSettings(versionName);
        Debug.Log("TRYING TO RECONNECT...");
    }
}
