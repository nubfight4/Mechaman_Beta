using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

[System.Serializable]
public class Attacks
{
	public string name;
	public int damage;
}

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
	//bool hitstartReset;
	//prevent
	bool isOtherCombo;
	[Header("KnockBack")]
	public bool isKnockBack = false;
	public float knockBackSpeed = 0f;
	public float knockBackTimer = 0f;
	public float knockBackDuration = 0f;

	[Header("Attacks")]
	const int MAX_ATACK = 9;
	public Attacks[] attackList = new Attacks[MAX_ATACK];

	[Header("MiniGame")]
	bool isMiniGame;
	const int MAX_KEY = 4;
	public SpriteRenderer sprP1;
	public SpriteRenderer sprP2;
	public GameObject[] buttons = new GameObject[2];
	public GameObject[] explosion = new GameObject[2];
	public Sprite[] p1ButtonSet = new Sprite[MAX_KEY];
	public Sprite[] p2ButtonSet = new Sprite[MAX_KEY];
	bool[] isPressed = new bool[2];
	bool startPressedTimer;
	float miniGameTimer = 0;
	float pressedTimer = 0;
	public float pressedWindowDuration;
	public float miniGameDuration;
	int randNum = 0; //random for p1 button
	int correctPressed = 0;



	public GameObject RocketFistPrefab;
	//public GameObject JumpPunchCollider;
	//public GameObject DashPunchCollider;

	GameObject RocketFistPrefabClone;
//	GameObject JumpPunchColliderClone;
//	GameObject DashPunchColliderClone;


	//Special Attack
	bool p1LPressed;
	bool p1RPressed;
	bool p2LPressed;
	bool p2RPressed;

	Goatzilla goatzilla;
	public int maxCharge = 100;
	public int currentCharge = 0;
	public int specialCharge = 80;

	//public bool instantOnce = false;
	//public float jumpPunchDuration = 100f;
	//public float jumpPunchDurationTimer = 0f;
	//public float jumpPunchForce = 0.1f;
	//public float jumpPunchDamping = 0.5f;

	//public bool dashPunchLeft = false;
	//public bool dashPunchRight = false;
	//public float dashPunchDuration = 100f;
	//public float dashPunchDurationTimer = 0f;
	public float dashPunchForce = 10f;

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
		isMiniGame = false;
		timePressedNormal = 0;
		timePressedHeavy = 0;
		resetTimer = 0f;
		resetDuration = 0.7f;
		goatzilla = FindObjectOfType<Goatzilla> ();
		sprP1 = buttons[0].GetComponent<SpriteRenderer>();
		sprP2 = buttons[1].GetComponent<SpriteRenderer>();
		isPressed[0] = false;
		isPressed[1] = false;
		startPressedTimer = false;
		RandomButtonNumber();
	}

	// Update is called once per frame
	void Update ()
	{
		Debug.Log(correctPressed);
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
				}
			}
			if(!isMiniGame)
			{
				Movement();
				Boundary ();

				if(!isJumping)
				{
					transform.position = gamepadPos + transform.position;
					Movement ();
				}
				Combo ();
			}
			else if(isMiniGame)
			{
				miniGameTimer += Time.deltaTime;
				buttons[0].SetActive(true);
				buttons[1].SetActive(true);
				MiniGame();
				if(miniGameTimer >= miniGameDuration)
				{
					dMG = (correctPressed * 20) + attackList[7].damage;
					anim.SetTrigger("FistRocket");
					correctPressed = 0;
					isMiniGame = false;
					buttons[0].SetActive(false);
					buttons[1].SetActive(false);
				}
			}
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
		//anim.SetBool ("isJumping", isJumping);
		anim.SetBool ("isStop", isStop);
		//		anim.SetInteger ("State", state);
		anim.SetBool ("Chaining", startReset);
	}

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
			dMG = attackList[6].damage;
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
		if (Input.GetAxis ("Vertical") > 0) 
		{ // Up + X
			if (Input.GetButtonDown ("Normal Attack") && timePressedNormal == 0) 
			{
				anim.SetTrigger("JumpPunch");
				dMG = attackList[5].damage;
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
					dMG = attackList[0].damage;
					timePressedNormal++;

				} 
				else if (timePressedNormal == 1) 
				{
					//state = (int)STATE.WHATSUP;
					anim.SetTrigger("WhatsUp");
					resetTimer = 0;
					dMG = attackList[1].damage;
					timePressedNormal++;
				}
				else if(timePressedNormal == 2)
				{
					anim.SetTrigger("ShadowStrike");
					dMG = attackList[2].damage;
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
					dMG = attackList[3].damage;
					timePressedHeavy++;

				}
				else if(timePressedHeavy >= 1)
				{
					anim.SetTrigger("DoubleTrouble");
					dMG = attackList[4].damage;
					timePressedHeavy = 0;
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
				currentCharge = 0;
				isMiniGame = true;
			}
			p1LPressed = false;
			p1RPressed = false;
			p2LPressed = false;
			p2RPressed = false;
		}
	}

	void RandomButtonNumber()
	{
		randNum = Random.Range(0,MAX_KEY); //can be simplyfied lagi by using 2d array
		sprP1.sprite = p1ButtonSet[randNum];
		randNum = Random.Range(0,MAX_KEY);
		sprP2.sprite = p2ButtonSet[randNum];
	}

	void MiniGame()
	{
		if(startPressedTimer)
		{
			pressedTimer += Time.deltaTime;
			if(pressedTimer >= pressedWindowDuration)
			{
				startPressedTimer = false;
				pressedTimer = 0;
				isPressed[0] = false;
				isPressed[1] = false;
				explosion[0].SetActive(false);
				explosion[1].SetActive(false);
				RandomButtonNumber();
			}
		}
		if(!isPressed[0])//P1 movement
		{
			if(Input.GetAxis("Vertical") > 0) //up
			{
				if(sprP1.sprite == p1ButtonSet[0])
				{
					isPressed[0] = true;
					explosion[0].SetActive(true);
				}
			}
			else if(Input.GetAxis("Vertical") < 0) //down 
			{
				if(sprP1.sprite == p1ButtonSet[1])
				{
					isPressed[0] = true;
					explosion[0].SetActive(true);
				}
			}
			else if(Input.GetAxis("Horizontal") < 0) //left
			{
				if(sprP1.sprite == p1ButtonSet[2])
				{
					isPressed[0] = true;
					explosion[0].SetActive(true);
				}
			}
			else if(Input.GetAxis("Horizontal") > 0) //right
			{
				if(sprP1.sprite == p1ButtonSet[3])
				{
					isPressed[0] = true;
					explosion[0].SetActive(true);
				}
			}
			startPressedTimer = true;
		}

		if(!isPressed[1])//P2 attack
		{
			if(Input.GetButtonDown("Button_A"))
			{
				if(sprP2.sprite == p2ButtonSet[0])
				{
					isPressed[1] = true;
					explosion[1].SetActive(true);
				}
			}
			else if(Input.GetButtonDown("Button_B"))
			{
				if(sprP2.sprite == p2ButtonSet[1])
				{
					isPressed[1] = true;
					explosion[1].SetActive(true);
				}
			}
			else if(Input.GetButtonDown("Button_X"))
			{
				if(sprP2.sprite == p2ButtonSet[2])
				{
					isPressed[1] = true;
					explosion[1].SetActive(true);
				}
			}
			else if(Input.GetButtonDown("Button_Y"))
			{
				if(sprP2.sprite == p2ButtonSet[3])
				{
					isPressed[1] = true;
					explosion[1].SetActive(true);
				}
			}
			startPressedTimer = true;
		}

		if(isPressed[0] && isPressed[1])
		{
			correctPressed += 1;
			isPressed[0] = false;
			isPressed[1] = false;
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

    void Charging_Rocket_Fist()
    {
        SoundManagerScript.Instance.PlaySFX(AudioClipID.SFX_MECHAMAN_CHARGING_FIST_ROCKET);
    }

    void Getting_Headbutted()
    {
        SoundManagerScript.Instance.PlaySFX(AudioClipID.SFX_MECHAMAN_GETTING_HEADBUTTED);
    }

    void Getting_Swiped()
    {
        SoundManagerScript.Instance.PlaySFX(AudioClipID.SFX_MECHAMAN_GETTING_SWIPED);
    }

    void Getting_Roared()
    {
        SoundManagerScript.Instance.PlaySFX(AudioClipID.SFX_MECHAMAN_GETTING_ROARED);
    }

    void Hit_By_Rock()
    {
        SoundManagerScript.Instance.PlaySFX(AudioClipID.SFX_MECHAMAN_HIT_BY_ROCK);
    }

    void Hurt_By_Acid()
    {
        SoundManagerScript.Instance.PlaySFX(AudioClipID.SFX_MECHAMAN_HURT_BY_ACID);
    }

    void Rocket_Fist_Launched()
    {
        SoundManagerScript.Instance.PlaySFX(AudioClipID.SFX_MECHAMAN_ROCKET_FIST_LAUNCH);
    }

    void Ultimate_Mode()
    {
        SoundManagerScript.Instance.PlaySFX(AudioClipID.SFX_MECHAMAN_ULTIMATE_MODE);
    }
		
}