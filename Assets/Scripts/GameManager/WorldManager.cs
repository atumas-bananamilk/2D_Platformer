using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldManager : MonoBehaviour {
    public InputField join_server_input;
    public InputField create_server_input;

    public void CreateWorld(){
        // check if room with my username already exists on server
        //AwsApiManager
        // if not
        //  create map (on server) from default, set this user as owner, fetch map from server
        // if yes
        //  fetch map from server
        //  load it on MapManager



        gameObject.GetComponent<photonHandler>().createNewRoom(create_server_input.text);
    }
    public void JoinWorld()
    {
        gameObject.GetComponent<photonHandler>().joinOrCreateRoom(join_server_input.text);
    }
}