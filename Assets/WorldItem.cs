using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldItem : MonoBehaviour {
    public Sprite icon { get; set; }

    private readonly string PLAYER = "Player";
    private readonly string PICK_UP_ITEM = "PickUpItem";

	public Sprite GetSprite(){
        icon = gameObject.GetComponent<SpriteRenderer>().sprite;
        return gameObject.GetComponent<SpriteRenderer>().sprite;
    }

    private void OnCollisionEnter2D(Collision2D c)
    {
        if (c.gameObject.tag == PLAYER)
        {
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(PICK_UP_ITEM), LayerMask.NameToLayer(PLAYER), true);
        }
    }

	public void Use()
    {
        Debug.Log("USING ITEM 2");
    }

    public void Drop(Vector2 v, bool direction)
    {
        GameObject obj = Instantiate(gameObject, v, Quaternion.identity) as GameObject;
        obj.transform.localScale = new Vector3(0.5f, 0.5f, 0);
        obj.tag = PICK_UP_ITEM;
        obj.layer = LayerMask.NameToLayer(PICK_UP_ITEM);

        SpriteRenderer r = obj.AddComponent<SpriteRenderer>();
        r.sprite = icon;

        BoxCollider2D c = obj.AddComponent<BoxCollider2D>();
        c.size = new Vector2(1, 1);

        Rigidbody2D b = obj.AddComponent<Rigidbody2D>();
        b.mass = 1;
        b.gravityScale = 4;
        b.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
        Vector2 direction_v = direction ? Vector2.left : Vector2.right;
        b.AddForce(Vector2.up * 800 + direction_v * 200);
    }

    public void Remove(){
        Destroy(this);
    }
}