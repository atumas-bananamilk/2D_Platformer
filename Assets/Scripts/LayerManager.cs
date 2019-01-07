using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerManager {
    // PickUpItem
    public static readonly int PLAYER = StringToLayer("Player");
    public static readonly int PICK_UP_ITEM = StringToLayer("PickUpItem");
    public static readonly int CHEST = StringToLayer("Chest");
    public static readonly int UNKNOWN = StringToLayer("Unknown");

    public static int StringToLayer(string s){
        return LayerMask.NameToLayer(s);
    }
}