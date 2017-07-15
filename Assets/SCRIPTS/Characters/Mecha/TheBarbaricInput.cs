using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ButtonPress
{
	UP = 0,
	DOWN,
	LEFT,
	RIGHT,
	SHOOT,
	BLOCK,
}

public class TheBarbaricInput : MonoBehaviour
{

	public List<ButtonPress> ComboStore;

	public float holdUpDuration;
	public float holdDownDuration;
	public float holdLeftDuration;
	public float holdRightDuration;
	public float holdShootDuration;
	public float holdBlockDuration;

	public int maxButton = 8;

	public bool isKeyPress = false;
	public float keyPressDuration = 500f;
	public float keyPressDurationTimer = 0f;

	void Start ()
	{
		
	}

	void Update ()
	{
		//TAP
		if (Input.GetButtonDown ("Vertical")) {
			keyPressDurationTimer = 0f;
			isKeyPress = true;
			if (Input.GetAxis ("Vertical") > 0) {
				ComboStore.Add (ButtonPress.UP);
			} else if (Input.GetAxis ("Vertical") < 0) {
				ComboStore.Add (ButtonPress.DOWN);
			}
		}
		if (Input.GetButtonDown ("Vertical")) {

		}
		if (Input.GetButtonDown	("Horizontal")) {
			keyPressDurationTimer = 0f;
			isKeyPress = true;
			if (Input.GetAxis ("Horizontal") < 0) {
				ComboStore.Add (ButtonPress.LEFT);
			} else if (Input.GetAxis ("Horizontal") > 0) {
				ComboStore.Add (ButtonPress.RIGHT);
			}
		}
		if (Input.GetMouseButtonDown (0)) {
			keyPressDurationTimer = 0f;
			isKeyPress = true;
			ComboStore.Add (ButtonPress.SHOOT);
		}
		if (Input.GetMouseButtonDown (1)) {
			keyPressDurationTimer = 0f;
			isKeyPress = true;
			ComboStore.Add (ButtonPress.BLOCK);
		}

		//HOLD
		if (Input.GetButton ("Vertical")) {
			if (Input.GetAxis ("Vertical") > 0) {
				holdUpDuration += Time.deltaTime * 1000f;
			} else {
				holdUpDuration = 0f;
			}
		} else {
			holdUpDuration = 0f;
		}

		if (Input.GetButton ("Vertical")) {
			if (Input.GetAxis ("Vertical") < 0) {
				holdDownDuration += Time.deltaTime * 1000f;
			} else {
				holdDownDuration = 0f;
			}
		} else {
			holdDownDuration = 0f;
		}

		if (Input.GetButton	("Horizontal")) {
			if (Input.GetAxis ("Horizontal") < 0) {
				holdLeftDuration += Time.deltaTime * 1000f;
			} else {
				holdLeftDuration = 0f;
			}
		} else {
			holdLeftDuration = 0f;
		}

		if (Input.GetButton	("Horizontal")) {
			if (Input.GetAxis ("Horizontal") > 0) {
				holdRightDuration += Time.deltaTime * 1000f;
			} else {
				holdRightDuration = 0f;
			}
		} else {
			holdRightDuration = 0f;
		}

		if (Input.GetMouseButton (0)) {
			holdShootDuration += Time.deltaTime * 1000f;
		} else {
			holdShootDuration = 0f;
		}
		if (Input.GetMouseButton (1)) {
			holdBlockDuration += Time.deltaTime * 1000f;
		} else {
			holdBlockDuration = 0f;
		}

		if (ComboStore.Count > maxButton) {
			ComboStore.RemoveAt (0);
		}

		if (isKeyPress) {
			if (keyPressDurationTimer <= keyPressDuration) {
				keyPressDurationTimer += Time.deltaTime * 1000f;
			} else {
				keyPressDurationTimer = 0f;
				isKeyPress = false;
				ComboStore.Clear ();
			}
		} 


	}
}
