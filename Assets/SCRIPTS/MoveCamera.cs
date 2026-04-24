using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform cameraPostion;

    void Update()
    {
        transform.position = cameraPostion.position;
    }
}
