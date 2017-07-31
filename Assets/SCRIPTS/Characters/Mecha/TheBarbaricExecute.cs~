using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheBarbaricExecute : MonoBehaviour
{
	//checks from the barbaric combo to see if boolean returns true
	//add the functions related to the barbaric combo
	//return false after executing combo

	public float jumpPunchDuration = 100f;
	public float jumpPunchDurationTimer = 0f;

	public float dashPunchDuration = 100f;
	public float dashPunchDurationTimer = 0f;
	public float dashPunchForce = 10f;

	TheBarbaricInput bbInput;
	TheBarbaricCombo bbCombo;
	Mecha mecha;
	Animator anim;

	void Start ()
	{
		bbInput = gameObject.GetComponent <TheBarbaricInput> ();
		bbCombo = gameObject.GetComponent<TheBarbaricCombo> ();
		mecha = gameObject.GetComponent<Mecha> ();
		anim = gameObject.GetComponent<Animator> ();
	}

	void Update ()
	{
		if (bbCombo.canJumpPunch) {
			mecha.dMG = 250;
			mecha.stopMove = true;
			mecha.state = (int)STATE.JUMPPUNCH;
			bbInput.KeyTapReset ();

			if (jumpPunchDurationTimer <= jumpPunchDuration) {
				jumpPunchDurationTimer += Time.deltaTime * 1000f;
			} else {
				jumpPunchDurationTimer = 0f;
				bbCombo.canJumpPunch = false;
				mecha.stopMove = false;
				bbCombo.IsJumpPunchCooldown = true;
			}
		}

		if (bbCombo.canDashPunch && bbCombo.canDashPunchLeft) {
			mecha.dMG = 250;
			mecha.stopMove = true;
			mecha.state = (int)STATE.DASHPUNCH;
			bbInput.KeyTapReset ();

			if (dashPunchDurationTimer <= dashPunchDuration) {
				dashPunchDurationTimer += Time.deltaTime * 1000f;
				transform.localScale = new Vector2 (-1f, 1f);
				gameObject.transform.Translate (Vector2.left * dashPunchForce * Time.deltaTime);
			} else {
				dashPunchDurationTimer = 0f;
				mecha.stopMove = false;
				bbCombo.canDashPunch = false;
				bbCombo.canDashPunchLeft = false;
				bbCombo.IsDashPunchCooldown = true;
			}


		} else if (bbCombo.canDashPunch && bbCombo.canDashPunchRight) {
			mecha.dMG = 250;
			mecha.stopMove = true;
			mecha.state = (int)STATE.DASHPUNCH;
			bbInput.KeyTapReset ();

			if (dashPunchDurationTimer <= dashPunchDuration) {
				dashPunchDurationTimer += Time.deltaTime * 1000f;
				transform.localScale = new Vector2 (1f, 1f);
				gameObject.transform.Translate (Vector2.right * dashPunchForce * Time.deltaTime);
			} else {
				dashPunchDurationTimer = 0f;
				mecha.stopMove = false;
				bbCombo.canDashPunch = false;
				bbCombo.canDashPunchRight = false;
				bbCombo.IsDashPunchCooldown = true;
			}
		}
	}
}
