using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Brightness_Change : MonoBehaviour {

    public float Alpha = 0.5f;
        

	// Use this for initialization
	void Start () {
        Alpha = PlayerPrefs.GetFloat("BRIGHTNESS_Slider");
	}
	
	// Update is called once per frame
	void Update () {
        GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f, Alpha);
	}

    public void setAlpha (float value)
    {
        Alpha = value;
    }
}
