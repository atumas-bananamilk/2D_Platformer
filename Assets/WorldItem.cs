using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldItem : MonoBehaviour {
    GameObject obj = null;
    //private readonly string PICK_UP_ITEM = "PickUpItem";

    public Sprite GetSprite(){
        return gameObject.GetComponent<SpriteRenderer>().sprite;
    }

    private void OnCollisionEnter2D(Collision2D c)
    {
        if (c.gameObject.tag == "Player")
        {
            Physics2D.IgnoreCollision(c.collider, GetComponent<Collider2D>());
        }
    }

    public void Use()
    {
        Debug.Log("USING ITEM 2");
        //gameObject.SetActive(!gameObject.GetActive());
    }

    //public void Drop(Vector2 v)
    //{
    //    Debug.Log("DROPPING ITEM");
    //    //obj = Instantiate(gameObject, v, Quaternion.identity) as GameObject;
    //    //obj.GetComponent<SpriteRenderer>().sprite = GetSprite();
    //}

}