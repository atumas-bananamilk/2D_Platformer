using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour {
    public GameObject chat;
    public InputField chat_input;
    public Button chat_button;
    public Button chat_send_button;
    public Text local_player_chat_text;

    public void OpenChatWindow(){
        chat.SetActive(true);
    }

    public void SendChatMessage(){
        // locally
        local_player_chat_text.text = chat_input.text;

        object[] list = { 
            gameObject.GetComponent<PhotonView>().viewID, 
            chat_input.text
        };

        // remotely
        RaiseEventOptions options = new RaiseEventOptions();
        options.Receivers = ReceiverGroup.Others;
        PhotonNetwork.RaiseEvent((byte)playerMove.PHOTON_EVENTS.SEND_MSG, list, true, options);
    }
}
