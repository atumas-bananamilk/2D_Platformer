using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour {
    public Image slot_icon;
    public Button remove_button;
    private WorldItem item;

    private void CreateNewItem(){
        item = gameObject.AddComponent<WorldItem>();
    }

    public void AddToSlot(WorldItem i){
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
        Debug.Log("USING");
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
        Debug.Log("IS NULL BEFORE: " + is_null);
        item.Destroy();
        ClearSlot();
        is_null = item == null;
        Debug.Log("IS NULL AFTER: "+is_null);
    }

    public void DropItem()
    {
        playerMove p = gameObject.GetComponentInParent<playerMove>();
        Vector2 v = new Vector2(p.transform.position.x, p.transform.position.y + 1);
        item.Drop(v, p.sprite.flipX);
    }
}
