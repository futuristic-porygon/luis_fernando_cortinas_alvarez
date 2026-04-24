using UnityEngine;
using UnityEngine.EventSystems;
public class Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler
{
    [Header("Referencias")]
    public RectTransform joystickStick;
    public RectTransform centerPoint;

    [Header("Configuración")]
    public float radialLimit = 80f;
    public float resetSpeed = 15f;

    public Vector2 axis; // Lee el PlayerMovement

    private bool onUse = false;

    private void Update()
    {      
        if (!onUse)
        {
            if (joystickStick.localPosition.sqrMagnitude > 0.1f)
            {
                joystickStick.localPosition = Vector3.Lerp(joystickStick.localPosition, Vector3.zero, resetSpeed * Time.deltaTime);
            }
            else
            {
                joystickStick.localPosition = Vector3.zero;
            }

            //Se regresa
            axis = Vector2.zero;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        onUse = true;
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        
        RectTransformUtility.ScreenPointToLocalPointInRectangle(centerPoint, eventData.position, eventData.pressEventCamera, out pos);

        // Limite de joystick en UI
        joystickStick.localPosition = Vector2.ClampMagnitude(pos, radialLimit);

        axis = joystickStick.localPosition / radialLimit;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        onUse = false;
        axis = Vector2.zero;
    }

    //Un serguro por si sigue registrando movimiento
    public void OnPointerUp(PointerEventData eventData)
    {
        onUse = false;
        axis = Vector2.zero;
    }
}