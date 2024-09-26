using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class remoteGameplay : MonoBehaviour
{
    public BoxCollider2D boundingBoxCollider; 

    private Vector2 startPos;
    private Vector2 movement = new Vector2(0f, 0f); 

    [SerializeField]
    private bool isTouching;

    public Vector2 bottomLeft;
    public Vector2 topRight;

    public float speed = 2f;

    public Transform rear;

    public string selfId;

    private List<GameObject>followers ;

    public GameObject playerLabel;

    public GameObject playerLabelTemp;
    public Utility util = null;


    void onLoad(){
    }
     void Start()
    {
        boundingBoxCollider = this.GetComponent<BoxCollider2D>();

        // playerLabel = Instantiate(playerLabelTemp,
        //           new Vector3(0,0,0), Quaternion.identity);
        // playerLabel.GetComponent<playerLabel>().attach(this.gameObject);
        // playerLabel.GetComponent<playerLabel>().setName("...");

        followers = new List<GameObject>();
        rear = transform;
    }

    public void setLabel(string str){
        playerLabel = Instantiate(playerLabelTemp,
                  new Vector3(0,0,0),
                  Quaternion.identity);
        playerLabel.GetComponent<playerLabel>().attach(this.gameObject);
        playerLabel.GetComponent<playerLabel>().setName(str);
    }

    public void setName(string str){
        playerLabel.GetComponent<playerLabel>().setName(str);
    }

    void Update()
    {
        // Vector2 newPosition = new Vector2(transform.position.x + movement.x * speed * Time.deltaTime,
        //                                   transform.position.y + movement.y * speed * Time.deltaTime);
        // if (newPosition.x > bottomLeft.x && newPosition.x < topRight.x &&
        // newPosition.y > bottomLeft.y && newPosition.y < topRight.y)
        // {
        //     SetPosition(newPosition);

        // }
        
        // foreach (GameObject obj in  GameObject.FindGameObjectsWithTag("Rat"))
        // {
        //     if (boundingBoxCollider.bounds.Contains(obj.transform.position))
        //     {   
        //         if(util.tryToAnswer(obj.name)){
        //             Debug.Log("Object within bounding box: " + obj.name);
        //             obj.GetComponent<Follower>().toFollow = rear;
        //             rear = obj.transform;
        //             obj.tag = "Snake";
        //             obj.GetComponent<Follower>().toFollowStr=selfId;
        //             this.followers.Add(obj);
        //         }else{
        //             Debug.Log("Wrong ans check once ");
        //             Destroy(obj);
        //         }
        //     }
        // }
    }

     public void addRat(string str)
    {
        GameObject obj = this.util.getNewRat(str);
        obj.transform.localScale = Vector3.zero;
        obj.transform.DOScale(10, 0.5f);
        obj.GetComponent<Follower>().toFollow = rear;
        rear = obj.transform;
        obj.tag = "Snake";
    }

    public void SetPosition(Vector2 newPosition)
    {
        transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
    }

    public void destoyFollowers(){
        foreach(GameObject it in this.followers){
            Destroy(it);
        }
        Destroy(playerLabel);
    }

    public void removeRat()
    {   
        Transform rat = rear;
        
        if (rat.GetComponent<Follower>())
        {
            rear = rat.GetComponent<Follower>().toFollow;
            rat.DOScale(Vector3.zero, 0.5f) 
                .OnComplete(() => 
                {
                    Destroy(rat.gameObject);
                });
            rat.GetComponent<Follower>().toFollow = null;
        }
    }

    
    
    
    
}
