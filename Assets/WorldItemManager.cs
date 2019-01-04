using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldItemManager : MonoBehaviour {
    public Sprite default_icon;
    public List<WorldItem> dropped_items = new List<WorldItem>();
    public GameObject item_prefab;
    public GameObject wall_prefab;

	public void AddItemToWorld(){
        CreateItem(WorldItem.ITEM_NAME.COIN_GOLD, 
                   item_prefab, 
                   new Vector2(0, 0), 
                   Quaternion.identity, 
                   new Vector2(0.5f, 0.5f), 
                   default_icon, 
                   false,
                   1f);
    }

    public void DropItemToWorld(Vector2 pos, WorldItem item, Vector2 direction){
        Vector2 scale = new Vector2(1f, 1f);
        Vector2 collider_size = new Vector2(0.2f, 0.2f);

        switch (item.item_type){
            case WorldItem.ITEM_TYPE.GUN:{
                    scale = new Vector2(1f, 1f);
                    collider_size = new Vector2(1.6f, 0.9f);
                    break;
                }
            case WorldItem.ITEM_TYPE.GRENADE:{
                    scale = new Vector2(5f, 5f);
                    collider_size = new Vector2(0.2f, 0.2f);
                    break;
                }
            case WorldItem.ITEM_TYPE.HEALTH:{
                    scale = new Vector2(7f, 7f);
                    collider_size = new Vector2(0.2f, 0.2f);
                    break;
                }
            case WorldItem.ITEM_TYPE.SHIELD:{
                    scale = new Vector2(7f, 7f);
                    collider_size = new Vector2(0.2f, 0.2f);
                    break;
                }
        }

        GameObject obj = CreateItem(item.item_name,
                                    item_prefab,
                                    pos,
                                    Quaternion.identity,
                                    scale,
                                    item.GetSprite(),
                                    true,
                                    1f);

        obj.GetComponent<BoxCollider2D>().size = collider_size;
        obj.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 1100 + direction * 300);
    }

    // same as add item, but no gravity
    public void AddWallToWorld(WorldItem.ITEM_NAME name, Vector2 position, Quaternion rotation, Vector2 scale, Sprite sprite){
        CreateItem(name, 
                   wall_prefab, 
                   position, 
                   rotation, 
                   scale, 
                   sprite, 
                   true,
                   1f);
    }

    public GameObject AddWallHoverToWorld(WorldItem.ITEM_NAME name, Vector2 position, Quaternion rotation, Vector2 scale, Sprite sprite){
        return CreateItem(name, 
                   wall_prefab, 
                   position, 
                   rotation, 
                   scale, 
                   sprite, 
                   false,
                   0.3f);
    }

    public GameObject CreateItem(WorldItem.ITEM_NAME name, 
                                 GameObject prefab, 
                                 Vector2 position, 
                                 Quaternion rotation, 
                                 Vector2 scale, 
                                 Sprite sprite, 
                                 bool has_collider,
                                 float opacity){
        
        GameObject obj = Instantiate(prefab, position, rotation) as GameObject;
        obj.transform.localScale = scale;
        obj.GetComponent<SpriteRenderer>().sprite = sprite;
        obj.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, opacity);
        obj.GetComponent<WorldItem>().item_name = name;
        obj.GetComponent<WorldItem>().amount = 1;
        if (obj.GetComponent<BoxCollider2D>() != null){
            obj.GetComponent<BoxCollider2D>().enabled = has_collider;
        }
        if (obj.GetComponent<PolygonCollider2D>() != null){
            obj.GetComponent<PolygonCollider2D>().enabled = has_collider;
        }
        return obj;
    }
}
