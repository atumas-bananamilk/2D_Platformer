using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldItem : MonoBehaviour {
    public Sprite icon { get; set; }
    public ITEM_NAME item_name;
    public ITEM_TYPE item_type;
    public int amount;

    public enum ITEM_NAME
    {
        WALL_WOODEN, WALL_BRICK, WALL_METAL, COIN_GOLD, BLOCK_DIRT
    }

    public enum ITEM_TYPE{
        GUN, GRENADE, HEALTH, SHIELD
    }

    public void SetSprite(Sprite sprite){
        icon = sprite;
    }

	public Sprite GetSprite(){
        //icon = gameObject.GetComponent<SpriteRenderer>().sprite;
        //return gameObject.GetComponent<SpriteRenderer>().sprite;
        return icon;
    }

    private void OnCollisionEnter2D(Collision2D c)
    {
        if (c.gameObject.tag == TagManager.PLAYER){
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(TagManager.PICK_UP_ITEM), LayerMask.NameToLayer(TagManager.PLAYER), true);
        }
    }
    
    IEnumerator ChangeWallColourAfterDelay(float time, Color c){
        yield return new WaitForSeconds(time);
        GetComponent<SpriteRenderer>().color = c;
     }

    public void TakeDamage(){
        StartCoroutine(ChangeWallColourAfterDelay(0f, Color.red));
        StartCoroutine(ChangeWallColourAfterDelay(0.1f, Color.white));
    }

	public void Use()
    {
        Debug.Log("USING ITEM 2");
    }

    public void Drop(Vector2 v, bool direction, int how_many)
    {
        GameObject obj = Instantiate(gameObject, v, Quaternion.identity) as GameObject;
        obj.transform.localScale = new Vector3(0.5f, 0.5f, 0);
        obj.tag = TagManager.PICK_UP_ITEM;
        obj.layer = LayerMask.NameToLayer(TagManager.PICK_UP_ITEM);

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

        obj.GetComponent<WorldItem>().amount = how_many;
    }

    public void Destroy(){
        Destroy(this);
    }
}