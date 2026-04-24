using UnityEngine;

public class BuffIman : MonoBehaviour
{
    public float duration = 5f;

    [HideInInspector] public ObjectPooling2 poolManager;
    [HideInInspector] public Transform currentSpawn;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMagnet magnet = other.GetComponent<PlayerMagnet>();
            if (magnet != null)
            {
                magnet.ActivateMagnet(duration);
            }

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
