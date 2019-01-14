using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWallManager : MonoBehaviour {
    public Sprite wooden_wall;
    public Sprite brick_wall;
    public Sprite metal_wall;
    public GameObject materials;

    public enum MATERIAL_TYPES{
        UNKNOWN, WOOD, BRICK, METAL
    }

    public GameObject wall_hover;
    private MATERIAL_TYPES selected_material;
    private readonly float WALL_UNIT_SIZE = 3f;

    private static Vector2 wall_size_flat = new Vector2(3f, 0.76f);
    private static Vector2 wall_size_diagonal = new Vector2(4.3f, 0.76f);

    private static Vector2 polygon_scale_flat = new Vector2(wall_size_flat.x / 10, wall_size_flat.y / 5);
    private static Vector2 polygon_scale_diagonal = new Vector2(wall_size_diagonal.x / 10, wall_size_diagonal.y / 5);

    public enum WALL_ROTATION{
        HORIZONTAL, DIAGONAL_LEFT, VERTICAL, DIAGONAL_RIGHT
    }

    public Dictionary<WALL_ROTATION, Quaternion> wall_rotations = new Dictionary<WALL_ROTATION, Quaternion>();

	private void Start()
    {
        wall_rotations.Add(WALL_ROTATION.HORIZONTAL, Rotation(0));
        wall_rotations.Add(WALL_ROTATION.VERTICAL, Rotation(90));
        wall_rotations.Add(WALL_ROTATION.DIAGONAL_RIGHT, Rotation(45));
        wall_rotations.Add(WALL_ROTATION.DIAGONAL_LEFT, Rotation(-45));
        SelectWood();
	}

    public void SelectMetal()
    {
        SelectMaterial(MATERIAL_TYPES.METAL, 0);
    }
    public void SelectBrick(){
        SelectMaterial(MATERIAL_TYPES.BRICK, 1);
    }
    public void SelectWood(){
        SelectMaterial(MATERIAL_TYPES.WOOD, 2);
    }
    private void SelectMaterial(MATERIAL_TYPES type, int material_id){
        foreach (Outline o in materials.GetComponentsInChildren<Outline>()){
            o.enabled = false;
        }
        // material_id * 2 - because each child has 2 outlines
        materials.GetComponentsInChildren<Outline>()[material_id * 2].enabled = true;
        selected_material = type;
    }

    public void PlaceNetworkedRealWall(WALL_ROTATION wall_rotation){
        PlaceRealWall(wall_rotation, selected_material);
        //TCPNetwork.PlaceWall(wall_rotation, selected_material);
    }

    public void PlaceRealWall(WALL_ROTATION wall_rotation, MATERIAL_TYPES type){
        WallTransform t = GetWallTransform(wall_rotation);

        Debug.DrawLine(transform.position, t.position);

        if (!TCPPlayer.IsMine(gameObject)) {
            if (GetComponent<playerMove>().direction == playerMove.MOVEMENT_DIRECTION.RIGHT){
                t.position.x += 1;
            }
        }
        if (t != null){
            PlaceWall(false, wall_rotation, t.position, t.rotation, type);
            //TCPNetwork.PlaceWall(wall_rotation, selected_material);
            TCPNetwork.PlaceWall(wall_rotation, t.position, t.rotation, type);
        }
    }

    public void PlaceHoverWall(WALL_ROTATION wall_rotation){
        WallTransform t = GetWallTransform(wall_rotation);
        Destroy(wall_hover);

        if (t != null && TCPPlayer.IsMine(gameObject)){
            PlaceWall(true, wall_rotation, t.position, t.rotation, selected_material);
        }
    }

    public void PlaceWall(bool is_hover, WALL_ROTATION wall_rotation, Vector2 pos, Quaternion rotation, MATERIAL_TYPES type){
        Vector2 wall_size;
        Vector2 polygon_scale;

        if (wall_rotation == WALL_ROTATION.HORIZONTAL || wall_rotation == WALL_ROTATION.VERTICAL){
            wall_size = wall_size_flat;
            polygon_scale = polygon_scale_flat;
        }
        else{
            wall_size = wall_size_diagonal;
            polygon_scale = polygon_scale_diagonal;
        }

        PlaceWallOfType(is_hover, pos, rotation, wall_size, polygon_scale, type);
    }

    private void PlaceWallOfType(bool hover, Vector2 pos, Quaternion rotation, Vector2 wall_size, Vector2 polygon_scale, MATERIAL_TYPES type){
        Sprite wall_sprite = wooden_wall;
        WorldItem.ITEM_NAME item_name = WorldItem.ITEM_NAME.WALL_WOODEN;

        switch (type){
            case MATERIAL_TYPES.WOOD:{
                    wall_sprite = wooden_wall;
                    item_name = WorldItem.ITEM_NAME.WALL_WOODEN;
                    break;
                }
            case MATERIAL_TYPES.BRICK:{
                    wall_sprite = brick_wall;
                    item_name = WorldItem.ITEM_NAME.WALL_BRICK;
                    break;
                }
            case MATERIAL_TYPES.METAL:{
                    wall_sprite = metal_wall;
                    item_name = WorldItem.ITEM_NAME.WALL_METAL;
                    break;
                }
        }
        if (hover){
            wall_hover = GetComponent<WorldItemManager>().AddWallHoverToWorld(item_name, pos, rotation, wall_size, wall_sprite, polygon_scale);
        }
        else{
            gameObject.GetComponent<WorldItemManager>().AddWallToWorld(item_name, pos, rotation, wall_size, wall_sprite, polygon_scale);
        }
    }

    private WallTransform GetWallTransform(WALL_ROTATION rotation){
        Vector2 position = new Vector2(
            GetWallTransformCoordinate(rotation, true), 
            GetWallTransformCoordinate(rotation, false)
        );
        return new WallTransform(rotation, wall_rotations[rotation], position);
    }

    private float GetWallTransformCoordinate(WALL_ROTATION rotation, bool check_x_coordinate){
        playerMove.MOVEMENT_DIRECTION direction = gameObject.GetComponent<playerMove>().direction;
        float offset = 0;
        float start_pos;
        float player_pos;
        int units;

        // horizontal
        if (check_x_coordinate){
            start_pos = gameObject.GetComponent<MapManager>().start_x;

                player_pos = transform.position.x;
            units = (int)(Math.Abs(player_pos - start_pos) / WALL_UNIT_SIZE);
            
            switch (rotation){
                case WALL_ROTATION.VERTICAL:{
                        offset = (direction == playerMove.MOVEMENT_DIRECTION.LEFT) ? 0 : WALL_UNIT_SIZE;
                        break;
                    }
                case WALL_ROTATION.HORIZONTAL:{
                        offset = WALL_UNIT_SIZE / 2;
                        break;
                    }
                case WALL_ROTATION.DIAGONAL_LEFT:{
                        offset = (direction == playerMove.MOVEMENT_DIRECTION.LEFT) ? (-WALL_UNIT_SIZE / 2) : (WALL_UNIT_SIZE + WALL_UNIT_SIZE / 2);
                        break;
                    }
                case WALL_ROTATION.DIAGONAL_RIGHT:{
                        offset = (direction == playerMove.MOVEMENT_DIRECTION.LEFT) ? (-WALL_UNIT_SIZE / 2) : (WALL_UNIT_SIZE + WALL_UNIT_SIZE / 2);
                        break;
                    }
            }

            return start_pos + (WALL_UNIT_SIZE * units) + offset;
        }
        // vertical
        else{
            start_pos = gameObject.GetComponent<MapManager>().start_y;
            player_pos = transform.position.y - 1;
            units = (int)(Math.Abs(player_pos - start_pos) / WALL_UNIT_SIZE);
            
            switch (rotation){
                case WALL_ROTATION.VERTICAL:{
                        offset = 0;
                        break;
                    }
                case WALL_ROTATION.HORIZONTAL:{
                        offset = WALL_UNIT_SIZE / 2;
                        break;
                    }
                case WALL_ROTATION.DIAGONAL_LEFT:{
                        offset = (direction == playerMove.MOVEMENT_DIRECTION.LEFT) ? 0 : WALL_UNIT_SIZE;
                        break;
                    }
                case WALL_ROTATION.DIAGONAL_RIGHT:{
                        offset = (direction == playerMove.MOVEMENT_DIRECTION.LEFT) ? WALL_UNIT_SIZE : 0;
                        break;
                    }
            }

            return start_pos - (WALL_UNIT_SIZE * units) - offset;
        }
    }

    private class WallTransform{
        public WALL_ROTATION wall_rotation;
        public Quaternion rotation;
        public Vector2 position;

        public WallTransform(WALL_ROTATION wall_rotation, Quaternion rotation, Vector2 position){
            this.wall_rotation = wall_rotation;
            this.rotation = rotation;
            this.position = position;
        }
    }

    private Quaternion Rotation(float degrees){
        return Quaternion.Euler(0, 0, degrees);
    }
}
