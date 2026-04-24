using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public static InputHandler Instance;

    private PlayerControls inputs;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        inputs = new PlayerControls();
    }

    private void OnEnable()
    {
        inputs.Enable();
    }

    private void OnDisable()
    {
        inputs.Disable();
    }

    public Vector2 Move()
    {
        return inputs.Player.Move.ReadValue<Vector2>();
    }
}
