using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Mecha : LifeObject 
{
	public enum STATE
	{
		IDLE = 0,
		WALKING = 1,
		PUNCH = 2,
		WHATSUP = 3,
		SHADOW = 4,
		HEAVY = 5,
		DOUBLETROUBLE = 6,
		JUMPPUNCH1 = 7,
		DASHPUNCH1 = 8,
		TOTAL
	};

	public Vector3 gamepadPos;
	Vector3 minPos;
	Vector3 maxPos;

	private Animator anim;
	private SpriteRenderer sprite;
	public int state;
	public int dMG;
	bool isJumping;
	bool isStop;
	//! Combo and Syncgronise attack
	bool p1LPressed;
	bool p1RPressed;
	bool p2LPressed;
	bool p2RPressed;
	int timePressedNormal;
	int timePressedHeavy;
	//reset
	bool startReset;
	float resetTimer;
	float resetDuration;

	//hitsparkreset
	bool hitstartReset;
	float hitresetTimer = 0.0f;
	float hitresetDuration = 1.0f;
	//prevent
	bool isOtherCombo;
	bool isJumpPunching;

	public GameObject JumpPunchCollider;
	public GameObject DashPunchCollider;

	GameObject JumpPunchColliderClone;
	GameObject DashPunchColliderClone;

	void Awake()
	{
		SetMaxHP (500);
		SetHP (this.GetMaxHP ());
	}

	// Use this for initialization
	void Start ()
	{ 
		anim = GetComponent<Animator> ();
		state = 0;
		isJumping = false; 
		isStop = false;
		maxPos.x = 4.34f;
		//maxPos.y = -1.0f;
		minPos.x = -4.55f;
		//minPos.y = -2.25f;
		anim = GetComponent<Animator> ();
		isOtherCombo = false;
		isJumpPunching = false;
		startReset = false;
		p1LPressed = false;
		p1RPressed = false;
		p2LPressed = false;
		p2RPressed = false;
		timePressedNormal = 0;
		timePressedHeavy = 0;
		resetTimer = 0f;
		//resetDuration = 0.7f;
		resetDuration = 1.2f;
	}

	// Update is called once per frame
	void Update ()
	{
		if (PauseOnPress.Instance.paused != true) {
			CheckDeath();
			Debug.Log (state);
			Boundary ();
			gamepadPos.x = Input.GetAxis ("Horizontal");
			//gamepadPos.y = Input.GetAxis ("Vertical");
			if(!isJumpPunching)
			{
				Debug.Log("I can Move");
				transform.position = gamepadPos + transform.position;
				Movement ();
			}
			//transform.position = gamepadPos + transform.position;
			//Movement ();
			//Movement ();
			Combo ();
			UpdateAnimator ();
		}

	}

	void OnHitSpark()
	{
		GameObject.FindGameObjectWithTag ("PlayerHitSpark").GetComponent<SpriteRenderer> ().enabled = true;
	}

	void OffHitSpark()
	{
		GameObject.FindGameObjectWithTag ("PlayerHitSpark").GetComponent<SpriteRenderer> ().enabled = false;
	}

	void Movement ()
	{
		gamepadPos.x = Input.GetAxis ("Horizontal");
		//gamepadPos.y = Input.GetAxis ("Vertical");
		transform.position = gamepadPos + transform.position;
		if (gamepadPos.x < -0.0187) {
			transform.localScale = new Vector3 (-1, transform.localScale.y);
		}
		if (gamepadPos.x > 0.01875) {
			transform.localScale = new Vector3 (1, transform.localScale.y);	
		}
	}

	void Boundary ()
	{

		if (transform.position.x > maxPos.x) {
			transform.position = new Vector3 (maxPos.x, transform.position.y);
		}

		if (transform.position.x < minPos.x) {
			transform.position = new Vector3 (minPos.x, transform.position.y);
		}

//		if (transform.position.y > maxPos.y) 
//		{
//			transform.position = new Vector3 (transform.position.x,maxPos.y);
//		}
//
//		if (transform.position.y < minPos.y) 
//		{
//			transform.position = new Vector3 (transform.position.x,minPos.y);
//		}
	}

	public override void CheckDeath ()
	{
		if (HP <= 0) 
		{
			if(lives > 0)
			{
				lives--;
				HP = 500;
			}
			else
			{
				Destroy (gameObject, 3f);
				isAlive = false;
				SceneManager.LoadScene (5);
			}
		}
	}

	void UpdateAnimator ()
	{
		anim.SetFloat ("Speed", gamepadPos.x);
		//anim.SetBool ("isJumping", isJumping);
		anim.SetBool ("isStop", isStop);
		anim.SetInteger ("State", state);
	}

	public bool instantOnce = false;

	public float jumpPunchDuration = 100f;
	public float jumpPunchDurationTimer = 0f;
	public float jumpPunchForce = 0.1f;
	public float jumpPunchDamping = 0.5f;

	public bool dashPunch = false;
	public bool dashPunchLeft = false;
	public bool dashPunchRight = false;
	public float dashPunchDuration = 100f;
	public float dashPunchDurationTimer = 0f;
	public float dashPunchForce = 10f;

	void Combo ()
	{
		if (gamepadPos.x == 0) 
		{
			isStop = true;
		} 
		else 
		{
			isStop = false;
		}
		//delay
		if (startReset) 
		{
			resetTimer += Time.deltaTime;
			if (resetTimer >= resetDuration) 
			{
				resetTimer = 0;
				timePressedNormal = 0;
				timePressedHeavy = 0;
				startReset = false;
				isOtherCombo = false;
				isJumpPunching = false;
				//dashPunch = false;
				p1LPressed = false;
				p1RPressed = false;
				p2LPressed = false;
				p2RPressed = false;
				resetDuration = 1.0f;
				state = (int)STATE.IDLE;
			}
		}

		//animation
		//when activate, do first jump punch
		//after it ends, freeze at fist holding up
		//then return to idle

		//Jump Punch (Terrence)
		if (Input.GetAxis ("Vertical") > 0) { // Up + X
			if (Input.GetButtonDown ("Normal Attack") && timePressedNormal == 0) 
			{
				state = (int)STATE.JUMPPUNCH1;
				if(dashPunch)
				{
					dMG = 88;
				}
				else
				{
					dMG = 250;
				}
				Debug.Log ("JumpPunch");
				startReset = true;
				resetTimer = resetDuration * 0.5f;
				isOtherCombo = true;
				isJumpPunching = true;
			}
		}

//		if(isJumpPunching)
//		{
//			gameObject.transform.Translate (Vector2.up * jumpPunchForce * Time.deltaTime);
//		}

//		if (isJumpPunching) 
//		{
//			if (jumpPunchDurationTimer <= jumpPunchDuration) 
//			{
//				state = (int)STATE.JUMPPUNCH1;
//				jumpPunchDurationTimer += Time.deltaTime * 1000f;
//			} 
//			else
//			{
//				state = (int)STATE.IDLE;
//				Destroy (JumpPunchColliderClone);
//				jumpPunchDurationTimer = 0f;
//				isJumpPunching = false;
//				instantOnce = false;
//			}
//				gameObject.transform.Translate (Vector2.up * jumpPunchForce * Time.deltaTime);
//			/* 
//			pseudocode for looking for specific animation
//			if (anim.frame == 5) {
//				anim.frame.pause;
//			}
//			*/
//			if (!instantOnce) {
//				JumpPunchColliderClone = Instantiate (JumpPunchCollider, new Vector2 (transform.position.x, transform.position.y + 1), Quaternion.identity);
//				JumpPunchColliderClone.transform.parent = gameObject.transform;
//				instantOnce = true;
//			}
//		}

		//Dash punch (Terrence)
		if (gamepadPos.x > 0) { // A + Mech Move Right
			if (Input.GetButtonDown ("Heavy Attack") && isJumpPunching == false && timePressedHeavy == 0) 
			{
				Debug.Log ("Dash attack right");
				//startReset = true;
				isOtherCombo = true;
				dashPunch = true;
				dashPunchRight = true;
				dashPunchLeft = false;
			}
		} else if (gamepadPos.x < 0) { //A + Mech Move Left
			if (Input.GetButtonDown ("Heavy Attack") && isJumpPunching == false && timePressedHeavy == 0) 
			{
				Debug.Log ("Dash attack left");
				//startReset = true;
				isOtherCombo = true;
				dashPunch = true;
				dashPunchRight = false;
				dashPunchLeft = true;
			}
		}
			
		if (dashPunch) {
			if (dashPunchDurationTimer <= dashPunchDuration) 
			{
				state = (int)STATE.DASHPUNCH1;
				dMG = 170;
				dashPunchDurationTimer += Time.deltaTime * 1000f;
			} 
			else 
			{
				//state = (int)STATE.IDLE;
				//Destroy (DashPunchColliderClone);
				dashPunchDurationTimer = 0f;
				dashPunch = false;
				dashPunchLeft = false;
				dashPunchRight = false;
				instantOnce = false;
			}

			if (dashPunchLeft && !dashPunchRight) 
			{
				gameObject.transform.Translate (Vector2.left * dashPunchForce * Time.deltaTime);
				if (!instantOnce) 
				{
					//DashPunchColliderClone = Instantiate (DashPunchCollider, new Vector2 (transform.position.x - 1, transform.position.y), Quaternion.identity);
					//DashPunchColliderClone.transform.parent = gameObject.transform;
					instantOnce = true;
				}
			} else if (dashPunchRight && !dashPunchLeft) 
			{
				gameObject.transform.Translate (Vector2.right * dashPunchForce * Time.deltaTime);
				if (!instantOnce) 
				{
					//DashPunchColliderClone = Instantiate (DashPunchCollider, new Vector2 (transform.position.x + 1, transform.position.y), Quaternion.identity);
					//DashPunchColliderClone.transform.parent = gameObject.transform;
					instantOnce = true;
				}
			}
		}

		//normal combo
		if (timePressedNormal < 4) 
		{
			if (Input.GetButtonDown ("Normal Attack")) 
			{ 

				if (!isOtherCombo) 
				{
					startReset = true;
					timePressedHeavy = 0;
					if (timePressedNormal == 0) 
					{
						state = (int)STATE.PUNCH;
						dMG = 60;
						resetTimer = 0;
						timePressedNormal++;
					} 
					else if (timePressedNormal == 1) 
					{
						state = (int)STATE.WHATSUP;
						dMG = 130;
						resetTimer = 0;
						timePressedNormal++;
					}
					else if(timePressedNormal == 2)
					{
						state = (int)STATE.SHADOW;
						dMG = 200;
					} 
				}
			}
		}

		if (timePressedNormal >= 3) 
		{
			resetTimer = resetDuration;
			Debug.Log ("OVER =3");
			timePressedNormal = 0;
			state = (int)STATE.IDLE;
		}

		//heavy combo
//		if (Input.GetButtonDown("Heavy Attack")) 
//		{
//			Debug.Log(timePressedHeavy);
//			if(!isOtherCombo)
//			{
//				timePressedNormal = 0;
//				startReset = true;
//				if(timePressedHeavy < 5)
//				{
//					if(timePressedHeavy == 0)
//					{
//						state = (int)STATE.HEAVY;
//						resetTimer = 0;
//						timePressedHeavy++;
//					}
//					else if(timePressedHeavy >= 1)
//					{
//						state = (int)STATE.DOUBLETROUBLE;
//					}
//				}
//				else
//				{
//					resetTimer = resetDuration;
//					timePressedHeavy = 0;
//					state = (int)STATE.IDLE;
//				}
//			}
//		}
		Debug.Log(timePressedHeavy);
		if(timePressedHeavy < 3)
		{
			if (Input.GetButtonDown("Heavy Attack")) 
			{ 
				if(!isOtherCombo)
				{
					Debug.Log(timePressedHeavy);
					startReset = true;
					timePressedNormal = 0;
					if(timePressedHeavy == 0)
					{
						state = (int)STATE.HEAVY;
						dMG = 100;
						resetTimer = 0;
						timePressedHeavy++;
						Debug.Log("HEAVY1");
					}
					else if(timePressedHeavy == 1)
					{
						Debug.Log("HEAVY2");
						state = (int)STATE.DOUBLETROUBLE;
						dMG = 250;
					}
				}
			}
		}
//		if(timePressedHeavy >= 3)
//		{
//			resetTimer = resetDuration;
//			Debug.Log("OVER =3");
//			timePressedHeavy = 0;
//			state = (int)STATE.IDLE;
//		}

		//ULtimate
		if (Input.GetButtonDown ("Bumper_Left_P1")) 
		{
			startReset = true;
			p1LPressed = true;
		}
		if (Input.GetButtonDown ("Bumper_Right_P1")) 
		{
			startReset = true;
			p1RPressed = true;
		}
		if (Input.GetButtonDown ("Bumper_Left_P2")) 
		{
			startReset = true;
			p2LPressed = true;
		}
		if (Input.GetButtonDown ("Bumper_Right_P2")) 
		{
			startReset = true;
			p2RPressed = true;
		}

		if (p1LPressed == true && p1RPressed == true && p2LPressed == true && p2RPressed == true) 
		{
			Debug.Log ("UltimateGG");
		}
	}
}