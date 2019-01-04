using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropManager : MonoBehaviour {

    public void DropRandomizedItems(Vector2 pos){
        int p = GetRandomInt(0, 100);

        // 70% - 0 items
        // 20% - 1 item
        // 10% - 2 items
        if (p >= 0 && p < 1){
            // drop nothing
        }
        else if (p >= 1 && p < 90){
            DropRandomItem(pos);
        }
        else{
            DropRandomItem(pos);
            DropRandomItem(pos);
        }
    }

    private void DropRandomItem(Vector2 pos){
        WorldItem item = GenerateRandomItem();
        DropItem(pos, item);
    }

    private WorldItem GenerateRandomItem(){
        Sprite[] sprites;
        WorldItem item = new WorldItem();
        int p = GetRandomInt(0, 100);
        
        // 30% - gun
        // 10% - grenade
        // 30% - health
        // 30% - shield
        if (p >= 0 && p < 30){
            sprites = Resources.LoadAll <Sprite> ("AssetsWeapons");
            item.item_type = WorldItem.ITEM_TYPE.GUN;
        }
        else if (p >= 30 && p < 40){
            sprites = Resources.LoadAll <Sprite> ("AssetsGrenades");
            item.item_type = WorldItem.ITEM_TYPE.GRENADE;
        }
        else if (p >= 40 && p < 70){
            sprites = Resources.LoadAll<Sprite>("Items/Health");
            item.item_type = WorldItem.ITEM_TYPE.HEALTH;
        }
        else{
            sprites = Resources.LoadAll<Sprite>("Items/Shield");
            item.item_type = WorldItem.ITEM_TYPE.SHIELD;
        }
        Sprite s = sprites[GetRandomInt(0, sprites.Length - 1)];
        item.SetSprite(s);

        return item;
    }

    public void DropItem(Vector2 pos, WorldItem item){
        if (item == null){
            //item = new WorldItem();
            //item.SetSprite(null);
        }

        Vector2 direction = (GetRandomInt(0, 1) == 0) ? Vector2.right : Vector2.left;
        GetComponent<WorldItemManager>().DropItemToWorld(pos, item, direction);
    }

    private int GetRandomInt(int min, int max){
        int r = Random.Range(min, max + 1);
        return r;
    }
}