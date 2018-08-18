using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour {
    public Sprite slot_icon;
    public Button remove_button;
    private WorldItem item;

    public void AddItem(WorldItem i){
        Debug.Log("ADDING ITEM: "+i);
        item = i;
        //slot_icon.sprite = i.icon;
        //slot_icon.enabled = true;
        slot_icon = i.GetSprite();
        remove_button.interactable = true;
    }

    public void ClearSlot(){
        item = null;
        //slot_icon.sprite = null;
        //slot_icon.enabled = false;
        slot_icon = null;
        remove_button.interactable = false;
    }

    public void RemoveItem()
    {
        //item.Remove();
    }

    public void DropItem()
    {
        //item.Drop();
    }

    public void UseItem(){
        if (item != null){
            //item.Use();
        }
    }
}
