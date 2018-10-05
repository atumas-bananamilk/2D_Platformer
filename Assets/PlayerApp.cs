using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerApp : MonoBehaviour {

    private GAMESTATE state;

    private enum GAMESTATE{
        RUNNING,
        PAUSE
    }

	private void Start()
	{
        state = GAMESTATE.RUNNING;
	}

    void OnApplicationFocus(bool has_focus)
    {
        if (!has_focus){
            state = GAMESTATE.PAUSE;
        }
        else{
            state = GAMESTATE.RUNNING;
        }
    }

    void OnApplicationPause(bool pause_status)
    {
        if (!pause_status)
        {
            state = GAMESTATE.PAUSE;
        }
        else
        {
            state = GAMESTATE.RUNNING;
        }
    }

    void OnApplicationQuit()
    {
        TCPNetwork.Disconnect();
    }
}
