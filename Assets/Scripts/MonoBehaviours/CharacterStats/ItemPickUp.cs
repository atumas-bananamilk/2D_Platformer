using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public ItemPickUps_SO itemDefinition;

    public CharacterStats charStats;
    CharacterInventory charInventory;

    GameObject foundStats;

    #region Constructors
    public ItemPickUp()
    {
        charInventory = CharacterInventory.Instance;
    }
    #endregion

    void Start()
    {
        if (charStats != null)
        {
            foundStats = GameObject.FindGameObjectWithTag("Player");
            charStats = foundStats.GetComponent<CharacterStats>();
        }
    }

    void StoreItemInInventory()
    {
        charInventory.StoreItem(this);
    }

    public void UseItem()
    {
        switch (itemDefinition.itemType)
        {
            case ItemTypeDefinition.HEALTH:
                charStats.ApplyHealth(itemDefinition.itemAmount);
                Debug.Log(charStats.GetCurrentHealth());
                break;

            case ItemTypeDefinition.MANA:
                charStats.ApplyMana(itemDefinition.itemAmount);
                break;

            case ItemTypeDefinition.WEALTH:
                charStats.GiveWealth(itemDefinition.itemAmount);
                break;

            case ItemTypeDefinition.WEAPON:
                charStats.ChangeWeapon(this);
                break;

            case ItemTypeDefinition.ARMOR:
                charStats.ChangeArmor(this);
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
         if (other.tag == "Player")
        {
            if (itemDefinition.isStorable)
            {
                StoreItemInInventory();
            }
            else
            {
                UseItem();
            }
        }
    }

}
