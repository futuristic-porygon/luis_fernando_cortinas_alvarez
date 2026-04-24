using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
   
    [Header("Movement")]
    public float moveSpeed = 9f; 
    public float groundDrag = 5f;
    public float airMultiplier = 0.5f;
   
    [Header("Jumping")]
    public float jumpForce = 7f; 
    public float jumpCoolDown = 0.25f;
    private bool readyToJump = true;

   
    [Header("Ground Check")]
    public float playerHeight = 2f;
    public LayerMask whatIsGround;
    private bool grounded;   

    [Header("References")]
    public Transform orientation;       
    public Joystick mobileJoystick;    

    private PlayerControls controls;   
    private Vector2 moveInput;          
    private Vector3 moveDirection;     
    private Rigidbody rb;
    private float baseJumpForce;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        controls = new PlayerControls();
       
        controls.Player.Jump.performed += ctx => AttemptJump();
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    void Start()
    {
        rb.freezeRotation = true;
        baseJumpForce = jumpForce;
    }

    void Update()
    {
       
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

       //Porque tenia problemas con el salto queria ver visualmente el rango de deteccion del suelo
        Debug.DrawRay(transform.position, Vector3.down * (playerHeight * 0.5f + 0.3f), grounded ? Color.green : Color.red);

        GetInput();     
        SpeedControl();

        
        if (grounded) rb.linearDamping = groundDrag;
        else rb.linearDamping = 0;
    }

    void FixedUpdate()
    {
        
        MovePlayer();
    }

    private void GetInput()
    {
        
        Vector2 keyboardInput = controls.Player.Move.ReadValue<Vector2>();
        Vector2 joystickInput = (mobileJoystick != null) ? mobileJoystick.axis : Vector2.zero;

        moveInput = keyboardInput + joystickInput;

        //Para que no tiemble el jugador
        if (moveInput.magnitude < 0.05f) moveInput = Vector2.zero;
    }

    private void MovePlayer()
    {
        if (moveInput == Vector2.zero) return;
       
        moveDirection = orientation.forward * moveInput.y + orientation.right * moveInput.x;

        // Aceleracion movimeinto
        if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Acceleration);
        else
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Acceleration);
    }

    private void SpeedControl()
    {
        
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        // para que sea gradual el parar 
        if (moveInput == Vector2.zero && grounded)
        {
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, new Vector3(0, rb.linearVelocity.y, 0), Time.deltaTime * 20f);

            if (flatVel.magnitude < 0.1f)
                rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
        //Para limitar velocidad
        else if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    public void AttemptJump()
    {        
        if (readyToJump && grounded)
        {
            readyToJump = false;
            Jump();           
            Invoke(nameof(ResetJump), jumpCoolDown); //para el cooldown
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
       
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump() => readyToJump = true;

    //Para iniciar el buff
    public void StartJumpBuff(float multiplier, float duration)
    {
        StartCoroutine(JumpBuffCoroutine(multiplier, duration));
    }

    private IEnumerator JumpBuffCoroutine(float multiplier, float duration)
    {
        jumpForce *= multiplier;
        yield return new WaitForSeconds(duration);
        jumpForce /= multiplier;
    }
}