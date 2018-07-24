using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TiledSharp;

public class MainGameManager : MonoBehaviour {
    [SerializeField] private GameObject player_info_text;
    [SerializeField] private GameObject player_grid;

    // called by photon
    public void OnPhotonPlayerConnected(PhotonPlayer player){
        GameObject obj = Instantiate(player_info_text, new Vector2(0, 0), Quaternion.identity);
        obj.transform.SetParent(player_grid.transform, false);
        obj.GetComponent<Text>().text = player.NickName + " joined the server";
        obj.GetComponent<Text>().color = Color.green;
    }

    public void OnPhotonPlayerDisconnected(PhotonPlayer player){
        GameObject obj = Instantiate(player_info_text, new Vector2(0, 0), Quaternion.identity);
        obj.transform.SetParent(player_grid.transform, false);
        obj.GetComponent<Text>().text = player.NickName + " left the server";
        obj.GetComponent<Text>().color = Color.red;
    }

	public void Awake()
	{
        var map = new TmxMap("./desert.tmx");
        var version = map.Version;
        var myTileset = map.Tilesets["myTileset"];
        var myLayer = map.Layers[2];
        var hiddenChest = map.ObjectGroups["Chests"].Objects["hiddenChest"];
	}


}