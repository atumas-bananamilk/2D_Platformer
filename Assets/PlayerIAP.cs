using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIAP : MonoBehaviour {
    public void Buy_SP(int amount)
    {
        IAPManager.Instance.Buy_SP(amount);
    }
}