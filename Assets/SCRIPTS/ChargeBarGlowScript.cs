using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeBarGlowScript : MonoBehaviour {

	public Mecha mecha;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (mecha.currentCharge == mecha.maxCharge) {
			GetComponent<SpriteRenderer> ().enabled = true;
		} 
		else {
			GetComponent<SpriteRenderer> ().enabled = false;
		}
	}
}
