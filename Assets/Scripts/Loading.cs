using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
public class Loading : MonoBehaviour
{  
    private Animation anim;
    private Image image;


    void Start()
    {
         anim = GetComponent<Animation>();
         image = GetComponent<Image>();
         image.enabled=false;
    }

    public void show(){
        anim.enabled=true;
        image.enabled=true;
        anim.Play("circle_half_rotating_fading_6_1");
    }

    public void hide(){
        Debug.Log("loading is hided!");
        anim.Stop("circle_half_rotating_fading_6_1");
        image.enabled=false;
        anim.enabled=false;
    }
}
