using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealtDot : MonoBehaviour {

	public LifeObject target;
	private Image dot;

	// Use this for initialization
	void Start () {
		dot = GetComponent<Image> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (target.lives < 3) 
		{
			dot.enabled = false;
		}
	}
}
