using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {

	public static CameraShake _instance;
	public float shakeDuration;
	public float shakeAmount;
	Vector3 _originalPos;
	float posX;
	float posY;

	void Awake()
	{
		_instance = this;
		_originalPos = this.GetComponent<Transform>().position;
	}
		
	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		if(shakeDuration > 0)
		{
			posX = Random.value * shakeAmount;
			posY = Random.value * shakeAmount;
			Vector3 newPos = new Vector3(posX,posY,-1.0f);
			this.GetComponent<Transform>().position = newPos;
			shakeDuration -= Time.deltaTime;
		}
		else
		{
			shakeDuration = 0.0f;
			this.GetComponent<Transform>().position = _originalPos;
		}
	}
}
