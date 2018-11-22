using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallManager : MonoBehaviour {
    public Sprite wooden_wall;
    public GameObject wall_hover;

    public enum WALL_ROTATION{
        TOP, TOP_LEFT, LEFT, LEFT_BOTTOM, BOTTOM, BOTTOM_RIGHT, RIGHT, RIGHT_TOP
    }

	public void PlaceWall(WALL_ROTATION wall_rotation){
        WallTransform t = GetWall(wall_rotation);
        if (t != null){
            gameObject.GetComponent<WorldItemManager>().AddWallToWorld(WorldItem.ITEM_NAME.WALL_WOODEN, t.position, t.rotation, new Vector2(0.3f, 0.18f), wooden_wall);
        }
    }

    public void PlaceHoverWall(WALL_ROTATION wall_rotation){
        WallTransform t = GetWall(wall_rotation);
        Destroy(wall_hover);
        if (t != null){
            wall_hover = gameObject.GetComponent<WorldItemManager>().AddWallHoverToWorld(WorldItem.ITEM_NAME.WALL_WOODEN, t.position, t.rotation, new Vector2(0.3f, 0.18f), wooden_wall);
        }
    }

    private WallTransform GetWall(WALL_ROTATION wall_rotation){
        WallTransform t = GetWallTransform(wall_rotation);

        int oversize_amount = 3;

        int offset_x = (int)(Math.Abs(transform.position.x - gameObject.GetComponent<MapManager>().start_x) / oversize_amount);
        float x = t.offset.x + (float)(gameObject.GetComponent<MapManager>().start_x + oversize_amount * offset_x + 1.5f);

        int offset_y = (int)(Math.Abs(transform.position.y - gameObject.GetComponent<MapManager>().start_y) / oversize_amount);
        float y = t.offset.y + (float)(gameObject.GetComponent<MapManager>().start_y - oversize_amount * offset_y);

        t.position = new Vector3(x, y, t.position.z);

        //if (t.wall_rotation == WALL_ROTATION.LEFT && x < transform.position.x){
        //    return t;
        //}
        //else{
        //    return null;
        //}
        
        return t;
    }

    private WallTransform GetWallTransform(WALL_ROTATION rotation){
        float dist_0 = 1.3f;
        float dist_1 = 1;

        switch (rotation){
            case WALL_ROTATION.TOP:{
                    return new WallTransform(rotation, Rotation(0), Position(0, dist_0), transform);
                }
            case WALL_ROTATION.TOP_LEFT:{
                    return new WallTransform(rotation, Rotation(-135), Position(-dist_1, dist_1), transform);
                }
            case WALL_ROTATION.LEFT:{
                    return new WallTransform(rotation, Rotation(-90), Position(-dist_0, 0), transform);
                }
            case WALL_ROTATION.LEFT_BOTTOM:{
                    return new WallTransform(rotation, Rotation(-45), Position(-dist_1, -dist_1), transform);
                }
            case WALL_ROTATION.BOTTOM:{
                    return new WallTransform(rotation, Rotation(-180), Position(0, -dist_0), transform);
                }
            case WALL_ROTATION.BOTTOM_RIGHT:{
                    return new WallTransform(rotation, Rotation(-315), Position(dist_1, -dist_1), transform);
                }
            case WALL_ROTATION.RIGHT:{
                    return new WallTransform(rotation, Rotation(-270), Position(dist_0, 0), transform);
                }
            case WALL_ROTATION.RIGHT_TOP:{
                    return new WallTransform(rotation, Rotation(-225), Position(dist_1, dist_1), transform);
                }
            default:{
                    return new WallTransform(rotation, Rotation(0), Position(0, -dist_0), transform);
                }
        }
    }

    private Quaternion Rotation(float degrees){
        return Quaternion.Euler(0, 0, degrees);
    }

    private Vector2 Position(float x, float y){
        return new Vector2(x, y);
    }

    private class WallTransform{
        public WALL_ROTATION wall_rotation;
        public Quaternion rotation;
        public Vector3 position;
        public Vector2 offset;

        public WallTransform(WALL_ROTATION wall_rotation, Quaternion rotation, Vector2 offset, Transform transform){
            this.wall_rotation = wall_rotation;
            this.rotation = rotation;
            this.offset = offset;
            this.position = new Vector2(transform.position.x + offset.x, transform.position.y + offset.y);
        }
    }
}
