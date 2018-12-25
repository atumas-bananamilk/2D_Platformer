using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RespawnScript : MonoBehaviour {
    public static RespawnScript Instance;

    [HideInInspector] public GameObject local_player;
    public GameObject respawn_canvas;
    public Text timer_text;

    [SerializeField] private float timer_amount = 5;

    private bool enable_timer = false;

	private void Awake()
	{
        Instance = this;
	}

	private void Update()
	{
        if (enable_timer){
            timer_amount -= Time.deltaTime;
            timer_text.text = "Respawn in: "+timer_amount.ToString("F0"); // F0 - format

            if (timer_amount <= 0){
                //local_player.GetComponent<PhotonView>().RPC("respawnPlayer", PhotonTargets.AllBuffered);
                local_player.GetComponent<playerHealthBar>().RespawnPlayer();
                respawn_canvas.SetActive(false);
                enable_timer = false;
            }
        }
	}

    public void ShowDeadCanvas(float time){
        respawn_canvas.SetActive(true);
        StartTimer(time);
    }

    public void StartTimer(float time){
        timer_amount = time;
        enable_timer = true;
    }
}