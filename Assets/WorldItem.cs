using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class WorldItem : ScriptableObject {
public class WorldItem : MonoBehaviour {

    private readonly string PICK_UP_ITEM = "PickUpItem";
    
    void OnMouseDown(){
        //GameObject[] items = null;

        //if (gameObject.tag.Equals(PICK_UP_ITEM)){
        //    items = GameObject.FindGameObjectsWithTag(PICK_UP_ITEM);
        //}

        //foreach (GameObject p in items)
        //{
        //    if (gameObject.GetInstanceID() == p.GetInstanceID()){
        //        Destroy(p);
        //    }
        //}

        //Debug.Log("VIEW: "+gameObject.GetPhotonView());

        //PhotonView.Find()
    }

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

    //[HideInInspector] public Sprite icon;
    //public GameObject item_prefab;
    //GameObject obj = null;

    //public WorldItem(Sprite image)
    //{
    //    icon = image;
    //}

    //public void Use()
    //{
    //    Destroy(obj);
    //    Destroy(this);
    //}

    public void Remove(){
        //Destroy(this);
    }

    //public void Drop()
    //{
    //    float x = item_prefab.transform.position.x;
    //    float y = item_prefab.transform.position.y;

    //    obj = Instantiate(item_prefab, new Vector2(x, y), Quaternion.identity) as GameObject;
    //    obj.GetComponent<SpriteRenderer>().sprite = icon;
    //}

}
