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
		FISTROCKET = 9,
		TOTAL
	};

	public Vector3 gamepadPos;
	Vector3 minPos;
	Vector3 maxPos;

	//	[Header("Movement")]
	//	public float MovementSpeed;
	float translation = 0.0f;

	private Animator anim;
	private SpriteRenderer sprite;
	public int state;
	public int dMG;
	public bool isJumping;
	public bool dashPunch;
	bool isStop;
	bool faceRight;
	bool faceLeft;
	//! Combo and Syncgronise attack
	int timePressedNormal;
	int timePressedHeavy;
	//reset
	bool startReset;
	float resetTimer;
	float resetDuration;

	//hitsparkreset
	bool hitstartReset;
	//prevent
	bool isOtherCombo;

	public GameObject RocketFistPrefab;
	public GameObject JumpPunchCollider;
	public GameObject DashPunchCollider;

	GameObject RocketFistPrefabClone;
	GameObject JumpPunchColliderClone;
	GameObject DashPunchColliderClone;


	//Special Attack
	bool p1LPressed;
	bool p1RPressed;
	bool p2LPressed;
	bool p2RPressed;
	bool isFistRocket;

	Goatzilla goatzilla;
	public int maxCharge = 100;
	public int currentCharge = 0;
	public int specialCharge = 80;

	public float GetCurrentChargePercentage ()
	{
		return currentCharge * 100 / maxCharge;
	}

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
		dashPunch = false;
		isStop = false;
		maxPos.x = 4.6f;
		minPos.x = -4.75f;
		anim = GetComponent<Animator> ();
		isOtherCombo = false;
		startReset = false;
		p1LPressed = false;
		p1RPressed = false;
		p2LPressed = false;
		p2RPressed = false;
		timePressedNormal = 0;
		timePressedHeavy = 0;
		resetTimer = 0f;
		resetDuration = 0.7f;

		goatzilla = FindObjectOfType<Goatzilla> ();
	}

	// Update is called once per frame
	void Update ()
	{
		if (PauseOnPress.Instance.paused != true) 
		{
			CheckDeath();
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
					isJumping = false;
					dashPunch = false;
					p1LPressed = false;
					p1RPressed = false;
					p2LPressed = false;
					p2RPressed = false;
					anim.ResetTrigger("WhatsUp");
					anim.ResetTrigger("ShadowStrike");
					anim.ResetTrigger("DoubleTrouble");
				}
			}
			Movement();
			Boundary ();

			if(!isJumping)
			{
				transform.position = gamepadPos + transform.position;
				Movement ();
			}
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
			//transform.localScale = new Vector3 (-1, transform.localScale.y);
			transform.Translate(Vector3.left * Time.deltaTime * translation);
			transform.localScale = new Vector3 (-1,1,1);

		}
		if (gamepadPos.x > 0.01875) {
			//transform.localScale = new Vector3 (1, transform.localScale.y);
			transform.Translate(Vector3.right * Time.deltaTime * translation);
			transform.localScale = new Vector3 (1,1,1);

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
		anim.SetBool ("isJumping", isJumping);
		anim.SetBool ("isStop", isStop);
		//		anim.SetInteger ("State", state);
		anim.SetBool ("Chaining", startReset);
	}

	public bool instantOnce = false;
	public float jumpPunchDuration = 100f;
	public float jumpPunchDurationTimer = 0f;
	public float jumpPunchForce = 0.1f;
	public float jumpPunchDamping = 0.5f;

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

		//Dash punch (Terrence)
		if (Input.GetAxis("Horizontal")> 0) // A + Mech Move Right
		{ 
			if (Input.GetButtonDown ("Heavy Attack") && isJumping == false) 
			{
				dashPunch = true;
				isOtherCombo = true;
				startReset = true;
				faceRight = true;
				faceLeft = false;
				anim.SetTrigger("DashPunch");

			}
		} else if (Input.GetAxis("Horizontal") < 0) //A + Mech Move Left
		{ 
			if (Input.GetButtonDown ("Heavy Attack") && isJumping == false) 
			{
				dashPunch = true;
				isOtherCombo = true;
				startReset = true;
				faceRight = false;
				faceLeft = true;
				anim.SetTrigger("DashPunch");
			}
		}

		if(dashPunch)
		{
			dMG = 180;
			if(faceRight)
			{
				gameObject.transform.Translate (Vector2.right * dashPunchForce * Time.deltaTime);
			}
			else if(faceLeft)
			{
				gameObject.transform.Translate (Vector2.left * dashPunchForce * Time.deltaTime);
			}
		}

		//Jump Punch (Terrence)
		if (Input.GetAxis ("Vertical") > 0) { // Up + X
			if (Input.GetButtonDown ("Normal Attack") && timePressedNormal == 0) 
			{
				anim.SetTrigger("JumpPunch");
				if(dashPunch)
				{
					dMG = 180;

				}
//				else
//				{
//					dMG = 180;
//				}
				startReset = true;
				isOtherCombo = true;
				isJumping = true;
			}
		}

		//normal Punch
		if (Input.GetButtonDown ("Normal Attack")) 
		{ 
			startReset = true;
			timePressedHeavy = 0;
			if (!isOtherCombo) 
			{
				if (timePressedNormal == 0) 
				{
					anim.SetTrigger("Punch");
					resetTimer = 0;
					dMG = 40;
					timePressedNormal++;

				} 
				else if (timePressedNormal == 1) 
				{
					//state = (int)STATE.WHATSUP;
					anim.SetTrigger("WhatsUp");
					resetTimer = 0;
					dMG = 60;
					timePressedNormal++;
				}
				else if(timePressedNormal == 2)
				{
					//state = (int)STATE.SHADOW;
					anim.SetTrigger("ShadowStrike");
					dMG = 80;
				} 
			}
		}
		//Heavy Punch
		if (Input.GetButtonDown("Heavy Attack")) 
		{ 
			if(!isOtherCombo)
			{
				startReset = true;
				timePressedNormal = 0;
				if(timePressedHeavy == 0)
				{
					anim.SetTrigger("HeavyAttack");
					dMG = 70;
					timePressedHeavy++;

				}
				else if(timePressedHeavy == 1)
				{
					anim.SetTrigger("DoubleTrouble");
					dMG = 180;
				}
			}
		}

		if(timePressedNormal >= 3)
		{
			timePressedNormal = 0;
		}
		if(timePressedHeavy >= 2)
		{
			timePressedHeavy = 0;
		}
	
		//Ultimate
		if (Input.GetButtonDown ("Bumper_Left_P1")) {
			Debug.Log ("P1.L.Bumper");
			startReset = true;
			p1LPressed = true;
		
		}
		if (Input.GetButtonDown ("Bumper_Right_P1")) {
			Debug.Log ("P1.R.Bumper");
			startReset = true;
			p1RPressed = true;

		}
		if (Input.GetButtonDown ("Bumper_Left_P2")) {
			Debug.Log ("P2.L.Bumper");
			startReset = true;
			p2LPressed = true;

		}
		if (Input.GetButtonDown ("Bumper_Right_P2")) {
			Debug.Log ("P2.R.Bumper");
			startReset = true;
			p2RPressed = true;
		
		}
			
		if (p1LPressed == true && p1RPressed == true && p2LPressed == true && p2RPressed == true) {
			Debug.Log ("UltimateGG");
			if (currentCharge >= specialCharge) {
				if (transform.position.x < goatzilla.transform.position.x) {
					transform.localScale = new Vector2 (1f, 1f);
				} else if (transform.position.x > goatzilla.transform.position.x) {
					transform.localScale = new Vector2 (-1f, 1f);
				}
				anim.SetTrigger("FistRocket");
				currentCharge -= specialCharge;
//				canSpecial = true;
//				isMinigame = true;
//				stopMove = true;
			}
		}

		if (isFistRocket) {
			state = (int)STATE.FISTROCKET;
			isFistRocket = false;
		}

	
	}

	void SpawnFist ()
	{
		RocketFistPrefabClone = Instantiate (RocketFistPrefab, new Vector2 (transform.position.x, transform.position.y + 1), Quaternion.identity);
	}

	void WalkSound()
	{
		SoundManagerScript.Instance.PlaySFX(AudioClipID.SFX_MECHAMAN_WALKING);
	}

	void NormalPunchSound()
	{
		SoundManagerScript.Instance.PlaySFX(AudioClipID.SFX_MECHAMAN_NORMAL_PUNCH);
	}

	void HeavyPunchSound()
	{
		SoundManagerScript.Instance.PlaySFX(AudioClipID.SFX_MECHAMAN_HEAVY_PUNCH);
	}

	void DashPunchSound()
	{
		SoundManagerScript.Instance.PlaySFX(AudioClipID.SFX_MECHAMAN_DASH_PUNCH);
	}



}