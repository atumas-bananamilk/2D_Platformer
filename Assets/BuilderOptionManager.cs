using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BuilderOptionManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

    public GameObject builder_options;
    private static int NUMBER_OF_OPTIONS = 8;
    private int OPTION_DEGREE_SPAN = 360 / NUMBER_OF_OPTIONS;

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("ANGLE: " + angle + ", " + quaternion);
        ShowOption(GetQuaternion(GetAngle(transform.position, eventData.position)));
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        ClearOptions();
        TryBuildWall(GetQuaternion(GetAngle(transform.position, eventData.position)));
    }

    public void OnBeginDrag(PointerEventData eventData)
    {

    }

    private float GetAngle(Vector2 p1, Vector2 p2){
        float angle = Mathf.Atan2(p2.y - p1.y, p2.x - p1.x) * Mathf.Rad2Deg;

        if (angle < 0){
            angle += 360;
        }

        return angle;
    }

    private int GetQuaternion(float angle){
        return (int)((angle + 292.5) / OPTION_DEGREE_SPAN) % NUMBER_OF_OPTIONS;
    }

    private void ShowOption(int quaternion){
        ClearOptions();
        Image[] options = builder_options.GetComponentsInChildren<Image>().ToArray();
        options[quaternion * 2].enabled = true;
        options[quaternion * 2 + 1].enabled = true;
    }

    private void ClearOptions(){
        Image[] options = builder_options.GetComponentsInChildren<Image>().ToArray();
        foreach (Image o in options){
            o.enabled = false;
        }
    }

    private void BuildWall(PlayerWallManager.WALL_ROTATION rotation){
        gameObject.GetComponentInParent<PlayerWallManager>().PlaceWall(rotation);
    }

    private void TryBuildWall(int quaternion){
        switch (quaternion){
            case 0:{
                    BuildWall(PlayerWallManager.WALL_ROTATION.TOP);
                    break;
                }
            case 1:{
                    BuildWall(PlayerWallManager.WALL_ROTATION.TOP_LEFT);
                    break;
                }
            case 2:{
                    BuildWall(PlayerWallManager.WALL_ROTATION.LEFT);
                    break;
                }
            case 3:{
                    BuildWall(PlayerWallManager.WALL_ROTATION.LEFT_BOTTOM);
                    break;
                }
            case 4:{
                    BuildWall(PlayerWallManager.WALL_ROTATION.BOTTOM);
                    break;
                }
            case 5:{
                    BuildWall(PlayerWallManager.WALL_ROTATION.BOTTOM_RIGHT);
                    break;
                }
            case 6:{
                    BuildWall(PlayerWallManager.WALL_ROTATION.RIGHT);
                    break;
                }
            case 7:{
                    BuildWall(PlayerWallManager.WALL_ROTATION.RIGHT_TOP);
                    break;
                }
            default:{
                    break;
                }
        }
    }
}
