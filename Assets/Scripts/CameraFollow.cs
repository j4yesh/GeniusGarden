using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;   
    public Vector3 offset;    

    private float fixedZ; 

    void Start()
    {
        fixedZ = transform.position.z;
    }

    void LateUpdate()
    {
        Vector3 desiredPosition = target.position + offset;

        desiredPosition.z = fixedZ;

        transform.position = desiredPosition;
    }
}
