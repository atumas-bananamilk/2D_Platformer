using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour {

    public Text welcome_message;
    public Text gold_amount_text;
    public Text SP_amount_text;

	void Start () {
        gameObject.GetComponent<AwsApiManager>().GetUserStats();
	}

    public void SetStats(){
        gameObject.GetComponent<AwsApiManager>().SetDefaultStats(100);
    }

    public void Logout(){
        
    }
}