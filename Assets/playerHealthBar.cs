using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerHealthBar : Photon.MonoBehaviour {
    public playerMove player_move;

    public GameObject world_space_canvas;

    public GameObject local_player_canvas;
    public GameObject other_player_canvas;

    public Image local_player_healthbar;
    public Image other_player_healthbar;

    public Vector3 local_player_name_pos;
    public Vector3 other_player_name_pos;

	void Awake()
    {
        if (!player_move.dev_testing){
            setCorrectCanvas();
        }

        if (photonView.isMine){
            RespawnScript.Instance.local_player = this.gameObject;
        }
	}
    
    void setCorrectCanvas(){
        if (photonView.isMine){
            player_move.player_name.GetComponent<RectTransform>().anchoredPosition = (local_player_name_pos);
            local_player_canvas.SetActive(true);
        }
        else{
            player_move.player_name.GetComponent<RectTransform>().anchoredPosition = (other_player_name_pos);
            other_player_canvas.SetActive(true);
        }
    }

    [PunRPC]
    public void reduceHealth()
    {
        reduceHealthAmount(0.2f);
    }

    public void reduceHealthAmount(float hit)
    {
        if (photonView.isMine)
        {
            local_player_healthbar.fillAmount -= hit;
            checkHealthAmount();
        }
        else
        {
            other_player_healthbar.fillAmount -= hit;
        }
    }

    private void checkHealthAmount()
    {
        if (local_player_healthbar.fillAmount <= 0.1f)
        {
            RespawnScript.Instance.StartTimer();
            player_move.disable_move = true;
            this.GetComponent<PhotonView>().RPC("killPlayer", PhotonTargets.AllBuffered);
        }
    }

    [PunRPC]
    public void killPlayer()
    {
        this.GetComponent<SpriteRenderer>().enabled = false;
        this.GetComponent<Rigidbody2D>().simulated = false;
        this.GetComponent<BoxCollider2D>().enabled = false;

        if (!photonView.isMine){
            other_player_canvas.SetActive(false);
        }

        world_space_canvas.SetActive(false);
    }

    [PunRPC]
    public void respawnPlayer()
    {
        this.GetComponent<SpriteRenderer>().enabled = true;
        this.GetComponent<Rigidbody2D>().simulated = true;
        this.GetComponent<BoxCollider2D>().enabled = true;

        if (!photonView.isMine)
        {
            other_player_canvas.SetActive(true);
            other_player_healthbar.fillAmount = 1;
        }
        else{
            player_move.disable_move = false;
            local_player_healthbar.fillAmount = 1;
        }

        world_space_canvas.SetActive(true);
    }
}
