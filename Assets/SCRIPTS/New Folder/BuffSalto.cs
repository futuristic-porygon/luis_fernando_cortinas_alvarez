using UnityEngine;

public class BuffSalto : MonoBehaviour
{
    [Header("Settings")]
    public float jumpMultiplier = 1.5f;
    public float duration = 5f;

    [Header("Audio")]
    public AudioClip pickupSFX;

    [Header("VFX")]
    public GameObject pickupVFXPrefab;

    [HideInInspector] public ObjectPooling2 poolManager;
    [HideInInspector] public Transform currentSpawn;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();

            if (player != null)
            {
                if (pickupVFXPrefab != null)
                {
                    Instantiate(pickupVFXPrefab, transform.position, Quaternion.identity);
                }

                if (pickupSFX != null)
                {
                    AudioSource.PlayClipAtPoint(pickupSFX, transform.position);
                }

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