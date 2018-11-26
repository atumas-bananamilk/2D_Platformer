using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallManager : MonoBehaviour {
    public Sprite wooden_wall;
    public GameObject wall_hover;
    private readonly float WALL_UNIT_SIZE = 3f;
    private Vector2 wall_size_flat = new Vector2(0.3f, 0.18f);
    private Vector2 wall_size_diagonal = new Vector2(0.42f, 0.18f);

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
	}

	public void PlaceWall(WALL_ROTATION wall_rotation){
        WallTransform t = GetWallTransform(wall_rotation);

        if (t != null){
            if (wall_rotation == WALL_ROTATION.HORIZONTAL || wall_rotation == WALL_ROTATION.VERTICAL){
                gameObject.GetComponent<WorldItemManager>().AddWallToWorld(WorldItem.ITEM_NAME.WALL_WOODEN, t.position, t.rotation, wall_size_flat, wooden_wall);
            }
            else{
                gameObject.GetComponent<WorldItemManager>().AddWallToWorld(WorldItem.ITEM_NAME.WALL_WOODEN, t.position, t.rotation, wall_size_diagonal, wooden_wall);
            }
        }
    }

    public void PlaceHoverWall(WALL_ROTATION wall_rotation){
        WallTransform t = GetWallTransform(wall_rotation);
        Destroy(wall_hover);
        if (t != null){
            wall_hover = gameObject.GetComponent<WorldItemManager>().AddWallHoverToWorld(WorldItem.ITEM_NAME.WALL_WOODEN, t.position, t.rotation, new Vector2(0.3f, 0.18f), wooden_wall);
        }
    }

    private WallTransform GetWallTransform(WALL_ROTATION rotation){
        Vector2 position = new Vector2(
            GetWallTransformCoordinate(rotation, true), 
            GetWallTransformCoordinate(rotation, false)
        );
        return new WallTransform(rotation, wall_rotations[rotation], position);
    }

    //private float GetWallTransform_Y_Coordinate(WALL_ROTATION rotation){
    //    float offset = 0;
    //    float start_coordinate = gameObject.GetComponent<MapManager>().start_y;
    //    int units = (int)(Math.Abs(transform.position.y - start_coordinate) / WALL_UNIT_SIZE);
    //    playerMove.MOVEMENT_DIRECTION direction = gameObject.GetComponent<playerMove>().direction;

    //    switch (rotation){
    //        case WALL_ROTATION.VERTICAL:{
    //                offset = 0;
    //                break;
    //            }
    //        case WALL_ROTATION.HORIZONTAL:{
    //                offset = WALL_UNIT_SIZE / 2;
    //                break;
    //            }
    //        case WALL_ROTATION.DIAGONAL_LEFT:{
    //                offset = (direction == playerMove.MOVEMENT_DIRECTION.LEFT) ? 0 : WALL_UNIT_SIZE;
    //                break;
    //            }
    //        case WALL_ROTATION.DIAGONAL_RIGHT:{
    //                offset = (direction == playerMove.MOVEMENT_DIRECTION.LEFT) ? WALL_UNIT_SIZE : 0;
    //                break;
    //            }
    //    }

    //    float y = start_coordinate - (WALL_UNIT_SIZE * units) - offset;
    //    return y;
    //}

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
            player_pos = transform.position.y;
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
