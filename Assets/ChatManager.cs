using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour {
    [SerializeField] private GameObject player_info_text;
    [SerializeField] private GameObject player_grid;
    public InputField chat_input;

    void Awake()
    {
        
    }

    public void chatInputChange()
    {
        GameObject obj = Instantiate(player_info_text, new Vector2(0, 0), Quaternion.identity);
        obj.transform.SetParent(player_grid.transform, false);
        obj.GetComponent<Text>().text = PhotonNetwork.playerName + ": " + chat_input.text;
        obj.GetComponent<Text>().color = Color.blue;

        chat_input.text = "";
    }
}
