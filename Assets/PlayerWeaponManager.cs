using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerWeaponManager : MonoBehaviour {
    public GameObject player_weapon;
    public GameObject weapon_point;
    public GameObject bullet_prefab;
    private Sprite new_weapon_sprite;

    public float range = 10f;
    public float target_x = 10000;
    public float weapon_point_distance_x;
    public float weapon_point_distance_y;
    Vector2 weapon_target;

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
        // flip weapon sprite
        player_weapon.GetComponent<SpriteRenderer>().flipX = flipX;
        Quaternion r = player_weapon.GetComponent<SpriteRenderer>().transform.rotation;
        player_weapon.GetComponent<SpriteRenderer>().transform.rotation = Quaternion.Inverse(r);
    }

    public void TryFlipWeaponShootVector()
    {
        float weapon_point_x;
        float weapon_point_y;

        if (GetComponent<playerMove>().direction == playerMove.MOVEMENT_DIRECTION.LEFT){
            weapon_point_x = transform.position.x - weapon_point_distance_x;
        }
        else{
            weapon_point_x = transform.position.x + weapon_point_distance_x;
        }
        weapon_point_y = transform.position.y - weapon_point_distance_y;

        // flip weapon point
        weapon_point.transform.position = new Vector2(weapon_point_x, weapon_point_y);
    }

    public void Shoot(){
        TryFlipWeaponShootVector();

        Vector2 bullet_pos = weapon_point.transform.position;
        bullet_pos.y += UnityEngine.Random.Range(-0.1f, 0.1f);

        GameObject obj = Instantiate(bullet_prefab, bullet_pos, Quaternion.identity) as GameObject;
        obj.GetComponent<Bullet>().ChangeDirection(GetComponent<playerMove>().direction);
    }

    public void HideWeapon(bool hide){
        player_weapon.GetComponent<SpriteRenderer>().enabled = !hide;
    }
}