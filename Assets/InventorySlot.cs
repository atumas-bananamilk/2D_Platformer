using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour {
    public Sprite icon { get; set; }

    public void Use(){
        //WorldItemManager.AddItemToWorld();
    }

    public void Drop(Vector2 v){
        Debug.Log("DROPPING ITEM");
        //WorldItemManager
    }
}

public class InventorySlot : MonoBehaviour {
    public GameObject item_prefab;
    public Image slot_icon;
    public Button remove_button;
    //private WorldItem item;
    private GameObject item;

    public void AddItem(WorldItem i){
        item = Instantiate(item_prefab, new Vector2(0, 0), Quaternion.identity) as GameObject;
        item.icon = i.GetSprite();
        //item = i;
        slot_icon.sprite = item.icon;
        slot_icon.enabled = true;
        remove_button.interactable = true;
    }

    public void ClearSlot(){
        Destroy(item);
        slot_icon.sprite = null;
        slot_icon.enabled = false;
        remove_button.interactable = false;
    }

    public void UseItem()
    {
        if (item != null)
        {
            //item.Use();
            DropItem();
            ClearSlot();
        }
    }

    public void RemoveItem()
    {
        //item.Remove();
    }

    public void DropItem()
    {
        float x = gameObject.GetComponentInParent<playerMove>().transform.position.x;
        float y = gameObject.GetComponentInParent<playerMove>().transform.position.y;
        Vector2 v = new Vector2(x, y);
        item.Drop(v);
    }
}
