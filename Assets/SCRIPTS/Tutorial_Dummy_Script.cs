using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_Dummy_Script : LifeObject {

	void Awake () {
		SetMaxHP (500);
		SetHP (GetMaxHP ());
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
