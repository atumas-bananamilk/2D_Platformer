using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatManager : MonoBehaviour {
    [SerializeField] private GameObject player_info_text;
    [SerializeField] private GameObject player_grid;
    //public InputField chat_input;

    void Awake()
    {
        
    }

    public void chatInputChange()
    {
        //GameObject obj = PhotonNetwork.Instantiate(player_info_text.name, new Vector2(0, 0), Quaternion.identity, 0);
        ////GameObject obj = Instantiate(player_info_text, new Vector2(0, 0), Quaternion.identity);
        //obj.transform.SetParent(player_grid.transform, false);
        //obj.GetComponent<Text>().text = PhotonNetwork.playerName + ": " + chat_input.text;
        //obj.GetComponent<Text>().color = Color.blue;

        //chat_input.text = "";

        //if (PhotonNetwork.isMasterClient){
        //    photonView.RPC("SendMessageRPC", PhotonTargets.All, "Hello there!");
        //}

    }

    //[PunRPC]
    //void SendMessage(string message)
    //{
    //    chatBox.AddMessage(message);
    //}
}
