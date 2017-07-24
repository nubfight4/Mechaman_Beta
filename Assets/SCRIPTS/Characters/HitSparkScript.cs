using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitSparkScript : MonoBehaviour {

	public float lifetime = 0.5f;
	public float timer = 0.0f;
	bool startCountDown = false;
	// Use this for initialization

	// Update is called once per frame
	void Update () {
		if(this.isActiveAndEnabled)
		{
			startCountDown = true;
		}
		if(startCountDown)
		{
			timer += Time.deltaTime;
		}
		if(timer >= lifetime)
		{
			timer = 0.0f;
			this.gameObject.SetActive(false);
		}
	}
}
