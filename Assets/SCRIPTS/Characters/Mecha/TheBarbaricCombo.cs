using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheBarbaricCombo : MonoBehaviour
{

	TheBarbaricInput bbInput;

	//if input is done within a time frame return true
	//return false once combo has excuted

	// Shoot (P2) + Up (P1)
	public bool canJumpPunch = false;

	// Block (P2) + Down (P1) + Left or Right (P1)
	public bool canDoubleKick = false;

	// Shoot (P1) -> Shoot (P2) + Down (P1)
	public bool canMeSmash = false;

	// Block (P2) + Block (P1)
	public bool canFistRocket = false;

	// Shoot (P2) + Shoot (P1)
	public bool canLetsDance = false;

	void Start ()
	{
		bbInput = gameObject.GetComponent<TheBarbaricInput> ();
	}

	void Update ()
	{
		if (bbInput.ComboStore.Count > 0) {
			if (bbInput.ComboStore.Count > 1) {
				if (bbInput.ComboStore [0] == ButtonPress.SHOOT && bbInput.ComboStore [1] == ButtonPress.UP) {
					if (!canJumpPunch) {
						Debug.Log ("Jump Punch");
						canJumpPunch = true;
					} 
				} else if (bbInput.ComboStore [0] == ButtonPress.BLOCK && bbInput.ComboStore [1] == ButtonPress.BLOCK) {
					if (!canFistRocket) {			
						Debug.Log ("Fist Rocket");
						canFistRocket = true;
					}
				} else if (bbInput.ComboStore [0] == ButtonPress.SHOOT && bbInput.ComboStore [1] == ButtonPress.SHOOT) {
					if (!canLetsDance) {
						Debug.Log ("Lets Dance");
						canLetsDance = true;
					}
				} 
			}
			if (bbInput.ComboStore.Count > 2) {
				if (bbInput.ComboStore [0] == ButtonPress.BLOCK && bbInput.ComboStore [1] == ButtonPress.DOWN
				    && bbInput.ComboStore [2] == ButtonPress.LEFT) {
					if (!canDoubleKick) {
						Debug.Log ("Left Double Kick");
						canDoubleKick = true;
					}
				} else if (bbInput.ComboStore [0] == ButtonPress.BLOCK && bbInput.ComboStore [1] == ButtonPress.DOWN
				           && bbInput.ComboStore [2] == ButtonPress.RIGHT) {
					if (!canDoubleKick) {
						Debug.Log ("Right Double Kick");
						canDoubleKick = true;
					}
				} else if (bbInput.ComboStore [0] == ButtonPress.SHOOT && bbInput.ComboStore [1] == ButtonPress.SHOOT
				           && bbInput.ComboStore [2] == ButtonPress.DOWN) {
					if (!canMeSmash) {
						Debug.Log ("Me Smash");
						canMeSmash = true;
					}
				}
			}
		}/* else {
			canJumpPunch = false;
			canDoubleKick = false;
			canMeSmash = false;
			canFistRocket = false;
			canLetsDance = false;
		}*/
	}
}
