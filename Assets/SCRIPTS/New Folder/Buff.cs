using System.Collections;
using UnityEngine;

public abstract class Buff : MonoBehaviour
{
    public float duration = 5f;

    protected abstract void ApplyBuff(GameObject player);
    protected abstract void RemoveBuff(GameObject player);

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ApplyBuff(other.gameObject);
            StartCoroutine(BuffTimer(other.gameObject));
            gameObject.SetActive(false);
        }
    }

    private IEnumerator BuffTimer(GameObject player)
    {
        yield return new WaitForSeconds(duration);
        RemoveBuff(player);
    }
}
