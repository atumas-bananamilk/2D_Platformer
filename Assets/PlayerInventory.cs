using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour {
    public GameObject inventory;

    public void OpenInventory(){
        inventory.SetActive(!inventory.GetActive());
    }
}
