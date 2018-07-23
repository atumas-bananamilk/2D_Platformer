using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public CharacterStats_SO characterDefinition;
    public CharacterInventory charInv;
    public GameObject characterWeaponSlot;

    #region Constructors
    public CharacterStats()
    {
        charInv = CharacterInventory.Instance;
    }
    #endregion

    #region Initialization
    void Start()
    {
        if (!characterDefinition.setManually)
        {
            characterDefinition.maxHealth = 100;
            characterDefinition.currentHealth = 50;

            characterDefinition.maxMana = 25;
            characterDefinition.currentMana = 10;

            characterDefinition.maxWealth = 500;
            characterDefinition.currentWealth = 0;

            characterDefinition.baseDamage = 2;
            characterDefinition.currentDamaage = characterDefinition.baseDamage;
            
            characterDefinition.baseResistance = 0;
            characterDefinition.currentResistance = 0;

            characterDefinition.maxEncumbrance = 50f;
            characterDefinition.currentEncumbrance = 0f;

            characterDefinition.charExperience = 0;
            characterDefinition.charLevel = 1;
        }
    }
    #endregion

    #region Stat Increasers
    public void ApplyHealth(int healthAmouunt)
    {
        characterDefinition.ApplyHealth(healthAmouunt);
    }

    public void ApplyMana(int manaAmount)
    {
        characterDefinition.ApplyMana(manaAmount);
    }

    public void GiveWealth(int wealthAmount)
    {
        characterDefinition.GiveWealth(wealthAmount);
    }
    #endregion

    #region Weapon and Armor Changes
    public void ChangeWeapon(ItemPickUp weaponPickUp)
    {
        if (!characterDefinition.UnEquipWeapon(weaponPickUp, charInv, characterWeaponSlot))
        {
            characterDefinition.EquipWeapon(weaponPickUp, charInv, characterWeaponSlot);
        }
    }

    public void ChangeArmor(ItemPickUp armorPickUp)
    {
        if (!characterDefinition.UnEquipArmor(armorPickUp, charInv))
        {
            characterDefinition.EquipArmor(armorPickUp, charInv);
        }
    }
    #endregion

    #region Stat Reducers
    public void TakeDamage(int amount)
    {
        characterDefinition.TakeDamage(amount);
    }

    public void TakeMana(int amount)
    {
        characterDefinition.TakeMana(amount);
    }
    #endregion

    #region Reporters
    public int GetMaxHealth()
    {
        return characterDefinition.maxHealth;
    }

    public int GetCurrentHealth()
    {
        return characterDefinition.currentHealth;
    }

    public int GetMaxMana()
    {
        return characterDefinition.maxMana;
    }

    public int GetCurrentMana()
    {
        return characterDefinition.currentMana;
    }

    public int GetMaxWealth()
    {
        return characterDefinition.maxWealth;
    }

    public int GetCurrentWealth()
    {
        return characterDefinition.currentWealth;
    }

    public int GetBaseDamage()
    {
        return characterDefinition.baseDamage;
    }

    public int GetCurrentDamage()
    {
        return characterDefinition.currentDamaage;
    }

    public float GetMaxEncumbrance()
    {
        return characterDefinition.maxEncumbrance;
    }

    public float GetCurrentEncumbrance()
    {
        return characterDefinition.currentEncumbrance;
    }

    public float GetBaseResistance()
    {
        return characterDefinition.baseResistance;
    }

    public float GetCurrentResistance()
    {
        return characterDefinition.currentResistance;
    }
    
    public int GetCurrentLevel()
    {
        return characterDefinition.charLevel;
    }

    public ItemPickUp GetHeadArmor()
    {
        return characterDefinition.headArmor;
    }

    public ItemPickUp GetChestArmor()
    {
        return characterDefinition.chestArmor;
    }

    public ItemPickUp GetHandArmor()
    {
        return characterDefinition.handArmor;
    }

    public ItemPickUp GetLegArmor()
    {
        return characterDefinition.legArmor;
    }

    public ItemPickUp GetFootArmor()
    {
        return characterDefinition.footArmor;
    }

    public ItemPickUp GetCurrentWeapon()
    {
        return characterDefinition.weapon;
    }
    #endregion  
}
