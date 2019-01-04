using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Threading;

//public class InventorySlot : MonoBehaviour, IDragHandler, IEndDragHandler, IDropHandler {
public class InventorySlot : MonoBehaviour, IDragHandler, IEndDragHandler {
    public Image slot_icon;
    public Button remove_button;
    public Image slot_amount;
    public Image slot_glow;
    public WorldItem item;

    public void OnDrag(PointerEventData eventData)
    {
        if (item)
        {
            slot_icon.transform.position = Input.mousePosition;
            SetSlotVisibility(false);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (item)
        {
            slot_icon.transform.localPosition = Vector3.zero;
            SetSlotVisibility(true);
        }
    }

    //public void OnDrop(PointerEventData eventData)
    //{
    //    foreach (InventorySlot slot in transform.parent.gameObject.GetComponentsInChildren<InventorySlot>()){
    //        // check if dragged into any slot
    //        if (RectTransformUtility.RectangleContainsScreenPoint(slot.gameObject.transform as RectTransform, Input.mousePosition))
    //        {
    //            InventorySlot dragged_from = eventData.lastPress.GetComponentInParent<InventorySlot>();

    //            if (dragged_from.item && slot != dragged_from){
    //                slot.AddToSlotAfterDragged(dragged_from.item);
    //                dragged_from.RemoveItem();
    //            }
    //        }
    //    }
    //}

    private void CreateNewItem(int amount){
        item = gameObject.AddComponent<WorldItem>();
        SetAmount(amount);
    }

    //public void AddToSlot(WorldItem i){
    //    // if something of this type is already in this slot - just add amount
    //    if (item && item.item_name == i.item_name){
    //        SetAmount(item.amount + i.amount);
    //    }
    //    // otherwise - put new item into inventory
    //    else{
    //        CreateNewItem(i.amount);
    //        SetSlotIcon(i.GetSprite());
    //        item.icon = i.GetSprite();
    //    }
    //}

    //private void AddToSlotAfterDragged(WorldItem i){
    //    CreateNewItem(i.amount);
    //    SetSlotIcon(i.icon);
    //    item.icon = i.icon;
    //}

    //private void SetSlotIcon(Sprite s)
    //{
    //    slot_icon.sprite = s;
    //    slot_icon.enabled = s;
    //    remove_button.interactable = s;
    //    remove_button.gameObject.SetActive(s);
    //}

    private void SetAmount(int amount){
        //SetAmountVisibility(amount > 1);
        //slot_amount.GetComponentInChildren<Text>().text = amount.ToString();
        item.amount = amount;
    }

    private void SetSlotVisibility(bool v)
    {
        remove_button.interactable = v;
        slot_amount.enabled = v;
        slot_amount.GetComponentInChildren<Text>().enabled = v;
    }

    private void SetAmountVisibility(bool v){
        slot_amount.enabled = v;
        slot_amount.gameObject.SetActive(v);
        slot_amount.GetComponentInChildren<Text>().enabled = v;
    }

    private void ResetAmount(){
        SetAmount(1);
    }

    //public void SelectItem(){
    //    // select this slot
    //    slot_glow.enabled = !slot_glow.enabled;
    //    gameObject.GetComponentInParent<PlayerInventory>().ToggleDropButton();
    //}

    //public void UseItem()
    //{
    //    if (item != null)
    //    {
    //        //item.Use();
    //        DropItem();
    //    }
    //}

    //public void DropItem()
    //{
    //    if (item != null)
    //    {
    //        playerMove p = gameObject.GetComponentInParent<playerMove>();
    //        Vector2 v = new Vector2(p.transform.position.x, p.transform.position.y + 1);

    //        item.Drop(v, p.sprite.flipX, 1);
    //        if (item.amount <= 1)
    //        {
    //            ResetAmount();
    //            ClearSlot();
    //        }
    //        SetAmount(item.amount - 1);
    //    }
    //}

    //public void RemoveItem()
    //{
    //    if (item){
    //        item.Destroy();
    //    }
    //    ResetAmount();
    //    ClearSlot();
    //}

    //public void ClearSlot()
    //{
    //    Destroy(item);
    //    SetSlotIcon(null);
    //    slot_amount.enabled = false;
    //    slot_amount.gameObject.SetActive(false);
    //}

    public bool IsEmpty()
    {
        return item == null;
    }
}