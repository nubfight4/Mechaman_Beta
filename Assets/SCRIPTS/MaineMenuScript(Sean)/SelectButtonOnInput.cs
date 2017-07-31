﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class SelectButtonOnInput : MonoBehaviour
{

	public EventSystem eventSystem;
	public GameObject selectedObject;

	private bool buttonSelected = false;

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetAxisRaw ("VerticalP1") != 0 || Input.GetAxisRaw ("VerticalP2") != 0) {
			if (!buttonSelected) {
				eventSystem.SetSelectedGameObject (selectedObject);
				buttonSelected = true;
			}
		}
	}

	private void OnDisable ()
	{
		buttonSelected = false;
	}
}