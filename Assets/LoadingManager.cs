using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour {
    public GameObject loading_panel;
    public Image loading_circle;
    public SpriteRenderer loading_center;
    public Sprite loading_key;
    public Sprite loading_syringe;
    public Sprite loading_medkit;
    public Sprite loading_potion_shield_small;
    public Sprite loading_potion_shield_big;

    public enum LOADING_TYPE{
        CHEST, HEALTH_SMALL, HEALTH_BIG, SHIELD_SMALL, SHIELD_BIG
    }

    public void UpdateLoadingCircle(LOADING_TYPE type, int mouse_down_count){
        loading_circle.fillAmount = (float) mouse_down_count / GetMouseDownLimit(type);
    }

    public void SetLoadingPanelVisible(bool active){
        if (loading_panel.active != active){
            loading_panel.SetActive(active);
        }
    }

    public void SetupLoadingPanel(LOADING_TYPE type, Vector2 pos){
        //loading_panel.GetComponent<RectTransform>().localScale = new Vector2(0.8f, 0.8f);
        switch (type){
            case LOADING_TYPE.CHEST:{
                    loading_center.sprite = loading_key;
                    break;
                }
            case LOADING_TYPE.HEALTH_SMALL:{
                    loading_center.sprite = loading_syringe;
                    break;
                }
            case LOADING_TYPE.HEALTH_BIG:{
                    loading_center.sprite = loading_medkit;
                    break;
                }
            case LOADING_TYPE.SHIELD_SMALL:{
                    loading_center.sprite = loading_potion_shield_small;
                    break;
                }
            case LOADING_TYPE.SHIELD_BIG:{
                    loading_center.sprite = loading_potion_shield_big;
                    break;
                }
        }
        loading_circle.transform.position = pos;
    }

    public int GetMouseDownLimit(LOADING_TYPE type){
        switch (type){
            case LOADING_TYPE.CHEST:{
                    return 200;
                }
            case LOADING_TYPE.HEALTH_SMALL:{
                    return 80;
                }
            case LOADING_TYPE.HEALTH_BIG:{
                    return 120;
                }
            case LOADING_TYPE.SHIELD_SMALL:{
                    return 80;
                }
            case LOADING_TYPE.SHIELD_BIG:{
                    return 120;
                }
        }
        return 200;
    }
}