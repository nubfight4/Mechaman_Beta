using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectOnInput : MonoBehaviour {

    public EventSystem eventSyst;
    public GameObject objSelect;
	Vector3 gamepadPos;

	public Button StartButton;
	public Button SettingsButton;
	public Button CreditsButton;
	public Button QuitButton;


    private bool buttonSelect;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		gamepadPos.x = Input.GetAxis ("Horizontal");
		gamepadPos.y = Input.GetAxis ("Vertical");

		if (gamepadPos.x < -0.05) 
		{
			SettingsButton.FindSelectableOnLeft();
			QuitButton.FindSelectableOnLeft();
		}

		if (gamepadPos.x > 0.05) 
		{
			StartButton.FindSelectableOnRight();
			CreditsButton.FindSelectableOnRight();
		}

		if (gamepadPos.y < -0.05) 
		{
			StartButton.FindSelectableOnDown();
			SettingsButton.FindSelectableOnDown();
		}

		if (gamepadPos.y > 0.05) 
		{
			CreditsButton.FindSelectableOnUp();
			QuitButton.FindSelectableOnUp();
		}

	}

//    private void OnDisable()
//    {
//        buttonSelect = false;
//    }
}
