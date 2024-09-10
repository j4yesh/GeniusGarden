using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FloatingJoystick : Joystick
{       
    private bool canTouch = false;

    protected override void Start()
    {
        base.Start();
        background.gameObject.SetActive(false);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {   
        if(canTouch){
            background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
            background.gameObject.SetActive(true);
            base.OnPointerDown(eventData);
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {   
        if(canTouch){
            background.gameObject.SetActive(false);
            base.OnPointerUp(eventData);
        }
    }

    public void SetActive(bool change){
        this.canTouch = change;
    }
}