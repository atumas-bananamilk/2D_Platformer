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
    private WorldItem item;
    //private GameObject item;

    //void Start()
    //{
    //    item = gameObject.AddComponent<WorldItem>();
    //}

    private void CreateNewItem(){
        item = gameObject.AddComponent<WorldItem>();
    }

    public void AddItem(WorldItem i){
        CreateNewItem();
        item.icon = i.GetSprite();
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
        item.Remove();
    }

    public void DropItem()
    {
        playerMove p = gameObject.GetComponentInParent<playerMove>();
        Vector2 v = new Vector2(p.transform.position.x, p.transform.position.y + 1);
        item.Drop(v, p.sprite.flipX);
    }
}
