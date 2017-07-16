using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockIndicatorScript : MonoBehaviour {

	private Animator anim;
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
	}

	void OnEnable()
	{
		anim.SetTrigger("Indicate");
	}

}
