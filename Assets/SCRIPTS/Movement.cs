using UnityEngine;


/// <summary>
/// Este movimiento va a requerir de un Rigidbody para funcionar
/// </summary>
[RequireComponent (typeof(Rigidbody),typeof(Gravity))]
public class Movement : MonoBehaviour
{
    [SerializeField] private float walkSpeed =5;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

    }

    private void Update()
    {
        Move();
    }

    public void Move()
    {
        rb.linearVelocity = new Vector3(InputHandler.Instance.Move().x,
            0,
            InputHandler.Instance.Move().y * Time.deltaTime * (walkSpeed *100));
    }

   
}
