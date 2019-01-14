using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerHealthBar : Photon.MonoBehaviour {
    public playerMove player_move;

    public GameObject world_space_canvas;

    public GameObject canvas_local_player;
    public GameObject canvas_other_player;

    public Image local_player_healthbar;
    public Image other_player_healthbar;

    public Vector3 local_player_name_pos;
    public Vector3 other_player_name_pos;

    public Vector3 local_player_flag_pos;
    public Vector3 other_player_flag_pos;

    public GameObject background;
    public GameObject damage_text;
    private bool showing_damage_text = false;
    private float damage_text_size_init;
    private float damage_text_size;

    private int MAX_DAMAGE_TEXT_SIZE = 34;

    private float health_amount = 100f;

	void Awake()
    {
        //if (!player_move.dev_testing){
        //    SetCorrectCanvas();
        //}
        //else{
        //    canvas_local_player.SetActive(true);
        //}

        //if (photonView.isMine || player_move.dev_testing){
        if (TCPPlayer.IsMine(gameObject) || player_move.dev_testing){
            Debug.Log("ASSIGNING LOCAL");
            RespawnScript.Instance.local_player = gameObject;
        }
	}

	private void Start()
	{
        damage_text_size = damage_text_size_init = damage_text.GetComponent<Text>().fontSize;
	}

	private void Update()
	{
        SetDamageTextSize();
	}

    private void SetDamageTextSize(){
        if (showing_damage_text){
            if (damage_text_size < MAX_DAMAGE_TEXT_SIZE){
                damage_text_size += 1f;
            }
            damage_text.GetComponent<Text>().fontSize = Mathf.RoundToInt(damage_text_size);
        }
        else{
            damage_text_size = damage_text_size_init;
        }
    }

	public void SetCorrectCanvas(){
        //if (photonView.isMine){
        if (TCPPlayer.IsMine(gameObject)){
            player_move.player_name.GetComponent<RectTransform>().anchoredPosition = (local_player_name_pos);
            player_move.player_flag.GetComponent<RectTransform>().anchoredPosition = (local_player_flag_pos);
            canvas_local_player.SetActive(true);
            background.SetActive(true);
        }
        else{
            player_move.player_name.GetComponent<RectTransform>().anchoredPosition = (other_player_name_pos);
            player_move.player_flag.GetComponent<RectTransform>().anchoredPosition = (other_player_flag_pos);
            canvas_other_player.SetActive(true);
            background.SetActive(false);
        }
    }

    private int GetRandomInt(int min, int max){
        int r = UnityEngine.Random.Range(min, max + 1);
        return r;
    }
    
    IEnumerator SetDamageVisibleAfterDelay(float time, bool visible, int hit_amount){
        yield return new WaitForSeconds(time);
        damage_text.GetComponent<Text>().text = hit_amount.ToString();
        showing_damage_text = visible;
        damage_text.SetActive(visible);
     }

    private void PopupDamage(int hit_amount){
        StartCoroutine(SetDamageVisibleAfterDelay(0f, true, hit_amount));
        StartCoroutine(SetDamageVisibleAfterDelay(0.8f, false, 0));
    }

    public void ReduceHealth(float hit_amount)
    {
        if (TCPPlayer.IsMine(gameObject)){
            health_amount -= hit_amount;
            local_player_healthbar.fillAmount -= hit_amount;
        }
        else{
            other_player_healthbar.fillAmount -= hit_amount;

            PopupDamage((int) (hit_amount * 1000) + GetRandomInt(-100, 100));
        }
        CheckHealthAmount();
        GetComponent<Animator>().Play(AnimatorManager.PLAYER_HIT);
    }

    private void CheckHealthAmount()
    {
        if (health_amount <= 0.1f){
            KillPlayer();
            TCPNetwork.KillPlayer();
            gameObject.GetComponent<playerMove>().TryChangePlayerState(playerMove.PLAYERSTATE.DEAD);
        }
        //if (local_player_healthbar.fillAmount <= 0.1f)
        //{
        //}
    }

    public void KillPlayer()
    {
        player_move.disable_move = true;

        if (TCPPlayer.IsMine(gameObject)){
            RespawnScript.Instance.ShowDeadCanvas(10);
        }

        //Debug.Log("KILLING PLAYER");
        player_move.ChangePlayerState(playerMove.PLAYERSTATE.DEAD);
        GetComponent<PlayerWeaponManager>().HideWeapon(true);

        SetPlayerActive(false);

        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Rigidbody2D>().simulated = false;
        GetComponent<BoxCollider2D>().enabled = false;

        //if (!photonView.isMine){
        if (!TCPPlayer.IsMine(gameObject)){
            canvas_other_player.SetActive(false);
        }

        world_space_canvas.SetActive(false);
    }

    private void SetPlayerActive(bool active){
        float refill_amount = 1f;

        //this.GetComponent<SpriteRenderer>().enabled = active;
        GetComponent<Rigidbody2D>().simulated = active;
        //this.GetComponent<BoxCollider2D>().enabled = active;
        world_space_canvas.SetActive(active);

        if (TCPPlayer.IsMine(gameObject)){
            player_move.disable_move = !active;
            local_player_healthbar.fillAmount = refill_amount;
        }
        else{
            canvas_other_player.SetActive(active);
            other_player_healthbar.fillAmount = refill_amount;
        }
    }

    //[PunRPC]
    public void RespawnPlayer()
    {
        GetComponent<PlayerWeaponManager>().HideWeapon(false);
        SetPlayerActive(true);
    }
}
