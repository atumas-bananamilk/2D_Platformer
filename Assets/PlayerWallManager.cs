using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallManager : MonoBehaviour {
    public Sprite wooden_wall;

    public enum WALL_ROTATION{
        TOP, TOP_LEFT, LEFT, LEFT_BOTTOM, BOTTOM, BOTTOM_RIGHT, RIGHT, RIGHT_TOP
    }

    public void PlaceWall(Vector2 position, WALL_ROTATION rotation){
        gameObject.GetComponent<WorldItemManager>().AddWallToWorld(WorldItem.ITEM_NAME.WALL_WOODEN, position, Quaternion.identity, new Vector2(0.1f, 0.1f), wooden_wall);
    }
    public void PlaceWall(WALL_ROTATION wall_rotation){
        //Vector2 wall_pos = new Vector2(transform.position.x + 2, transform.position.y);
        Transform t = GetDegrees(wall_rotation);

        gameObject.GetComponent<WorldItemManager>().AddWallToWorld(WorldItem.ITEM_NAME.WALL_WOODEN, t.position, t.rotation, new Vector2(0.1f, 0.1f), wooden_wall);
    }

    private Quaternion Rotation(float degrees){
        return Quaternion.Euler(0, 0, degrees);
    }

    private Vector2 Position(float x, float y){
        return new Vector2(transform.position.x + x, transform.position.y + y);
    }

    private Transform GetDegrees(WALL_ROTATION rotation){
        Transform t = transform;

        switch (rotation){
            case WALL_ROTATION.TOP:{
                    t.rotation = Rotation(0);
                    t.position = Position(0, -2);
                    break;
                }
            case WALL_ROTATION.TOP_LEFT:{
                    t.rotation = Rotation(-45);
                    t.position = Position(-1, -1);
                    break;
                }
            case WALL_ROTATION.LEFT:{
                    t.rotation = Rotation(-90);
                    t.position = Position(-2, 0);
                    break;
                }
            case WALL_ROTATION.LEFT_BOTTOM:{
                    t.rotation = Rotation(-135);
                    t.position = Position(-1, 1);
                    break;
                }
            case WALL_ROTATION.BOTTOM:{
                    t.rotation = Rotation(-180);
                    t.position = Position(0, 2);
                    break;
                }
            case WALL_ROTATION.BOTTOM_RIGHT:{
                    t.rotation = Rotation(-225);
                    t.position = Position(1, 1);
                    break;
                }
            case WALL_ROTATION.RIGHT:{
                    t.rotation = Rotation(-270);
                    t.position = Position(2, 0);
                    break;
                }
            case WALL_ROTATION.RIGHT_TOP:{
                    t.rotation = Rotation(-315);
                    t.position = Position(1, -1);
                    break;
                }
            default:{
                    break;
                }
        }
        return t;
    }
}
