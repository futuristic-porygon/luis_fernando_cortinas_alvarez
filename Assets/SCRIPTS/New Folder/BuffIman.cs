using UnityEngine;

public class BuffIman : MonoBehaviour
{
    [Header("Settings")]
    public float duration = 5f;

    [Header("Audio")]
    public AudioClip pickupSFX; // Sonido al recoger

    [Header("VFX")]
    public GameObject pickupVFXPrefab; // Partículas al recoger

    [HideInInspector] public ObjectPooling2 poolManager;
    [HideInInspector] public Transform currentSpawn;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMagnet magnet = other.GetComponent<PlayerMagnet>();

            if (magnet != null)
            {
               
                if (pickupVFXPrefab != null)
                {
                    Instantiate(pickupVFXPrefab, transform.position, Quaternion.identity);                   
                }

                if (pickupSFX != null)
                {
                    AudioSource.PlayClipAtPoint(pickupSFX, transform.position);
                }

                magnet.ActivateMagnet(duration);

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