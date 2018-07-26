using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : Photon.MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
    }

    [PunRPC]
    public void changeDirectionLeft()
    {
        //moving_direction = true;
    }

    [PunRPC]
    private void destroyObj()
    {
        Destroy(gameObject);
    }
}
