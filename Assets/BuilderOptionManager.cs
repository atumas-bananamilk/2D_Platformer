using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BuilderOptionManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

    public GameObject builder_options;
    private Dictionary<int, PlayerWallManager.WALL_ROTATION> wall_selections = new Dictionary<int, PlayerWallManager.WALL_ROTATION>();
    private static int NUMBER_OF_OPTIONS = 8;
    private int OPTION_DEGREE_SPAN = 360 / NUMBER_OF_OPTIONS;

    private bool dragging = false;
    private int qq = 0;

    void Start(){
        SetupWallSelections();
    }

	private void Update()
	{
        if (dragging){
            HoverWall(qq);
        }
	}

	private void SetupWallSelections(){
        wall_selections.Add(0, PlayerWallManager.WALL_ROTATION.TOP);
        wall_selections.Add(1, PlayerWallManager.WALL_ROTATION.TOP_LEFT);
        wall_selections.Add(2, PlayerWallManager.WALL_ROTATION.LEFT);
        wall_selections.Add(3, PlayerWallManager.WALL_ROTATION.LEFT_BOTTOM);
        wall_selections.Add(4, PlayerWallManager.WALL_ROTATION.BOTTOM);
        wall_selections.Add(5, PlayerWallManager.WALL_ROTATION.BOTTOM_RIGHT);
        wall_selections.Add(6, PlayerWallManager.WALL_ROTATION.RIGHT);
        wall_selections.Add(7, PlayerWallManager.WALL_ROTATION.RIGHT_TOP);
    }

    public void OnDrag(PointerEventData eventData)
    {
        dragging = true;
        int quaternion = GetQuaternion(GetAngle(transform.position, eventData.position));
        qq = quaternion;
        ShowOption(quaternion);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dragging = false;
        Destroy(gameObject.GetComponentInParent<PlayerWallManager>().wall_hover);
        ClearOptions();
        BuildWall(GetQuaternion(GetAngle(transform.position, eventData.position)));
    }

    public void OnBeginDrag(PointerEventData eventData)
    {

    }

    public float GetAngle(Vector2 p1, Vector2 p2){
        float angle = Mathf.Atan2(p2.y - p1.y, p2.x - p1.x) * Mathf.Rad2Deg;

        if (angle < 0){
            angle += 360;
        }

        return angle;
    }

    public int GetQuaternion(float angle){
        return (int)((angle + 292.5) / OPTION_DEGREE_SPAN) % NUMBER_OF_OPTIONS;
    }

    private void BuildWall(int quaternion){
        gameObject.GetComponentInParent<PlayerWallManager>().PlaceWall(wall_selections[quaternion]);
    }

    private void HoverWall(int quaternion){
        gameObject.GetComponentInParent<PlayerWallManager>().PlaceHoverWall(wall_selections[quaternion]);
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
}