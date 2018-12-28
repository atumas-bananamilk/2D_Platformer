using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {
    public SpriteRenderer block_crack;
    public Sprite crack_image_0;
    public Sprite crack_image_1;
    public Sprite crack_image_2;

    public enum BLOCK_TYPE{
        WOOD, BRICK, METAL
    }
    public BLOCK_TYPE block_type;
    public float health = 100f;
    public float health_max_wood = 100f;
    public float health_max_brick = 200f;
    public float health_max_metal = 300f;


	public void SetBlockType(int cell_id){
        if (MapManager.block_images.ContainsKey(cell_id)){
            if (cell_id == MapManager.TILE_ID_GROUND_5 ||
                cell_id == MapManager.TILE_ID_GROUND_6 ||
                cell_id == MapManager.TILE_ID_GROUND_7 ||
                cell_id == MapManager.TILE_ID_GROUND_11){
                block_type = BLOCK_TYPE.WOOD;
                health = health_max_wood;
            }
            else if (cell_id == 10){
                block_type = BLOCK_TYPE.BRICK;
                health = health_max_brick;
            }
            else{
                block_type = BLOCK_TYPE.METAL;
                health = health_max_metal;
            }
            block_crack.enabled = false;
            gameObject.GetComponent<SpriteRenderer>().sprite = MapManager.block_images[cell_id];
        }
        //block.GetComponent<SpriteRenderer>().sprite = block_images.ContainsKey(cell_id) ? block_images[cell_id] : ground_11;
    }

	public void Update()
	{
        if (block_type == BLOCK_TYPE.WOOD){
            if (health >= health_max_wood * 0.66 && health < health_max_wood)
            {
                block_crack.enabled = true;
                block_crack.sprite = crack_image_0;
            }
            else if (health >= health_max_wood * 0.33 && health < health_max_wood * 0.66)
            {
                block_crack.enabled = true;
                block_crack.sprite = crack_image_1;
            }
            else if (health >= 0 && health < health_max_wood * 0.33)
            {
                block_crack.enabled = true;
                block_crack.sprite = crack_image_2;
            }
            else if (health < 0){
                Destroy(gameObject);
            }
        }
	}
}