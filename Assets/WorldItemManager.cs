using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldItemManager : MonoBehaviour {
    public Sprite default_icon;
    public List<WorldItem> dropped_items = new List<WorldItem>();
    public GameObject item_prefab;

	public void AddItemToWorld(){
        //GameObject obj = Instantiate(item_prefab, new Vector2(0, 0), Quaternion.identity) as GameObject;
        //obj.transform.localScale = new Vector3(0.5f, 0.5f, 0);
        //obj.GetComponent<SpriteRenderer>().sprite = default_icon;
        CreateItem(WorldItem.ITEM_NAME.COIN_GOLD, item_prefab, new Vector2(0, 0), Quaternion.identity, new Vector2(0.5f, 0.5f), default_icon, true);
    }

    // same as add item, but no gravity
    public void AddWallToWorld(WorldItem.ITEM_NAME name, Vector2 position, Quaternion rotation, Vector2 scale, Sprite sprite){
        CreateItem(name, item_prefab, position, rotation, scale, sprite, false);
    }

    public GameObject CreateItem(WorldItem.ITEM_NAME name, GameObject prefab, Vector2 position, Quaternion rotation, Vector2 scale, Sprite sprite, bool has_gravity){
        GameObject obj = Instantiate(prefab, position, rotation) as GameObject;
        obj.transform.localScale = scale;
        obj.GetComponent<SpriteRenderer>().sprite = sprite;
        obj.GetComponent<WorldItem>().item_name = name;
        obj.GetComponent<WorldItem>().amount = 1;

        if (!has_gravity){
            obj.GetComponent<Rigidbody2D>().mass = 0;
            obj.GetComponent<Rigidbody2D>().gravityScale = 0;
        }

        return obj;
    }
}
