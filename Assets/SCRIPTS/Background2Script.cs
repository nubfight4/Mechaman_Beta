using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background2Script : MonoBehaviour {

	public Goatzilla goatzilla;
	SpriteRenderer spr;

	// Use this for initialization
	void Start () {
		goatzilla = GameObject.FindGameObjectWithTag ("Enemy").GetComponent<Goatzilla> ();
		spr = GetComponent<SpriteRenderer> ();
	}

	// Update is called once per frame
	void Update () {
		if (goatzilla.isEnraged) {
			spr.enabled = true;
		}
	}
}
