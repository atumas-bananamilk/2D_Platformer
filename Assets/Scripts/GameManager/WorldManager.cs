using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WorldManager : MonoBehaviour {
    public GameObject main_player;

    public InputField join_server_input;
    public Text error_create_world;
    public Button create_server_button;
    public Button my_server_button;

	private void Awake()
    {
        DontDestroyOnLoad(this.transform);
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
        gameObject.GetComponent<TCPNetwork>().Connect();
    }

    public void GoToMyWorld()
    {
        if (TCPNetwork.Connected)
        {
            SceneManager.LoadScene("MainGame");
        }
        //gameObject.GetComponent<photonHandler>().joinOrCreateRoom(PhotonNetwork.playerName);
    }

    private void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode){
        if (scene.name == "MainGame"){
            SpawnPlayer();
        }
    }

    private void SpawnPlayer()
    {
        GameObject[] spawn_points = GameObject.FindGameObjectsWithTag("SpawnPoint");
        gameObject.GetComponent<TCPNetwork>().Instantiate(main_player.name, "main_world", spawn_points[1].transform.position, main_player.transform.rotation);
    }

    //private void Start()
    //{
    //       AwsApiManager.Instance.CheckWorldExists(AnalyseResponse_1);
    //}

	//public void TryCreateWorld(){
    //    AwsApiManager.Instance.TryCreateWorld(AnalyseResponse_2);
    //}

    // being called by AwsApiManager
    //public void AnalyseResponse_1(object response){
    //    string msg = (string)response;
    //    bool has_no_text = msg.Length <= 0;
    //    SwitchButtons(msg, has_no_text);
    //}

    //// being called by AwsApiManager
    //public void AnalyseResponse_2(object response){
    //    string msg = (string) response;
    //    bool has_text = msg.Length > 0;
    //    SwitchButtons(msg, has_text);
    //}

    //private void SwitchButtons(string msg, bool on){
    //    error_create_world.text = msg;
    //    error_create_world.gameObject.SetActive(on);
    //    create_server_button.gameObject.SetActive(on);
    //    my_server_button.gameObject.SetActive(!on);
    //    //my_server_button.GetComponentInChildren<Text>().text = "My world (" + PhotonNetwork.playerName + ")";
    //}

    //public void JoinWorld()
    //{
    //    //gameObject.GetComponent<photonHandler>().joinOrCreateRoom(join_server_input.text);
    //}

    //public void JoinWorld(string name)
    //{
    //    //gameObject.GetComponent<photonHandler>().joinOrCreateRoom(name);
    //}
}