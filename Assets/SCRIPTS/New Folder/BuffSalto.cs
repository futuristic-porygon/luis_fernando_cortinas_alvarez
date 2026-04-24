using UnityEngine;

public class BuffSalto : MonoBehaviour
{
    public float jumpMultiplier = 1.5f;
    public float duration = 5f;

    
    [HideInInspector] public ObjectPooling2 poolManager;
    [HideInInspector] public Transform currentSpawn;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();

            if (player != null)
            {
                player.StartJumpBuff(jumpMultiplier, duration);
               
                if (poolManager != null && currentSpawn != null)
                {
                    poolManager.ReturnToList(currentSpawn, gameObject);
                }
                else
                {                    
                    gameObject.SetActive(false);
                }
            }
        }
    }
}