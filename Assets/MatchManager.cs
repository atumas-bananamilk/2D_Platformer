using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class MatchManager : MonoBehaviour {
    // sends TCP request - findmatch
    // TCP:
    // 1. tries to find a room (with X available spots)
    // 2. if couldn't find - creates a room (with 4 available spots)
    // 3. sends info (players' names) to all players about new players
    // 4. if enough players joined - send room name to everyone, notify to start the game
    // everyone joins the room
    // TCP ids are assigned, everyone inits everyone
    // game starts

    public GameObject match_ready_panel;
    public GameObject player_panel;
    public GameObject main_panel;
    public GameObject find_match_button;
    public GameObject leave_match_button;
    public Text match_ready_players_count;

    private Color DEFAULT_INDICATOR_COLOUR;
    private readonly int MATCH_PLAYER_LIMIT = 50;

    //int aa = 0;

	private void Start()
    {
        DEFAULT_INDICATOR_COLOUR = match_ready_panel.GetComponentsInChildren<Image>().Skip(1).ToArray()[0].color;
        //StartCoroutine(ChangeIndicatorAfterDelay());
    }

    //IEnumerator ChangeIndicatorAfterDelay()
    //{
    //    while (true)
    //    {
    //        IndicateReadyPlayers(aa);
    //        if (aa < MATCH_PLAYER_LIMIT){
    //            aa++;
    //        }
    //        yield return new WaitForSeconds(0.1f);
    //    }
    //}

	public void FindMatch(){
        gameObject.GetComponent<TCPNetwork>().FindMatch();
        match_ready_panel.SetActive(true);
        player_panel.SetActive(false);
        main_panel.SetActive(false);
        find_match_button.SetActive(false);
        leave_match_button.SetActive(true);
    }

    public void LeaveMatch(){
        gameObject.GetComponent<TCPNetwork>().LeaveMatch();
        match_ready_panel.SetActive(false);
        player_panel.SetActive(true);
        main_panel.SetActive(true);
        find_match_button.SetActive(true);
        leave_match_button.SetActive(false);
        gameObject.GetComponent<LobbyManager>().AnimatePlayer();
    }

    public void IndicateReadyPlayers(int amount){
        Image[] ready_indicators = match_ready_panel.GetComponentsInChildren<Image>().Skip(1).ToArray();

        //ClearIndicators();

        if (amount <= MATCH_PLAYER_LIMIT){
            for (int i = 0; i < amount; i++)
            {
                ready_indicators[i].color = new Color(0, 255, 0);
            }
        }
        match_ready_players_count.text = amount + " / " + MATCH_PLAYER_LIMIT;
    }

    public void ClearIndicators(){
        Image[] ready_indicators = match_ready_panel.GetComponentsInChildren<Image>().Skip(1).ToArray();

        foreach (Image indicator in ready_indicators){
            indicator.color = DEFAULT_INDICATOR_COLOUR;
        }
    }

    public void GoToRoom(string room_name, int spawn_location_id){
        gameObject.GetComponent<WorldManager>().GoToRoom(room_name, spawn_location_id);
    }
}
