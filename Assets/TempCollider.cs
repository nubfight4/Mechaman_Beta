using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempCollider : MonoBehaviour {

	public LifeObject target;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter2D (){
		target.ReceiveDamage (400);
	}
}
