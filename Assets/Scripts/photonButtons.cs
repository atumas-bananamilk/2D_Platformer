using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class photonButtons : MonoBehaviour {
    public InputField createRoom, joinRoom;
    public photonHandler handler;

	public void onClickCreateRoom()
    {
        handler.createNewRoom();
    }
    public void onClickJoinRoom()
    {
        handler.joinOrCreateRoom();
    }
}