using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;   
    public Vector3 offset;    
    public float smoothSpeed = 0.125f;

    private float fixedZ; 

    void Start()
    {
        
        fixedZ = transform.position.z;
    }

    void LateUpdate()
    {
   
        Vector3 desiredPosition = target.position + offset;

  
        desiredPosition.z = fixedZ;

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        transform.position = smoothedPosition;
    }
}
