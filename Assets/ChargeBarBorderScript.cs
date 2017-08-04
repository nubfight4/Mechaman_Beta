using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChargeBarBorderScript : MonoBehaviour {

	public Mecha target;
	private Image image;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (target.currentCharge == 100) {
			image.GetComponent<Image> ().color = new Color (1.0f, 0f, 1.0f, 1.0f);
		} 
		else {
			image.GetComponent<Image> ().color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
		}
	}
}
