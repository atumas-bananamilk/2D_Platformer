using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : Singleton<LobbyManager> {

    public Text welcome_message;
    public Text gold_amount_text;
    public Text SP_amount_text;
    public Animator player_animator;

	void Start () {
        AwsApiManager.Instance.GetUserStats();
        AnimatePlayer();
	}

    public void SetStats(){
        AwsApiManager.Instance.UpdateUserStats(100);
    }

    public void AnimatePlayer(){
        player_animator.Play(AnimatorManager.LOBBY_PLAYER_RUN);
    }

    public void OpenOutfitPanel(){
        
    }

    public void OpenMyEquipmentPanel(){
        
    }

    public void Logout(){
        
    }
}