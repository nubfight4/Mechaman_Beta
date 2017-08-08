using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SliderControllerScript : MonoBehaviour, IUpdateSelectedHandler
{
    public Slider slider;

    public void OnUpdateSelected(BaseEventData data)
    {
        if (Input.GetAxis("Horizontal") > 0 || Input.GetKeyDown(KeyCode.RightArrow))
        {
            slider.value += 0.01f;
            //Debug.Log("RIGHT");
        }
        else if (Input.GetAxis("Horizontal") < 0 || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            slider.value -= 0.01f;
            //Debug.Log("LEFT");
        }  
    }
}



