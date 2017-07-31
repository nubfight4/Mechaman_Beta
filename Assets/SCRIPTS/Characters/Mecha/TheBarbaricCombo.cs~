using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheBarbaricCombo : MonoBehaviour
{

	TheBarbaricInput bbInput;
	Mecha mecha;
	Goatzilla goatzilla;

	//if input is done within a time frame return true
	//return false once combo has excuted

	// Normal Attack (P2) + Up (P1)
	public bool canJumpPunch = false;
	public bool IsJumpPunchCooldown = false;

	public float jumpPunchCooldown = 500f;
	public float jumpPunchCDTimer = 0f;

	// Heavy Attack (P2) + Left or Right (P1)
	public bool canDashPunch = false;
	public bool canDashPunchLeft = false;
	public bool canDashPunchRight = false;
	public bool IsDashPunchCooldown = false;

	public float dashPunchCooldown = 500f;
	public float dashPunchCDTimer = 0f;

	// Block (P2) + Block (P1)
	public bool canMiniGame = false;
	public bool canFistRocket = false;

	void Start ()
	{
		bbInput = gameObject.GetComponent<TheBarbaricInput> ();
		mecha = gameObject.GetComponent<Mecha> ();
		goatzilla = gameObject.GetComponent<Goatzilla> ();
	}

	void Update ()
	{
		Cooldown ();
		SyncAttacks ();
		/*
		if (bbInput.ComboStore.Count > 0) {
			if (bbInput.ComboStore.Count > 1) {
				if (bbInput.ComboStore [0] == ButtonPress.FIRE02P2 && bbInput.ComboStore [1] == ButtonPress.UPP1) {
					if (!canJumpPunch) {
						Debug.Log ("Jump Punch");
						canJumpPunch = true;
					} 
				}
				if (bbInput.ComboStore [0] == ButtonPress.FIRE01P2 && bbInput.ComboStore [1] == ButtonPress.LEFTP1) {
				}
				if (bbInput.ComboStore [0] == ButtonPress.FIRE01P2 && bbInput.ComboStore [1] == ButtonPress.RIGHTP1) {
				} 
			}
			if (bbInput.ComboStore.Count > 3) {
				if (bbInput.ComboStore [0] == ButtonPress.LEFTBUMPP1 && bbInput.ComboStore [1] == ButtonPress.RIGHTBUMPP1
				    && bbInput.ComboStore [2] == ButtonPress.LEFTBUMPP2 && bbInput.ComboStore [3] == ButtonPress.RIGHTBUMPP2) {
				} 
			} else {
				canJumpPunch = false;
				canDashPunch = false;
				canFistRocket = false;
			}
		}
		*/
	}

	public void Cooldown ()
	{
		if (IsDashPunchCooldown) {
			if (dashPunchCDTimer <= dashPunchCooldown) {
				dashPunchCDTimer += Time.deltaTime * 1000f;
			} else {
				dashPunchCDTimer = 0f;
				IsDashPunchCooldown = false;
			}
		}
		if (IsJumpPunchCooldown) {
			if (jumpPunchCDTimer <= jumpPunchCooldown) {
				jumpPunchCDTimer += Time.deltaTime * 1000f;
			} else {
				jumpPunchCDTimer = 0f;
				IsJumpPunchCooldown = false;
			}
		}
	}

	public void SyncAttacks ()
	{
		if (!mecha.stopMove) {
			if (bbInput.tapFire2P2 == 1 && bbInput.holdUpP1Duration > 50f && !canJumpPunch && !IsJumpPunchCooldown) {
				Debug.Log ("JUMP PUNCH");
				canJumpPunch = true;
			}
			if (bbInput.tapFire1P2 == 1 && bbInput.holdLeftP1Duration > 50f && !canDashPunch && !canDashPunchLeft && !IsDashPunchCooldown) {
				Debug.Log ("DASH PUNCH LEFT");
				canDashPunch = true;
				canDashPunchLeft = true;
				canDashPunchRight = false;
			}
			if (bbInput.tapFire1P2 == 1 && bbInput.holdRightP1Duration > 50f && !canDashPunch && !canDashPunchRight && !IsDashPunchCooldown) {
				Debug.Log ("DASH PUNCH RIGHT");
				canDashPunch = true;
				canDashPunchRight = true;
				canDashPunchLeft = false;
			}
		}
	}
}
