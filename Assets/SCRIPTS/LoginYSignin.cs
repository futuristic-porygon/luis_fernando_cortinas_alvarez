using UnityEngine;
using UnityEngine.InputSystem;

public class LoginYSignin : MonoBehaviour
{
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject signinPanel;

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Login();
        }
    }

    public void Login()
    {
        loginPanel.SetActive(true);
        signinPanel.SetActive(false);
    }

    public void Signin()
    {
        loginPanel.SetActive(false);
        signinPanel.SetActive(true);
    }
}
