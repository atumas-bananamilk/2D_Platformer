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
        // if something of this type is already in this slot - just add amount
        if (item && item.item_name == i.item_name){
            UpdateAmount(1);
        }
        // otherwise - put new item into inventory
        else{
            ResetAmount();
            CreateNewItem();
            item.icon = i.GetSprite();
            slot_icon.sprite = item.icon;
            slot_icon.enabled = true;
            remove_button.interactable = true;
        }
    }

    private int GetAmount(){
        return Int32.Parse(slot_amount.GetComponentInChildren<Text>().text);
    }

    private void SetAmount(int amount){
        slot_amount.GetComponentInChildren<Text>().text = amount.ToString();
    }

    private void ResetAmount(){
        Text amount_text = slot_amount.GetComponentInChildren<Text>();
        slot_amount.enabled = false;
        amount_text.enabled = false;
        amount_text.text = "1";
    }

    private void UpdateAmount(int amount){
        Text amount_text = slot_amount.GetComponentInChildren<Text>();
        int new_amount = GetAmount() + amount;
        if (new_amount > 1){
            slot_amount.enabled = true;
            amount_text.enabled = true;
            SetAmount(new_amount);
        }
        else{
            ResetAmount();
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
        }
    }

    public void RemoveItem()
    {
        bool is_null = item == null;
        item.Destroy();
        ResetAmount();
        ClearSlot();
        is_null = item == null;
    }

    public void DropItem()
    {
        playerMove p = gameObject.GetComponentInParent<playerMove>();
        Vector2 v = new Vector2(p.transform.position.x, p.transform.position.y + 1);
        item.Drop(v, p.sprite.flipX);
        if (GetAmount() <= 1){
            ClearSlot();
        }
        UpdateAmount(-1);
    }
}
