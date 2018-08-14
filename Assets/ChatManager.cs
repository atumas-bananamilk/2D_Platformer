using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour {
    public GameObject chat;
    public InputField chat_input;
    public Button chat_button;
    public Button chat_send_button;
    public Text player_chat_text;

	// Use this for initialization
    public void OpenChatWindow(){
        chat.SetActive(true);
    }

    public void SendChatMessage(){
        player_chat_text.text = chat_input.text;
    }
}
