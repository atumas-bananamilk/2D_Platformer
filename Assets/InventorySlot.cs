using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDragHandler, IEndDragHandler, IDropHandler {
    public Image slot_icon;
    public Button remove_button;
    public Image slot_amount;
    public WorldItem item;

    public void OnDrag(PointerEventData eventData)
    {
        slot_icon.transform.position = Input.mousePosition;
        remove_button.interactable = false;
        slot_amount.enabled = false;
        slot_amount.GetComponentInChildren<Text>().enabled = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        slot_icon.transform.localPosition = Vector3.zero;
        remove_button.interactable = true;
        slot_amount.enabled = true;
        slot_amount.GetComponentInChildren<Text>().enabled = true;
    }

    public void OnDrop(PointerEventData eventData)
    {
        InventorySlot[] slots = transform.parent.gameObject.GetComponentsInChildren<InventorySlot>();

        Debug.Log("SLOTS: "+slots.Length);

        foreach (InventorySlot slot in slots){
            if (RectTransformUtility.RectangleContainsScreenPoint(slot.gameObject.transform as RectTransform, Input.mousePosition))
            {
                InventorySlot dragged_from = eventData.lastPress.GetComponentInParent<InventorySlot>();

                //slot.item = dragged_from.item;
                //slot.slot_icon.sprite = dragged_from.slot_icon.sprite;
                //slot.slot_icon.enabled = true;
                //slot.remove_button.interactable = true;
                //slot.slot_amount.enabled = true;
                //slot.slot_amount.GetComponentInChildren<Text>().enabled = true;
                //slot.SetAmount(dragged_from.GetAmount());

                //slot.AddToSlot(dragged_from.item);

                dragged_from.RemoveItem();
            }
        }
        //ClearSlot();
    }

    private void CreateNewItem(int amount){
        item = gameObject.AddComponent<WorldItem>();
        item.amount = amount;
    }

    public void AddToSlot(WorldItem i){
        // if something of this type is already in this slot - just add amount
        if (item && item.item_name == i.item_name){
            UpdateAmount(i.amount);
        }
        // otherwise - put new item into inventory
        else{
            //ResetAmount();
            CreateNewItem(i.amount);
            item.icon = i.GetSprite();
            slot_icon.sprite = item.icon;
            slot_icon.enabled = true;
            remove_button.interactable = true;
            remove_button.gameObject.SetActive(true);
        }
    }

    private void SetAmount(int amount){
        slot_amount.GetComponentInChildren<Text>().text = amount.ToString();
    }

    private void ResetAmount(){
        Text amount_text = slot_amount.GetComponentInChildren<Text>();
        slot_amount.enabled = false;
        slot_amount.gameObject.SetActive(false);
        amount_text.enabled = false;
        amount_text.text = "1";
    }

    private void UpdateAmount(int amount){
        Text amount_text = slot_amount.GetComponentInChildren<Text>();
        item.amount += amount;
        if (item.amount > 1){
            slot_amount.enabled = true;
            slot_amount.gameObject.SetActive(true);
            amount_text.enabled = true;
            SetAmount(item.amount);
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
        remove_button.gameObject.SetActive(false);
        slot_amount.enabled = false;
        slot_amount.gameObject.SetActive(false);
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
        item.Destroy();
        ResetAmount();
        ClearSlot();
    }

    public void DropItem()
    {
        playerMove p = gameObject.GetComponentInParent<playerMove>();
        Vector2 v = new Vector2(p.transform.position.x, p.transform.position.y + 1);
        item.Drop(v, p.sprite.flipX, 1);
        //if (GetAmount() <= 1){
        if (item.amount <= 1){
            ResetAmount();
            ClearSlot();
        }
        UpdateAmount(-1);
    }
}
