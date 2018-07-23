using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class photonConnect : MonoBehaviour {
    public string versionName = "0.1";
    //public GameObject section_view_1, section_view_2, section_view_3;

    //public void connectToPhoton()
    //{
    //    PhotonNetwork.ConnectUsingSettings(versionName);
    //    Debug.Log("CONNECTING...");
    //}

    public void Awake()
    {
        PhotonNetwork.ConnectUsingSettings(versionName);
        Debug.Log("CONNECTING...");
    }

    private void OnConnectedToMaster(){
        //section_view_2.SetActive(true);
        //section_view_3.SetActive(false);

        PhotonNetwork.JoinLobby(TypedLobby.Default);
        Debug.Log("CONNECTED TO MASTER");
    }

    private void OnJoinedLobby(){
        //section_view_1.SetActive(false);
        //section_view_2.SetActive(true);
        Debug.Log("JOINED LOBBY");
    }

    private void OnDisconnectedFromPhoton(){
        //if (section_view_1.GetActive()){
        //    section_view_1.SetActive(false);
        //}
        //if (section_view_2.GetActive()){
        //    section_view_2.SetActive(false);
        //}
        //section_view_3.SetActive(true);

        Debug.Log("DISCONNECTED");

        PhotonNetwork.ConnectUsingSettings(versionName);
        Debug.Log("TRYING TO RECONNECT...");
    }
}
