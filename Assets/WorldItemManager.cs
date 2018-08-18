using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldItemManager : MonoBehaviour {
    public Sprite default_icon;
    public List<WorldItem> dropped_items = new List<WorldItem>();
    public GameObject item_prefab;

	public void AddItemToWorld(){
        //WorldItem item = new WorldItem(default_icon);
        //dropped_items.Add(item);

        //item.Drop();

        GameObject obj = Instantiate(item_prefab, new Vector2(0, 0), Quaternion.identity) as GameObject;
        obj.GetComponent<SpriteRenderer>().sprite = default_icon;
    }
}
