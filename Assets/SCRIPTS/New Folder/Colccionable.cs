using UnityEngine;

public class Collectible : MonoBehaviour
{
    [HideInInspector] public ObjectPooling2 poolManager;
    [HideInInspector] public Transform currentSpawn;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Collect();
        }
    }

    public void Collect()
    {
        if (currentSpawn == null)
        {
            Debug.LogError("¡ERROR! La Kaoruko " + gameObject.name + " no tiene asignado un spawn.");
            return;
        }

        Debug.Log("Recolectada. Devolviendo spawn: " + currentSpawn.name);

        if (GameManager.instance != null)
            GameManager.instance.AddKaoruko();

        if (poolManager != null)
            poolManager.ReturnToList(currentSpawn, gameObject);
    }
}