using System.Collections;
using UnityEngine;
using TMPro;

public class Follower : MonoBehaviour
{
    public Transform toFollow = null;
    public float delay = 1f;
    public float followSpeed = 5f;  // Speed for following

    public TextMeshPro numberText;
    public string toFollowStr = "";

    private void Start()
    {
        // Find the child object "num" and get its TextMeshPro component
        Transform childTransform = transform.Find("num");
        GameObject num = childTransform.gameObject;
        numberText = num.GetComponent<TextMeshPro>();
        numberText.text = this.name;  
    }

    private void Update()
    {
        if (toFollow != null)
        {
            // Follow the target smoothly
            StartCoroutine(Follow(toFollow.position, toFollow.rotation));
        }
    }

    private IEnumerator Follow(Vector3 targetPosition, Quaternion targetRotation)
    {
        // Wait for the delay
        yield return new WaitForSeconds(delay);

        // Smoothly move towards the target position using Lerp
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        // Smoothly rotate towards the target rotation using Slerp
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, followSpeed * Time.deltaTime);

        yield break;
    }

    public void setNumber(string s)
    {
        numberText.text = s;
    }
}


// using System.Collections;
// using UnityEngine;
// using TMPro;


// public class Follower : MonoBehaviour
// {   
//     public Transform toFollow = null;   
//     public float delay = 1f; 

//     public TextMeshPro numberText;

//     public string toFollowStr="";

//     void onLoad(){
//     }
//     private void Start(){
//         Transform childTransform = transform.Find("num");
//         GameObject num = childTransform.gameObject;
//         numberText = num.GetComponent<TextMeshPro>();
//         numberText.text=this.name;  
        
//     }

//     private void Update()
//     {
//         if (toFollow != null)
//         {
//             StartCoroutine(Follow(toFollow.position, toFollow.rotation));
//         }
//     }
    
//     private IEnumerator Follow(Vector3 targetPosition, Quaternion targetRotation)  // Corrected parameter types
//     {
//         yield return new WaitForSeconds(delay);
//         transform.position = targetPosition;
//         transform.rotation = targetRotation;
//         yield break;
//     }
//     public void setNumber(string s){
//         numberText.text=s;
//     }

// }
