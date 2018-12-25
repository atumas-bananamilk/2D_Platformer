using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {
    public enum BLOCK_TYPE{
        WOOD, BRICK, METAL
    }
    public BLOCK_TYPE block_type;

	public void SetBlockType(int cell_id){
        if (MapManager.block_images.ContainsKey(cell_id)){
            if (cell_id == MapManager.TILE_ID_GROUND_5 ||
                cell_id == MapManager.TILE_ID_GROUND_6 ||
                cell_id == MapManager.TILE_ID_GROUND_7 ||
                cell_id == MapManager.TILE_ID_GROUND_11){
                gameObject.GetComponent<Block>().block_type = BLOCK_TYPE.WOOD;
            }
            else if (cell_id == 10){
                gameObject.GetComponent<Block>().block_type = BLOCK_TYPE.BRICK;
            }
            else{
                gameObject.GetComponent<Block>().block_type = BLOCK_TYPE.METAL;
            }
            gameObject.GetComponent<SpriteRenderer>().sprite = MapManager.block_images[cell_id];
        }
        //block.GetComponent<SpriteRenderer>().sprite = block_images.ContainsKey(cell_id) ? block_images[cell_id] : ground_11;
    }
}