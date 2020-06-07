using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    [Header("Options")]
    [Range(0f, 2f)] public float handleLimit = 1f;

    protected float input = 0;
    public float Horizontal { get { return input; } }

    [Header("Components")]
    public RectTransform background;
    public RectTransform handle;
    

    private Vector2 joystickPosition = Vector2.zero;

    void Start()
    {
        joystickPosition = RectTransformUtility.WorldToScreenPoint(new Camera(), background.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        float dir = (eventData.position.x - joystickPosition.x) / (background.sizeDelta.x/2);
        //float dir = handle.localPosition.x / background.sizeDelta.x / 2;

        input = Mathf.Clamp(dir, -1,1);

        handle.anchoredPosition =new Vector2(input * background.sizeDelta.x / 2f , 0) * handleLimit;

        //Vector2 dir = eventData.position - joystickPosition;
        //inputVector = dir.normalized;
        //ClampJoystick();
        //handle.anchoredPosition = (inputVector * background.sizeDelta.x / 2f) * handleLimit;
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        input = 0;
    }
}