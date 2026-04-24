using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    [Header("Configuración de Rotación")]
    public float sensX = 15f;
    public float sensY = 15f;

    [Header("Referencias")]
    public Transform orientation;

    private float xRotation;
    private float yRotation;

    // Valores que recibimos del Joystick
    private float inputX;
    private float inputY;

    private void Start()
    {
        // Forzamos que empiecen en cero para evitar el giro loco al iniciar
        inputX = 0;
        inputY = 0;

        
    }
    private void Update()
    {
        // Solo aplicamos rotación si el input es lo suficientemente grande
        // Esto evita que la cámara se mueva sola por errores de precisión
        if (Mathf.Abs(inputX) > 0.01f || Mathf.Abs(inputY) > 0.01f)
        {
            float rotateX = inputX * sensX * Time.deltaTime * 50f;
            float rotateY = inputY * sensY * Time.deltaTime * 50f;

            yRotation += rotateX;
            xRotation -= rotateY;

            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
            orientation.rotation = Quaternion.Euler(0, yRotation, 0);
        }
    }

    // El Joystick llamará a esta función
    public void ReceiveTouchInput(float x, float y)
    {
        inputX = x;
        inputY = y;
    }
}