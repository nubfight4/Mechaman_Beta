using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ButtonPress
{
	UPP1 = 0,
	DOWNP1,
	LEFTP1,
	RIGHTP1,
	FIRE01P2,
	FIRE02P2,
	FIRE03P2,
	LEFTBUMPP1,
	RIGHTBUMPP1,
	LEFTBUMPP2,
	RIGHTBUMPP2,
}

public class TheBarbaricInput : MonoBehaviour
{

	public List<ButtonPress> ComboStore;

	public int tapUpP1;
	public int tapDownP1;
	public int tapLeftP1;
	public int tapRightP1;
	public int tapFire1P2;
	public int tapFire2P2;
	public int tapFire3P2;
	public int tapLeftBumperP1;
	public int tapRightBumperP1;
	public int tapLeftBumperP2;
	public int tapRightBumperP2;

	public float holdUpP1Duration;
	public float holdDownP1Duration;
	public float holdLeftP1Duration;
	public float holdRightP1Duration;
	public float holdFire1P2Duration;
	public float holdFire2P2Duration;
	public float holdFire3P2Duration;
	public float holdLeftBumperP1Duration;
	public float holdRightBumperP1Duration;
	public float holdLeftBumperP2Duration;
	public float holdRightBumperP2Duration;

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
		if (Input.GetButtonDown ("VerticalP1")) {
			keyPressDurationTimer = 0f;
			isKeyPress = true;
			if (Input.GetAxis ("VerticalP1") > 0) {
				ComboStore.Add (ButtonPress.UPP1);
				tapUpP1++;
			} else if (Input.GetAxis ("VerticalP1") < 0) {
				ComboStore.Add (ButtonPress.DOWNP1);
				tapDownP1++;
			}
		}
		if (Input.GetButtonDown	("HorizontalP1")) {
			keyPressDurationTimer = 0f;
			isKeyPress = true;
			if (Input.GetAxis ("HorizontalP1") < 0) {
				ComboStore.Add (ButtonPress.LEFTP1);
				tapLeftP1++;
			} else if (Input.GetAxis ("HorizontalP1") > 0) {
				ComboStore.Add (ButtonPress.RIGHTP1);
				tapRightP1++;
			}
		}

		if (Input.GetButtonDown ("Fire01P2")) {
			keyPressDurationTimer = 0f;
			isKeyPress = true;
			ComboStore.Add (ButtonPress.FIRE01P2);
			tapFire1P2++;
		}
		if (Input.GetButtonDown ("Fire02P2")) {
			keyPressDurationTimer = 0f;
			isKeyPress = true;
			ComboStore.Add (ButtonPress.FIRE02P2);
			tapFire2P2++;
		}
		if (Input.GetButtonDown ("Fire03P2")) {
			keyPressDurationTimer = 0f;
			isKeyPress = true;
			ComboStore.Add (ButtonPress.FIRE03P2);
			tapFire3P2++;
		}

		if (Input.GetButtonDown ("Bumper_Right_P1")) {
			keyPressDurationTimer = 0f;
			isKeyPress = true;
			ComboStore.Add (ButtonPress.RIGHTBUMPP1);
			tapRightBumperP1++;
		}
		if (Input.GetButtonDown ("Bumper_Left_P1")) {
			keyPressDurationTimer = 0f;
			isKeyPress = true;
			ComboStore.Add (ButtonPress.LEFTBUMPP1);
			tapLeftBumperP1++;
		}
		if (Input.GetButtonDown ("Bumper_Right_P2")) {
			keyPressDurationTimer = 0f;
			isKeyPress = true;
			ComboStore.Add (ButtonPress.RIGHTBUMPP2);
			tapRightBumperP2++;
		}
		if (Input.GetButtonDown ("Bumper_Left_P2")) {
			keyPressDurationTimer = 0f;
			isKeyPress = true;
			ComboStore.Add (ButtonPress.LEFTBUMPP2);
			tapLeftBumperP2++;
		}

		//HOLD
		if (Input.GetButton ("VerticalP1")) {
			if (Input.GetAxis ("VerticalP1") > 0) {
				holdUpP1Duration += Time.deltaTime * 1000f;
			} else {
				holdUpP1Duration = 0f;
			}
		} else {
			holdUpP1Duration = 0f;
		}

		if (Input.GetButton ("VerticalP1")) {
			if (Input.GetAxis ("VerticalP1") < 0) {
				holdDownP1Duration += Time.deltaTime * 1000f;
			} else {
				holdDownP1Duration = 0f;
			}
		} else {
			holdDownP1Duration = 0f;
		}

		if (Input.GetButton	("HorizontalP1")) {
			if (Input.GetAxis ("HorizontalP1") < 0) {
				holdLeftP1Duration += Time.deltaTime * 1000f;
			} else {
				holdLeftP1Duration = 0f;
			}
		} else {
			holdLeftP1Duration = 0f;
		}

		if (Input.GetButton	("HorizontalP1")) {
			if (Input.GetAxis ("HorizontalP1") > 0) {
				holdRightP1Duration += Time.deltaTime * 1000f;
			} else {
				holdRightP1Duration = 0f;
			}
		} else {
			holdRightP1Duration = 0f;
		}

		if (Input.GetButton ("Fire01P2")) {
			holdFire1P2Duration += Time.deltaTime * 1000f;
		} else {
			holdFire1P2Duration = 0f;
		}
		if (Input.GetButton ("Fire02P2")) {
			holdFire2P2Duration += Time.deltaTime * 1000f;
		} else {
			holdFire2P2Duration = 0f;
		}
		if (Input.GetButton ("Fire03P2")) {
			holdFire3P2Duration += Time.deltaTime * 1000f;
		} else {
			holdFire3P2Duration = 0f;
		}

		if (Input.GetButton ("Bumper_Right_P1")) {
			holdRightBumperP1Duration += Time.deltaTime * 1000f;
		} else {
			holdRightBumperP1Duration = 0f;
		}
		if (Input.GetButton ("Bumper_Left_P1")) {
			holdLeftBumperP1Duration += Time.deltaTime * 1000f;
		} else {
			holdLeftBumperP1Duration = 0f;
		}
		if (Input.GetButton ("Bumper_Right_P2")) {
			holdRightBumperP2Duration += Time.deltaTime * 1000f;
		} else {
			holdRightBumperP2Duration = 0f;
		}
		if (Input.GetButton ("Bumper_Left_P2")) {
			holdLeftBumperP2Duration += Time.deltaTime * 1000f;
		} else {
			holdLeftBumperP2Duration = 0f;
		}

		if (ComboStore.Count > maxButton) {
			//ComboStore.RemoveAt (0);
			ComboStore.Clear ();
		}

		if (isKeyPress) {
			if (keyPressDurationTimer <= keyPressDuration) {
				keyPressDurationTimer += Time.deltaTime * 1000f;
			} else {
				keyPressDurationTimer = 0f;
				isKeyPress = false;
				ComboStore.Clear ();
				KeyTapReset ();
			}
		} 
	}

	public void KeyTapReset ()
	{
		tapUpP1 = 0;
		tapDownP1 = 0;
		tapLeftP1 = 0;
		tapRightP1 = 0;
		tapFire1P2 = 0;
		tapFire2P2 = 0;
		tapFire3P2 = 0;
		tapLeftBumperP1 = 0;
		tapRightBumperP1 = 0;
		tapLeftBumperP2 = 0;
		tapRightBumperP2 = 0;

		holdUpP1Duration = 0;
		holdDownP1Duration = 0;
		holdLeftP1Duration = 0;
		holdRightP1Duration = 0;
		holdFire1P2Duration = 0;
		holdFire2P2Duration = 0;
		holdFire3P2Duration = 0;
		holdLeftBumperP1Duration = 0;
		holdRightBumperP1Duration = 0;
		holdLeftBumperP2Duration = 0;
		holdRightBumperP2Duration = 0;
	}
}
