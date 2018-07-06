using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
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
}
