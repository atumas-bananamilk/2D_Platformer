using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BuilderOptionManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

    public GameObject builder_options;
    private Dictionary<int, PlayerWallManager.WALL_ROTATION> wall_selections = new Dictionary<int, PlayerWallManager.WALL_ROTATION>();
    private static int NUMBER_OF_OPTIONS = 4;
    private int OPTION_DEGREE_SPAN = 360 / NUMBER_OF_OPTIONS;

    private bool dragging = false;
    private int quaternion = 0;

    void Start(){
        SetupWallSelections();
    }

	private void Update()
	{
        if (dragging){
            HoverWall();
        }
	}

	private void SetupWallSelections(){
        wall_selections.Add(0, PlayerWallManager.WALL_ROTATION.HORIZONTAL);
        wall_selections.Add(1, PlayerWallManager.WALL_ROTATION.DIAGONAL_LEFT);
        wall_selections.Add(2, PlayerWallManager.WALL_ROTATION.VERTICAL);
        wall_selections.Add(3, PlayerWallManager.WALL_ROTATION.DIAGONAL_RIGHT);
    }

    // inherited
    public void OnDrag(PointerEventData eventData)
    {
        dragging = true;
        quaternion = GetQuaternion(GetAngle(transform.position, eventData.position));
        ShowOption();
    }

    // inherited
    public void OnEndDrag(PointerEventData eventData)
    {
        dragging = false;
        Destroy(gameObject.GetComponentInParent<PlayerWallManager>().wall_hover);
        ClearOptions();
        //BuildWall(GetQuaternion(GetAngle(transform.position, eventData.position)));
        BuildWall();
    }

    // inherited
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

    private void BuildWall(){
        gameObject.GetComponentInParent<PlayerWallManager>().PlaceNetworkedRealWall(wall_selections[quaternion]);
    }

    private void HoverWall(){
        if (wall_selections.ContainsKey(quaternion)){
            gameObject.GetComponentInParent<PlayerWallManager>().PlaceHoverWall(wall_selections[quaternion]);
        }
        else{
            Debug.Log("KEY DOESNT EXIST: "+quaternion);
        }
    }

    private void ShowOption(){
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