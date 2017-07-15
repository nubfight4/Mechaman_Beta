﻿using System.Collections;
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
		FISTROCKET = 9,
		TOTAL}

	;

	public Vector3 gamepadPos;
	Vector3 minPos;
	Vector3 maxPos;

	private Animator anim;
	private SpriteRenderer sprite;
	public int state;
	public int dMG;

	//! Combo and Synchronise attack
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

	//prevent
	bool isOtherCombo;
	bool isJumpPunching;

	public GameObject JumpPunchCollider;
	public GameObject DashPunchCollider;

	GameObject JumpPunchColliderClone;
	GameObject DashPunchColliderClone;

	Rigidbody2D rb2d;

	public float mechSpeed = 5f;

	void Awake ()
	{
		SetMaxHP (500);
		SetHP (this.GetMaxHP ());
	}

	void Start ()
	{ 
		QButton.SetActive (false);
		EButton.SetActive (false);
		OButton.SetActive (false);
		PButton.SetActive (false);

		anim = GetComponent<Animator> ();
		state = 0;
		maxPos.x = 4.34f;
		minPos.x = -4.55f;
		//maxPos.y = -1.0f;
		//minPos.y = -2.25f;

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
		resetDuration = 1.2f;

		rb2d = gameObject.GetComponent<Rigidbody2D> ();
	}

	// Update is called once per frame
	void Update ()
	{
		CheckDeath ();
		Boundary ();
		NewMovement ();
		Combo ();
		UpdateAnimator ();
	}

	void NewMovement ()
	{
		if (Input.GetButton ("Horizontal") && !stopMove) {
			state = (int)STATE.WALKING;
			if (Input.GetAxis ("Horizontal") > 0) {
				rb2d.velocity = new Vector2 (mechSpeed, transform.position.y);
				//transform.Translate (Vector2.right * mechSpeed * Time.deltaTime);
				transform.localScale = new Vector3 (1f, 1f, 1f);
			} else if (Input.GetAxis ("Horizontal") < 0) {
				rb2d.velocity = new Vector2 (-mechSpeed, transform.position.y);
				//transform.Translate (Vector2.left * mechSpeed * Time.deltaTime);
				transform.localScale = new Vector3 (-1f, 1f, 1f);
			}
		} else {
			//rb2d.velocity = new Vector2 (0f, transform.position.y);
			state = (int)STATE.IDLE;
		}

		if (Input.GetButton ("Vertical") && Input.GetAxis ("Vertical") > 0) {

		}
		if (Input.GetButton ("Vertical") && Input.GetAxis ("Vertical") < 0) {

		}
	}

	/*
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
	*/

	void Boundary ()
	{

		if (transform.position.x > maxPos.x) {
			transform.position = new Vector3 (maxPos.x, transform.position.y);
		}

		if (transform.position.x < minPos.x) {
			transform.position = new Vector3 (minPos.x, transform.position.y);
		}

		/*
		if (transform.position.y > maxPos.y) 
		{
			transform.position = new Vector3 (transform.position.x,maxPos.y);
		}

		if (transform.position.y < minPos.y) 
		{
			transform.position = new Vector3 (transform.position.x,minPos.y);
		}
		*/
	}

	public override void CheckDeath () //highest priority to check player death
	{
		if (HP <= 0) {
			if (lives > 0) {
				lives--;
				HP = 500;
			} else {
				Destroy (gameObject, 3f);
				isAlive = false;
				SceneManager.LoadScene (5);
			}
		}
	}

	void UpdateAnimator ()
	{
		//anim.SetBool ("isJumping", isJumping);
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

	public float specialAttackDuration = 5000f;
	public float specialAttackDurationTimer = 0f;

	public GameObject RocketFistPrefab;
	GameObject RocketFistPrefabClone;

	public GameObject QButton;
	public GameObject EButton;
	public GameObject OButton;
	public GameObject PButton;

	public bool stopMove = false;

	void Combo ()
	{
		/*
		if (gamepadPos.x == 0) {
			state = (int)STATE.IDLE;
		} else {
			state = (int)STATE.WALKING;
		}
		*/

		//delay before resetting button press
		if (startReset) {
			resetTimer += Time.deltaTime;
			if (resetTimer >= resetDuration) {
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

		//Jump Punch
		if (Input.GetButton ("Vertical") && Input.GetAxis ("Vertical") > 0) { // Up + X
			if (Input.GetButtonDown ("Normal Attack") && timePressedNormal == 0) {
				state = (int)STATE.JUMPPUNCH1;
				Debug.Log ("JumpPunch");
				if (dashPunch) {
					dMG = 88;
				} else {
					dMG = 250;
				}
				startReset = true;
				resetTimer = resetDuration * 0.5f;
				isOtherCombo = true;
				isJumpPunching = true;
			}
		}

		/*
		if(isJumpPunching)
		{
			gameObject.transform.Translate (Vector2.up * jumpPunchForce * Time.deltaTime);
		}

		if (isJumpPunching) 
		{
			if (jumpPunchDurationTimer <= jumpPunchDuration) 
			{
				state = (int)STATE.JUMPPUNCH1;
				jumpPunchDurationTimer += Time.deltaTime * 1000f;
			} 
			else
			{
				state = (int)STATE.IDLE;
				Destroy (JumpPunchColliderClone);
				jumpPunchDurationTimer = 0f;
				isJumpPunching = false;
				instantOnce = false;
			}
			
			//gameObject.transform.Translate (Vector2.up * jumpPunchForce * Time.deltaTime);
			
			if (!instantOnce) {
				JumpPunchColliderClone = Instantiate (JumpPunchCollider, new Vector2 (transform.position.x, transform.position.y + 1), Quaternion.identity);
				JumpPunchColliderClone.transform.parent = gameObject.transform;
				instantOnce = true;
			}
		}
		*/

		//Dash Punch
		if (Input.GetButton ("Horizontal") && Input.GetAxis ("Horizontal") > 0) { // A + Mech Move Right
			if (Input.GetButtonDown ("Heavy Attack") && isJumpPunching == false && timePressedHeavy == 0) {
				Debug.Log ("Dash attack right");
				state = (int)STATE.DASHPUNCH1;
				//startReset = true;
				isOtherCombo = true;
				dashPunch = true;
				dashPunchRight = true;
				dashPunchLeft = false;
			}
		} else if (Input.GetButton ("Horizontal") && Input.GetAxis ("Horizontal") < 0) { //A + Mech Move Left
			if (Input.GetButtonDown ("Heavy Attack") && isJumpPunching == false && timePressedHeavy == 0) {
				Debug.Log ("Dash attack left");
				state = (int)STATE.DASHPUNCH1;
				//startReset = true;
				isOtherCombo = true;
				dashPunch = true;
				dashPunchRight = false;
				dashPunchLeft = true;
			}
		}
			
		if (dashPunch) {
			if (dashPunchDurationTimer <= dashPunchDuration) {
				dMG = 170;
				dashPunchDurationTimer += Time.deltaTime * 1000f;
			} else {
				//state = (int)STATE.IDLE;
				//Destroy (DashPunchColliderClone);
				dashPunchDurationTimer = 0f;
				dashPunch = false;
				dashPunchLeft = false;
				dashPunchRight = false;
				instantOnce = false;
			}

			if (dashPunchLeft && !dashPunchRight) {
				gameObject.transform.Translate (Vector2.left * dashPunchForce * Time.deltaTime);
				/*
				if (!instantOnce) {
					DashPunchColliderClone = Instantiate (DashPunchCollider, new Vector2 (transform.position.x - 1, transform.position.y), Quaternion.identity);
					DashPunchColliderClone.transform.parent = gameObject.transform;
					instantOnce = true;
				}
				*/
			} else if (dashPunchRight && !dashPunchLeft) {
				gameObject.transform.Translate (Vector2.right * dashPunchForce * Time.deltaTime);
				/*
				if (!instantOnce) {
					DashPunchColliderClone = Instantiate (DashPunchCollider, new Vector2 (transform.position.x + 1, transform.position.y), Quaternion.identity);
					DashPunchColliderClone.transform.parent = gameObject.transform;
					instantOnce = true;
				}
				*/
			}
		}

		//normal combo
		if (timePressedNormal < 4) {
			if (Input.GetButtonDown ("Normal Attack")) { 

				if (!isOtherCombo) {
					startReset = true;
					timePressedHeavy = 0;
					if (timePressedNormal == 0) {
						state = (int)STATE.PUNCH;
						dMG = 60;
						resetTimer = 0;
						timePressedNormal++;
					} else if (timePressedNormal == 1) {
						state = (int)STATE.WHATSUP;
						dMG = 130;
						resetTimer = 0;
						timePressedNormal++;
					} else if (timePressedNormal == 2) {
						state = (int)STATE.SHADOW;
						dMG = 200;
					} 
				}
			}
		}

		if (timePressedNormal >= 3) {
			resetTimer = resetDuration;
			Debug.Log ("OVER =3");
			timePressedNormal = 0;
			state = (int)STATE.IDLE;
		}

		//heavy combo

		/*
		if (Input.GetButtonDown("Heavy Attack")) 
		{
			Debug.Log(timePressedHeavy);
			if(!isOtherCombo)
			{
				timePressedNormal = 0;
				startReset = true;
				if(timePressedHeavy < 5)
				{
					if(timePressedHeavy == 0)
					{
						state = (int)STATE.HEAVY;
						resetTimer = 0;
						timePressedHeavy++;
					}
					else if(timePressedHeavy >= 1)
					{
						state = (int)STATE.DOUBLETROUBLE;
					}
				}
				else
				{
					resetTimer = resetDuration;
					timePressedHeavy = 0;
					state = (int)STATE.IDLE;
				}
			}
		}
		*/

		Debug.Log (timePressedHeavy);
		if (timePressedHeavy < 3) {
			if (Input.GetButtonDown ("Heavy Attack")) { 
				if (!isOtherCombo) {
					Debug.Log (timePressedHeavy);
					startReset = true;
					timePressedNormal = 0;
					if (timePressedHeavy == 0) {
						state = (int)STATE.HEAVY;
						dMG = 100;
						resetTimer = 0;
						timePressedHeavy++;
						Debug.Log ("HEAVY1");
					} else if (timePressedHeavy == 1) {
						Debug.Log ("HEAVY2");
						state = (int)STATE.DOUBLETROUBLE;
						dMG = 250;
					}
				}
			}
		}

		/*
		if(timePressedHeavy >= 3)
		{
			resetTimer = resetDuration;
			Debug.Log("OVER =3");
			timePressedHeavy = 0;
			state = (int)STATE.IDLE;
		}
		*/

		//Ultimate
		if (Input.GetButtonDown ("Bumper_Left_P1")) {
			Debug.Log ("P1.L.Bumper");
			startReset = true;
			p1LPressed = true;
			if (isMinigame && randomNP1 == 0) {
				QButton.SetActive (false);
				correctPressCounter++;
			} else {
				Debug.Log ("WRONG PRESS");
				isRandomNP1 = false;
				isRandomNP2 = false;
			}

		}
		if (Input.GetButtonDown ("Bumper_Right_P1")) {
			Debug.Log ("P1.R.Bumper");
			startReset = true;
			p1RPressed = true;
			if (isMinigame && randomNP1 == 1) {
				EButton.SetActive (false);
				correctPressCounter++;
			} else {
				Debug.Log ("WRONG PRESS");
				isRandomNP1 = false;
				isRandomNP2 = false;
			}

		}
		if (Input.GetButtonDown ("Bumper_Left_P2")) {
			Debug.Log ("P2.L.Bumper");
			startReset = true;
			p2LPressed = true;
			if (isMinigame && randomNP2 == 0) {
				OButton.SetActive (false);
				correctPressCounter++;
			} else {
				Debug.Log ("WRONG PRESS");
				isRandomNP1 = false;
				isRandomNP2 = false;
			}
		}
		if (Input.GetButtonDown ("Bumper_Right_P2")) {
			Debug.Log ("P2.R.Bumper");
			startReset = true;
			p2RPressed = true;
			if (isMinigame && randomNP2 == 1) {
				PButton.SetActive (false);
				correctPressCounter++;
			} else {
				Debug.Log ("WRONG PRESS");
				isRandomNP1 = false;
				isRandomNP2 = false;
			}
		}

		//play animation, use animation function to spawn the fist at specific frame
		//just make it like respawn from thin air or some shit
		//rocket fist retain previous behaviour on separate script
		if (p1LPressed == true && p1RPressed == true && p2LPressed == true && p2RPressed == true && !canSpecial) {
			Debug.Log ("UltimateGG");
			canSpecial = true;
			isMinigame = true;
			stopMove = true;
			//RocketFistPrefabClone = Instantiate (RocketFistPrefab, transform.position, Quaternion.Euler (0f, 0f, 90f));
		}

		if (isFistRocket) {
			//RocketFistPrefabClone = Instantiate (RocketFistPrefab, transform.position, Quaternion.Euler (0f, 0f, 90f));
			state = (int)STATE.FISTROCKET;
			isFistRocket = false;
		}

		if (isMinigame) {
			if (specialAttackDurationTimer <= specialAttackDuration) {
				specialAttackDurationTimer += Time.deltaTime * 1000f;

				if (correctPressCounter == 2) { //the part where the damage will increase and then reset counter
					correctPressCounter = 0;
					tempBonusDamage += 20;
					isRandomNP1 = false;
					isRandomNP2 = false;
				}

				if (!isRandomNP1) {
					QButton.SetActive (false);
					EButton.SetActive (false);
					randomNP1 = Random.Range (0, 2);
				} 
				if (!isRandomNP2) {
					OButton.SetActive (false);
					PButton.SetActive (false);
					randomNP2 = Random.Range (0, 2);
				}

				if (randomNP1 == 0 && !isRandomNP1) { //Q
					QButton.SetActive (true);
					isRandomNP1 = true;
				} else if (randomNP1 == 1 && !isRandomNP1) { //E
					EButton.SetActive (true);
					isRandomNP1 = true;
				}

				if (randomNP2 == 0 && !isRandomNP2) { //O
					OButton.SetActive (true);
					isRandomNP2 = true;
				} else if (randomNP2 == 1 && !isRandomNP2) { //P
					PButton.SetActive (true);
					isRandomNP2 = true;
				}
			} else {
				specialAttackDurationTimer = 0f;
				isRandomNP1 = false;
				isRandomNP2 = false;
				isMinigame = false;
				isFistRocket = true;
				QButton.SetActive (false);
				EButton.SetActive (false);
				OButton.SetActive (false);
				PButton.SetActive (false);
				correctPressCounter = 0;
			}
		}
	}

	public bool canSpecial = false;
	public bool isFistRocket = false;
	public bool isMinigame = false;
	public bool isRandomNP1 = false;
	public bool isRandomNP2 = false;
	public int randomNP1 = 0;
	public int randomNP2 = 0;
	public int tempBonusDamage = 0;
	public int fistRocketDamage = 300;

	public int correctPressCounter = 0;

	public void SpawnFist ()
	{
		RocketFistPrefabClone = Instantiate (RocketFistPrefab, transform.position, Quaternion.identity);
	}

	//need to get the icons, get placeholder for "q", "e", "o", "p" [q,e (P1)] [o,p (P2)]
	//when activated start randomizing between int 0 to 1 for P1 and 0 to 1 for P2
	//spawn corresponding icon for the random
	//if correct press, add damage (TEMP DAMAGE FOR RECEIVE DAMAGE)
	//if wrong press, skip and re-roll the random
	//after the duration ends, mech will launch fist and deal damage + bonus damage
}