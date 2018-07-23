using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : Singleton<LobbyManager> {

    public Text welcome_message;
    public Text gold_amount_text;
    public Text SP_amount_text;

	void Start () {
        AwsApiManager.Instance.GetUserStats();
	}

    public void SetStats(){
        AwsApiManager.Instance.SetDefaultStats(100);
    }

    public void Logout(){
        
    }
}