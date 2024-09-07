using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class playerLabel : MonoBehaviour
{
    public GameObject toFollow;
    public Vector2 offset;
    public TextMeshPro playerName;

    void Start()
    {
        // Initialize playerName once in Start
        playerName = gameObject.GetComponent<TextMeshPro>();
        if (playerName == null)
        {
            Debug.LogError("TextMeshPro component not found on the GameObject.");
        }
    }

    void Update()
    {
        if (toFollow != null)
        {
            // Adjust the label's position based on the object it's following
            transform.position = new Vector3(
                toFollow.transform.position.x + offset.x,
                toFollow.transform.position.y + offset.y,
                0
            );
        }
    }

    public void attach(GameObject obj)
    {
        this.toFollow = obj;
    }

    public void setName(string itext)
    {
        // Only attempt to set the text if playerName is not null
        if (playerName != null)
        {
            playerName.text = itext;
            Debug.Log("Changing name to: " + itext);
        }
        else
        {
            Debug.LogError("TextMeshPro component not assigned.");
        }
    }
}
