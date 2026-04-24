using UnityEngine;

public class ButtonNavigation : MonoBehaviour
{
    public void ClickRestart()
    {
        if (PlayManager.Instance != null)
        {
            PlayManager.Instance.RestartGame();
        }
    }
}