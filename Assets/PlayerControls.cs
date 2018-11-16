using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour {

    public void ChooseWall()
    {

        Debug.Log("HERE");

        //Vector2 wall_pos = new Vector2(transform.position.x + 2, transform.position.y);
        //gameObject.GetComponent<PlayerWallManager>().PlaceWall(wall_pos);
    }

    public void CheckSection(){
        Debug.Log("OK");

        //float xDiff = x2 - x1;
        //float yDiff = y2 - y1;
        //return Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI;
    }
}
