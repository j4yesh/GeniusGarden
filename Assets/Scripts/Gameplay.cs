using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class Gameplay : MonoBehaviour
{   
    public BoxCollider2D boundingBoxCollider; 
    public GameObject Head;
    private Vector2 startPos;
    private Vector2 movement = new Vector2(0f, 0f);  // Movement vector

    [SerializeField]
    public FloatingJoystick __joystick;
    private bool isTouching;

    public Vector2 bottomLeft;
    public Vector2 topRight;

    public float speed = 2f;

    public Transform rear;

    public Utility until = null;

    void Start()
    {
        boundingBoxCollider = Head.GetComponent<BoxCollider2D>();
        rear = Head.transform;
    }

    void Update()
    {
        // Handle Touch Input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 centeredPosition = GetCenteredPosition(touch.position);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    OnTouch(centeredPosition);
                    break;

                case TouchPhase.Moved:
                    OnTouchMove(centeredPosition);
                    break;

                case TouchPhase.Ended:
                    OnTouchEnd(centeredPosition);
                    break;
            }
        }

        // Handle Mouse Input
        else if (Input.GetMouseButtonDown(0))
        {
            OnTouch(GetCenteredPosition(Input.mousePosition));
        }
        else if (Input.GetMouseButton(0))
        {
            OnTouchMove(GetCenteredPosition(Input.mousePosition));
        }
        else if (Input.GetMouseButtonUp(0))
        {
            OnTouchEnd(GetCenteredPosition(Input.mousePosition));
        }

        Vector2 newPosition = new Vector2(Head.transform.position.x + movement.x * speed * Time.deltaTime,
                                          Head.transform.position.y + movement.y * speed * Time.deltaTime);
        if (newPosition.x > bottomLeft.x && newPosition.x < topRight.x &&
        newPosition.y > bottomLeft.y && newPosition.y < topRight.y)
        {
            SetPosition(newPosition);

        }
        
        foreach (GameObject obj in  GameObject.FindGameObjectsWithTag("Rat"))
        {
            if (boundingBoxCollider.bounds.Contains(obj.transform.position))
            {   
                if(until.tryToAnswer(obj.name)){
                    Debug.Log("Object within bounding box: " + obj.name);
                    obj.GetComponent<Follower>().toFollow = rear;
                    rear = obj.transform;
                    obj.tag = "Snake";
                    until.generateQuestion();
                }
            }
        }
    }

    // Convert screen position to a centered position where the screen center is (0, 0)
    Vector2 GetCenteredPosition(Vector2 position)
    {
        float x = position.x - (Screen.width / 2f);
        float y = position.y - (Screen.height / 2f);
        return new Vector2(x, y);
    }

    void OnTouch(Vector2 position)
    {
        startPos = position;
        isTouching = true;
    }

    void OnTouchMove(Vector2 position)
    {
        if (isTouching)
        {
            // Calculate the length (magnitude) of the vector
            float length = Mathf.Sqrt(__joystick.Direction.x * __joystick.Direction.x + 
            __joystick.Direction.y * __joystick.Direction.y);

            if (length > 0) // Prevent division by zero
            {
                // Get the direction from the joystick
                movement.x = __joystick.Direction.x;
                movement.y = __joystick.Direction.y;

                movement.x = movement.x / length;
                movement.y = movement.y / length;
                // Calculate the angle in radians
                float angle = Mathf.Atan2(movement.y, movement.x);

                // Convert radians to degrees
                float angleInDegrees = angle * Mathf.Rad2Deg;

                // Apply the rotation to the Head's Transform (assuming rotation around the Z-axis)
                Head.transform.rotation = Quaternion.Euler(0, 0, angleInDegrees);
            }
            
        }
    }

    void OnTouchEnd(Vector2 position)
    {
        isTouching = false;
        // Add additional logic for what should happen when touch/click ends
    }

    // Function to set the position of the Head GameObject
    public void SetPosition(Vector2 newPosition)
    {
        Head.transform.position = new Vector3(newPosition.x, newPosition.y, Head.transform.position.z);
    }

    
    

}
