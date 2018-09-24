using UnityEngine;
using UnityEngine.EventSystems;

public class FixedJoystick : Joystick
{
    Vector2 joystickPosition = Vector2.zero;
    float joystick_top;
    float joystick_bottom;
    private Camera cam = new Camera();

    void Start()
    {
        joystickPosition = RectTransformUtility.WorldToScreenPoint(cam, background.position);
        joystick_top = joystickPosition.y + handle.rect.height / 2;
        joystick_bottom = joystickPosition.y - handle.rect.height / 2;
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (joystickMode == JoystickMode.Horizontal)
        {
            if (eventData.position.y > joystick_bottom && eventData.position.y < joystick_top)
            {
                MoveHandle(eventData);
            }
        }
        else{
            MoveHandle(eventData);
        }
    }

    private void MoveHandle(PointerEventData eventData){
        Vector2 direction = eventData.position - joystickPosition;
        inputVector = (direction.magnitude > background.sizeDelta.x / 2f) ? direction.normalized : direction / (background.sizeDelta.x / 2f);
        ClampJoystick();
        handle.anchoredPosition = (inputVector * background.sizeDelta.x / 2f) * handleLimit;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        inputVector = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
    }
}