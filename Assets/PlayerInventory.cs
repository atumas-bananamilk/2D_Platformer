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
        inventory_big_panel.GetComponentsInChildren<InventorySlot>()[0].AddItem(item.GetComponent<WorldItem>());
    }
}
