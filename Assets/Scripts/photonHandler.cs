using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class photonHandler : MonoBehaviour {
    //[SerializeField] private GameObject player_info_text;
    //[SerializeField] private GameObject player_grid;

    public photonButtons photon_buttons;
    public GameObject main_player;

	private void Awake()
	{
        DontDestroyOnLoad(this.transform);

        // default - 20
        PhotonNetwork.sendRate = 30;
        // default - 10
        PhotonNetwork.sendRateOnSerialize = 20;
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
	}

	public void moveScene(){
        PhotonNetwork.LoadLevel("MainGame");
    }

    public void createNewRoom(){
        if (photon_buttons.createRoom.text.Length > 0)
        {
            PhotonNetwork.CreateRoom(photon_buttons.createRoom.text, new RoomOptions() { MaxPlayers = 4 }, null);
        }
    }

    public void joinOrCreateRoom(){
        //PhotonNetwork.JoinRoom(joinRoom.text);
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 4;
        PhotonNetwork.JoinOrCreateRoom(photon_buttons.joinRoom.text, options, TypedLobby.Default);
    }
    private void OnJoinedRoom()
    {
        moveScene();
        Debug.Log("CONNECTED TO THE ROOM: " + PhotonNetwork.room.Name);
    }

    private void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode){
        if (scene.name == "MainGame"){
            spawnPlayer();
        }
    }

    private void spawnPlayer(){
        GameObject[] spawn_points = GameObject.FindGameObjectsWithTag("SpawnPoint");

        if (PhotonNetwork.playerList.Length == 1)
        {
            //PhotonNetwork.Instantiate(main_player.name, main_player.transform.position, main_player.transform.rotation, 0);
            PhotonNetwork.Instantiate(main_player.name, spawn_points[0].transform.position, main_player.transform.rotation, 0);
        }
        if (PhotonNetwork.playerList.Length > 1)
        {
            PhotonNetwork.Instantiate(main_player.name, spawn_points[1].transform.position, main_player.transform.rotation, 0);
        }
    }
}
