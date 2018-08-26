using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour {
    public GameObject inventory;
    public GameObject inventory_big_panel;

    public void OpenInventory(){
        inventory.SetActive(!inventory.GetActive());
    }

    public void AddToInventory(GameObject item){
        int id = FindWhereToPut();
        if (id != -1){
            inventory_big_panel.GetComponentsInChildren<InventorySlot>()[id].AddToSlot(item.GetComponent<WorldItem>());
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
