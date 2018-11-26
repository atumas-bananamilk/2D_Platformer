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
        //obj.GetComponent<BoxCollider2D>().enabled = has_collider;
        obj.GetComponent<PolygonCollider2D>().enabled = has_collider;

        return obj;
    }
}
