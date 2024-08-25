using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class Follower : MonoBehaviour
{   
    public Transform toFollow = null;   
    public float delay = 1f;  // Delay used in the coroutine

    public TextMeshPro numberText;

    private void Start(){
        Transform childTransform = transform.Find("num");
        GameObject num = childTransform.gameObject;
        numberText = num.GetComponent<TextMeshPro>();
        numberText.text=this.name;
        // Invoke("destroyTime",4f);
        // transform.DOScale(2, 3);
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

    void destroyTime(){
        // Destroy(this);
    }
}
