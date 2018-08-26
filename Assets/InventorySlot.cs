using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour {
    public Image slot_icon;
    public Button remove_button;
    public Image slot_amount;
    public WorldItem item;

    private void CreateNewItem(){
        item = gameObject.AddComponent<WorldItem>();
    }

    public void AddToSlot(WorldItem i){
        // check if something is already in this slot & the same type - increment amount
        if (item && item.item_name == i.item_name){
            ToggleAmount(true);
        }
        else{
            ToggleAmount(false);
            CreateNewItem();
            item.icon = i.GetSprite();
            slot_icon.sprite = item.icon;
            slot_icon.enabled = true;
            remove_button.interactable = true;
        }
    }

    private void ToggleAmount(bool toggle_on){
        Text amount_text = slot_amount.GetComponentInChildren<Text>();

        if (toggle_on){
            slot_amount.enabled = true;
            amount_text.enabled = true;
            int amount = Int32.Parse(amount_text.text) + 1;
            amount_text.text = amount.ToString();
        }
        else{
            slot_amount.enabled = false;
            amount_text.enabled = false;
            amount_text.text = "1";
        }
    }

    public void ClearSlot(){
        Destroy(item);
        slot_icon.sprite = null;
        slot_icon.enabled = false;
        remove_button.interactable = false;
    }

    public bool IsEmpty(){
        return item == null;
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
        bool is_null = item == null;
        item.Destroy();
        ClearSlot();
        is_null = item == null;
    }

    public void DropItem()
    {
        playerMove p = gameObject.GetComponentInParent<playerMove>();
        Vector2 v = new Vector2(p.transform.position.x, p.transform.position.y + 1);
        item.Drop(v, p.sprite.flipX);
    }
}
