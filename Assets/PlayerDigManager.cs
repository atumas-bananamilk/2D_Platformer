using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDigManager : MonoBehaviour {
    public GameObject player_pickaxe;

    public enum DIG_DIRECTION{
        BOTTOM, SIDE, TOP
    }

    public void Dig(DIG_DIRECTION d){
        player_pickaxe.SetActive(true);
        GetComponent<PlayerWeaponManager>().player_weapon.SetActive(false);

        switch (d){
            case DIG_DIRECTION.BOTTOM:{
                    gameObject.GetComponent<Animator>().Play(AnimatorManager.PLAYER_DIG_BOTTOM);
                    break;
                }
            case DIG_DIRECTION.SIDE:{
                    gameObject.GetComponent<Animator>().Play(AnimatorManager.PLAYER_DIG_SIDE);
                    break;
                }
            case DIG_DIRECTION.TOP:{
                    gameObject.GetComponent<Animator>().Play(AnimatorManager.PLAYER_DIG_TOP);
                    break;
                }
        }
        GetComponent<PlayerInventory>().ToggleToolSlot(true);
    }

    public void StopDig(){
        player_pickaxe.SetActive(false);
        GetComponent<PlayerWeaponManager>().player_weapon.SetActive(true);
        GetComponent<playerMove>().ChangePlayerState(playerMove.PLAYERSTATE.IDLE);
        GetComponent<PlayerInventory>().ToggleToolSlot(false);
    }

    public void FlipPickaxe(bool flipX){
        player_pickaxe.GetComponent<SpriteRenderer>().flipX = flipX;
        Quaternion r = player_pickaxe.GetComponent<SpriteRenderer>().transform.rotation;
        player_pickaxe.GetComponent<SpriteRenderer>().transform.rotation = Quaternion.Inverse(r);
    }
}
