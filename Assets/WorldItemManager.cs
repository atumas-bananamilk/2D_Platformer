using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldItemManager : MonoBehaviour {
    public Sprite default_icon;
    public List<WorldItem> dropped_items = new List<WorldItem>();
    public GameObject item_prefab;

	public void AddItemToWorld(){
        GameObject obj = Instantiate(item_prefab, new Vector2(0, 0), Quaternion.identity) as GameObject;
        obj.transform.localScale = new Vector3(0.5f, 0.5f, 0);
        obj.GetComponent<SpriteRenderer>().sprite = default_icon;
    }
}
