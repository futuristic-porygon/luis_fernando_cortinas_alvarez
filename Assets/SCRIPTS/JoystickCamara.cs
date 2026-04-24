using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickCamara : MonoBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler
{
    [Header("Referencias")]
    public PlayerCam camScript; 
    public RectTransform joystickStick;
    public RectTransform centerPoint;

    [Header("Configuración")]
    public float radialLimit = 100f;
    public float cameraSensitivity = 2f;

    private Vector2 axis;
    private bool onUse = false;

    private void Update()
    {
        if (onUse && camScript != null)
        {
            //para mandar input del joystick a la camara
            camScript.ReceiveTouchInput(axis.x * cameraSensitivity, axis.y * cameraSensitivity);
        }

        if (!onUse)
        {
            joystickStick.position = Vector3.Lerp(joystickStick.position, centerPoint.position, 10f * Time.deltaTime);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
       
        onUse = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Usamos ScreenPointToLocalPointInRectangle para que las coordenadas sean relativas al joystick, 
        // sin importar el tamaño de la pantalla o la resolución.
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(centerPoint, eventData.position, eventData.pressEventCamera, out localPoint);

        // Limitamos el movimiento del stick
        joystickStick.localPosition = Vector2.ClampMagnitude(localPoint, radialLimit);

        // Calculamos el eje entre -1 y 1
        axis = joystickStick.localPosition / radialLimit;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ResetJoystick();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        ResetJoystick();
    }

    private void ResetJoystick()
    {
        onUse = false;
        axis = Vector2.zero;

        // Le decimos a la cámara que ya no hay movimiento
        if (camScript != null)
        {
            camScript.ReceiveTouchInput(0, 0);
        }

        joystickStick.localPosition = Vector3.zero;
    }

}