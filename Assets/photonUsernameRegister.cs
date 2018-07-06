using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class photonUsernameRegister : MonoBehaviour {
    private bool already_registered = false;

    public InputField username_input;
    public GameObject create_button;
    public GameObject object_parent;

    void Awake()
    {
        // PlayerPrefs.SetInt("registered", 0)
        // PlayerPrefs.GetInt("registered")
        checkRegister();
    }

    void checkRegister(){
        if (!already_registered){
            object_parent.SetActive(true);
        }
    }

    public void usernameInputChange(){
        if (username_input.text.Length > 2){
            create_button.SetActive(true);
        }
        else{
            create_button.SetActive(false);
        }
    }

    public void createUsername(){
        PhotonNetwork.playerName = username_input.text;
        object_parent.SetActive(false);
        Debug.Log("MACHINE NAME: "+PhotonNetwork.playerName);
    }
}