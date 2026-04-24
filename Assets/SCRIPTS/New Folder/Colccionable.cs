using UnityEngine;

public class Collectible : MonoBehaviour
{
    [HideInInspector] public ObjectPooling2 poolManager;
    [HideInInspector] public Transform currentSpawn;

    [Header("Feedback")]
    public GameObject collectVFX; // Prefab de partículas
    public AudioClip collectSFX;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Collect();
        }
    }

    public void Collect()
    {
        if (currentSpawn == null) return;

        if (collectSFX) AudioSource.PlayClipAtPoint(collectSFX, transform.position);
        if (collectVFX) Instantiate(collectVFX, transform.position, Quaternion.identity);

        if (GameManager.instance != null)
            GameManager.instance.AddKaoruko();

        if (poolManager != null)
            poolManager.ReturnToList(currentSpawn, gameObject);
    }
}