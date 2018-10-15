using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class MatchManager : MonoBehaviour {
    public GameObject match_ready_panel;
    public GameObject find_match_button;
    public GameObject leave_match_button;

    private Color DEFAULT_INDICATOR_COLOUR;

    private readonly int MATCH_PLAYER_LIMIT = 4;

	// sends TCP request - findmatch
	// TCP:
	// 1. tries to find a room (with X available spots)
	// 2. if couldn't find - creates a room (with 4 available spots)
	// 3. sends info (players' names) to all players about new players
	// 4. if enough players joined - send room name to everyone, notify to start the game
	// everyone joins the room
	// TCP ids are assigned, everyone inits everyone
	// game starts

	private void Start()
    {
        DEFAULT_INDICATOR_COLOUR = match_ready_panel.GetComponentsInChildren<Image>().Skip(1).ToArray()[0].color;
	}

	public void FindMatch(){
        TCPNetwork.FindMatch();
        find_match_button.SetActive(false);
        leave_match_button.SetActive(true);
    }

    public void LeaveMatch(){
        TCPNetwork.LeaveMatch();
        find_match_button.SetActive(true);
        leave_match_button.SetActive(false);
    }

    public void IndicateReadyPlayers(int amount){
        Image[] ready_indicators = match_ready_panel.GetComponentsInChildren<Image>().Skip(1).ToArray();

        ClearIndicators();

        if (amount <= MATCH_PLAYER_LIMIT){
            for (int i = 0; i < amount; i++)
            {
                ready_indicators[i].color = new Color(0, 255, 0);
            }
        }
    }

    public void ClearIndicators(){
        Image[] ready_indicators = match_ready_panel.GetComponentsInChildren<Image>().Skip(1).ToArray();
        for (int i = 0; i < MATCH_PLAYER_LIMIT; i++)
        {
            ready_indicators[i].color = DEFAULT_INDICATOR_COLOUR;
        }
    }

    public void GoToRoom(string room_name, int spawn_location_id){
        gameObject.GetComponent<WorldManager>().GoToRoom(room_name, spawn_location_id);
    }
}
