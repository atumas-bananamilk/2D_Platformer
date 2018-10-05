using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class photonHandler : MonoBehaviour {
    //[SerializeField] private GameObject player_info_text;
    //[SerializeField] private GameObject player_grid;

    public GameObject main_player;

	private void Awake()
	{
        DontDestroyOnLoad(this.transform);
        PhotonNetwork.sendRate = 30; // default - 20
        PhotonNetwork.sendRateOnSerialize = 20; // default - 10
        SceneManager.sceneLoaded += OnSceneFinishedLoading;

        if (main_player.GetComponent<playerMove>().dev_testing){
            moveScene();
        }
	}

    public void createNewRoom(string world_name){
        if (world_name.Length > 0)
        {
            RoomOptions options = new RoomOptions();
            options.MaxPlayers = 4;
            PhotonNetwork.CreateRoom(world_name, options, null);
        }
    }

    public void joinOrCreateRoom(string world_name)
    {
        if (world_name.Length > 0)
        {
            RoomOptions options = new RoomOptions();
            options.MaxPlayers = 4;
            PhotonNetwork.JoinOrCreateRoom(world_name, options, TypedLobby.Default);
        }
    }

    private void OnJoinedRoom()
    {
        LocalStorageManager.StoreConnectedWorldName(PhotonNetwork.room.Name);
        moveScene();
        Debug.Log("CONNECTED TO THE ROOM: " + PhotonNetwork.room.Name);
    }

    public void moveScene()
    {
        PhotonNetwork.LoadLevel("MainGame");
    }

    private void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode){
        if (scene.name == "MainGame"){
            spawnPlayer();
        }
    }

    private void spawnPlayer(){
        GameObject[] spawn_points = GameObject.FindGameObjectsWithTag("SpawnPoint");

        gameObject.GetComponent<TCPNetwork>().Instantiate(main_player.name, spawn_points[1].transform.position, main_player.transform.rotation);

        //TCPNetwork.Instantiate(main_player.name, spawn_points[1].transform.position, main_player.transform.rotation);

        if (PhotonNetwork.playerList.Length == 1)
        {
            //PhotonNetwork.Instantiate(main_player.name, main_player.transform.position, main_player.transform.rotation, 0);
            //PhotonNetwork.Instantiate(main_player.name, spawn_points[0].transform.position, main_player.transform.rotation, 0);
        }
        //else
        //{
        //    PhotonNetwork.Instantiate(main_player.name, spawn_points[1].transform.position, main_player.transform.rotation, 0);
        //}
    }
}
