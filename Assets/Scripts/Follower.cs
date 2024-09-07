using System.Collections;
using UnityEngine;
using TMPro;


public class Follower : MonoBehaviour
{   
    public Transform toFollow = null;   
    public float delay = 1f; 

    public TextMeshPro numberText;

    public string toFollowStr="";

    void onLoad(){
    }
    private void Start(){
        Transform childTransform = transform.Find("num");
        GameObject num = childTransform.gameObject;
        numberText = num.GetComponent<TextMeshPro>();
        numberText.text=this.name;  
        
    }

    private void Update()
    {
        if (toFollow != null)
        {
            StartCoroutine(Follow(toFollow.position, toFollow.rotation));
        }
    }
    
    private IEnumerator Follow(Vector3 targetPosition, Quaternion targetRotation)  // Corrected parameter types
    {
        yield return new WaitForSeconds(delay);
        transform.position = targetPosition;
        transform.rotation = targetRotation;
        yield break;
    }
    public void setNumber(string s){
        numberText.text=s;
    }

}
