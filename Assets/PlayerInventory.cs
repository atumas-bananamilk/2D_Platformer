using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour {
    public GameObject inventory;
    public GameObject inventory_small_panel;
    public GameObject inventory_big_panel;

    public void ToggleToolSlot(bool enabled){
        inventory_small_panel.GetComponentsInChildren<Outline>()[0].enabled = enabled;
        ToggleItemSlot(!enabled);
    }

    public void ToggleItemSlot(bool enabled){
        Outline[] item_outlines = inventory_big_panel.GetComponentsInChildren<Outline>();
        foreach (Outline o in item_outlines){
            o.enabled = false;
        }

        if (enabled){
            item_outlines[0].enabled = true;
        }
    }

    public void OpenInventory(){
        inventory.SetActive(!inventory.GetActive());
    }

    public void DropItem(){
        
    }

    public void AddToInventory(GameObject item){
        int id = FindWhereToPut();
        if (id != -1){
            //inventory_big_panel.GetComponentsInChildren<InventorySlot>()[id].AddToSlot(item.GetComponent<WorldItem>());
        }
        else{
            Debug.Log("INVENTORY FULL");
        }
    }

    private int FindWhereToPut(){
        InventorySlot[] slots = inventory_big_panel.GetComponentsInChildren<InventorySlot>();

        // checking if item already exists in inventory - then put there
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item && slots[i].item.item_name == WorldItem.ITEM_NAME.COIN_GOLD)
            {
                return i;
            }
        }

        // checking if empty slot - then put there
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].IsEmpty())
            {
                return i;
            }
        }

        return -1;
    }
}