using System.Collections;
using UnityEngine;

public class PlayerMagnet : MonoBehaviour
{
    public float magnetRadius = 6f;
    public float pullSpeed = 10f;

    private bool magnetActive;

    public void ActivateMagnet(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(MagnetRoutine(duration));
    }

    private IEnumerator MagnetRoutine(float duration)
    {
        magnetActive = true;
        yield return new WaitForSeconds(duration);
        magnetActive = false;
    }

    private void FixedUpdate()
    {
        if (!magnetActive) return;

        Collider[] hits = Physics.OverlapSphere(transform.position, magnetRadius);

        foreach (Collider hit in hits)
        {
            float dist = Vector3.Distance(transform.position, hit.transform.position);
            if (dist < 1f)
            {
                Collectible c = hit.GetComponent<Collectible>();
                if (c != null) c.Collect();
            }
        }
    }
}
