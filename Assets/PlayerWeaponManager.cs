using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerWeaponManager : MonoBehaviour {
    public GameObject player_weapon;
    public GameObject weapon_point;
    public float damage = 0.2f;
    public float range = 10f;
    private Sprite new_weapon_sprite;
    private string weapons_path = "AssetsWeapons/gun_";
    private int new_weapon_number = 13;

	private void Start()
	{
        new_weapon_sprite = Resources.LoadAll<Sprite>(weapons_path + new_weapon_number)[0];
	}

	private void LateUpdate()
    {
        //gameObject.GetComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("AssetsCharacters/Wizard/character_jump_wizard")[0];
        player_weapon.GetComponent<SpriteRenderer>().sprite = new_weapon_sprite;
    }

    public void FlipWeapon(bool flipX){
        player_weapon.GetComponent<SpriteRenderer>().flipX = flipX;
        Quaternion r = player_weapon.GetComponent<SpriteRenderer>().transform.rotation;
        player_weapon.GetComponent<SpriteRenderer>().transform.rotation = Quaternion.Inverse(r);
    }

    public void Shoot(){
        gameObject.GetComponent<ParticleSystem>().Play();
    }
}
