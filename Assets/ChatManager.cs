using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : Photon.MonoBehaviour {
    public GameObject chat;
    public InputField chat_input;
    public Button chat_button;
    public Button chat_send_button;
    public Text local_player_chat_text;

    public Text player_chat_text_prefab;
    public GameObject canvas_local_player;

	private void Update()
	{
        if (chat_input.isFocused)
        {
            gameObject.GetComponent<playerMove>().disable_move = true;
        }
        else
        {
            gameObject.GetComponent<playerMove>().disable_move = false;
        }
	}

	public void OpenChatWindow(){
        chat.SetActive(!chat.GetActive());
    }

    public void ShowMessage(string msg){
        local_player_chat_text.text = msg;
        local_player_chat_text.gameObject.SetActive(true);
        StartCoroutine(RemoveMessageAfterSeconds(2));
    }

    IEnumerator RemoveMessageAfterSeconds(float time)
    {
        yield return new WaitForSeconds(time);
        local_player_chat_text.text = "";
        local_player_chat_text.gameObject.SetActive(false);
    }

    public void SendChatMessage(){
        // locally
        ShowMessage(chat_input.text);

        object[] list = { 
            gameObject.GetComponent<PhotonView>().viewID, 
            chat_input.text
        };

        ResetChatInput();

        // remotely
        RaiseEventOptions options = new RaiseEventOptions();
        options.Receivers = ReceiverGroup.Others;
        PhotonNetwork.RaiseEvent((byte)playerMove.PHOTON_EVENTS.SEND_MSG, list, true, options);
    }

    private void ResetChatInput(){
        chat_input.text = "";
    }

    public void CheckMessageFinished(){
        if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
        {
            SendChatMessage();
        }
    }
}
