using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

[System.Serializable]

public class Goatzilla : LifeObject 
{
	public BoxCollider2D box2d;
	public Rigidbody2D rb2d;
	private Mecha target;

	private float speed;	
	public float initialSpeed = 0.1f; // Phase 1 initial speed
	public float walkTimer = 0.0f;
	private float enrageHpThreshold = 1000.0f;
	private bool nearToTarget;
	//private Direction movingDirection;
	//public GameObject eyeLaserPrefab;
	[Header("Melee")]
	public float meleeRange = 3.5f; // Melee Distance 1f = 100px
	public float meleeTimer = 0.0f;
	public float meleeDurationNormal = 2.0f;
	public float meleeDurationEnrage = 1.0f;
	public bool isCheckMelee = true;
	public float swipeKnockbackValue = 2.0f;
	public float headButtKnockbackValue = 3.0f;

	[Header("Rock")]
	public GameObject rockPrefab;
	public GameObject rockIndicatorPrefab;
	public float indicatorHeight = 0;
	public float rockSpawnHeight = 20.0f;
	public int singleRockThrowCountNormal = 3;
	public int singleRockThrowCountEnrage = 5;
	int randRockChoice = -1;
	bool randRockThrow = false;
	bool isMultiRockThrow = false;
	int rockCounter = 0;
	int rockThrowCounter = 0;
	float rockTimer = 0.0f;
	public float rockDuration = 3.0f;
	public int rockThrowCounterLimit = 3;
	Vector3 temPos = Vector3.zero;

	[Header("Acid")]
	public float acidTimer = 0.0f;
	public float acidDuration = 1.0f;
//	public float acidDelayTimer = 0.0f;
//	public float acidDelayDuration = 2.0f;
	bool isAcidSpit = false;
	public GameObject acidProjectilePrefab;
	Vector3 spitPos;

	[Header("Roar")]
	public bool isRoarReady = false;
	public bool isRoarDone = false;
	public float roarChargeTimer = 0.0f;
	public float roarChargeDuration = 8.0f;
	public float roarDamageCheck = 0.0f;
	public float roarDamageLimit = 0.0f; 
	//public float roarStartLimit = 300.0f;
	public GameObject RoarPrefab;
	Vector3 roarPos;

	[Header("Animation")]
	private Animator anim;
	public bool isWalkAnim = false;
	public bool isSwipeAnim = false;
	public bool isAcidAnim = false;
	public bool isRoarAnim = false; 
	public bool isRoarPrepare = false;
	public bool isHeadbuttAnim = false;	
	public bool isThrowRockAnim = false;
	public bool isEnraged;
	bool isTransitionAnim = false;
	private bool isFlashes = false;

	public AttackState prevAttackState;
	public AttackState curAttackState;
	public BehaviorState curBehaviorState;

	public enum AttackState
	{
		SWIPE,
		HEADBUTT,
		THROWROCK,
		ACID,
		ROAR,
		CHECK,
		NONE,
		TRANSITION,
	}

	public enum BehaviorState
	{
		NORMAL,
		ENRAGE,
		DEATH,
	}

	void Awake ()
	{
		lives = 3;
		SetMaxHP (500); // Max HP for a bar
		SetHP (GetMaxHP ());
	}

	void Start () 
	{
		target = GameObject.FindGameObjectWithTag ("Player").GetComponent<Mecha> ();
		anim = GetComponent<Animator> ();
		SetSpeed (GetInitialSpeed ());
		isEnraged = false;
		//enrageHpThreshold = (GetHP() * lives) / 2 ; // Enrage HP
		//ReceiveDamage (1000);
		curAttackState = AttackState.CHECK;
		prevAttackState =  AttackState.CHECK;
		//anim.SetLayerWeight(1, 1);
	}

	void Boundary ()
	{
		if (transform.position.x > 4.7) 
		{
			transform.position = new Vector3 (4.7f, transform.position.y);
		}

		if (transform.position.x < -4.7) 
		{
			transform.position = new Vector3 (-4.7f, transform.position.y);
		}
	}

	void Update () 
	{
		Boundary ();
		if (target != null) 
		{
			CheckDeath();
			if(isAlive)
			{
				//! Normal
				if (curBehaviorState == BehaviorState.NORMAL) 
				{ 	
					UpdateMonsterCondition();
					//! Check headbutt 		
					//! Check swipe 	
					if(isCheckMelee)
					{
						if  (GetDistanceFromTarget() <= meleeRange)
						{
							meleeTimer += Time.deltaTime;
						}
						else
						{
							meleeTimer = 0.0f;
						}

						if (meleeTimer >= meleeDurationNormal)
						{
							meleeTimer = 0.0f;
							anim.SetTrigger("DoSwipe");
							isCheckMelee = false;
						}
					}

					if(curAttackState == AttackState.TRANSITION)
					{
						if(!isTransitionAnim)
						{
							isTransitionAnim = true;
							anim.SetTrigger ("DoEnrage");
						}
					}
					else if(curAttackState == AttackState.NONE)
					{
						//! idle anim
						//! actions after finishing an attack and before check state
						if(isMultiRockThrow == true && prevAttackState == AttackState.THROWROCK)
						{
							MultiRockThrow(5);
						}

						else if(isAcidSpit == true && prevAttackState == AttackState.ACID)
						{
							Acid ();
						}
					}
					else if (curAttackState == AttackState.ROAR)
					{
						if(!isRoarAnim)
						{
							isRoarAnim = true;
							anim.SetTrigger("DoPrepareRoar");
						}
						Roar();
					}
					else if (curAttackState == AttackState.ACID)
					{
						//! isAcidAnim boolean here
						if(isAcidAnim == false)
						{
							//acidDelayTimer += Time.deltaTime;
							//if(acidDelayTimer >= acidDelayDuration)
							//{
							//	acidDelayTimer = 0.0f;
								isAcidAnim = true;
								anim.SetTrigger ("DoAcid");
							//}
						}
					}
					else if (curAttackState == AttackState.THROWROCK)
					{
						if(randRockThrow == false)
						{
							randRockThrow = true;
							randRockChoice = Random.Range(0, 2);
							anim.SetBool("DoThrowRock", true);
						}
					}
					else if (curAttackState == AttackState.CHECK)
					{
						//! Check walk func
						if(isRoarAnim)
							isRoarAnim = false;
						anim.SetTrigger("DoWalk");
						Walk(3.0f);
					}
				}
				//! Enrage
				else if (curBehaviorState == BehaviorState.ENRAGE)
				{
					UpdateMonsterCondition();
					if(isCheckMelee)
					{
						if  (GetDistanceFromTarget() <= meleeRange)
						{
							meleeTimer += Time.deltaTime;
						}
						else
						{
							meleeTimer = 0.0f;
						}

						if (meleeTimer >= meleeDurationEnrage)
						{
							meleeTimer = 0.0f;
							anim.SetTrigger("DoSwipe");
							isCheckMelee = false;
						}
					}
					if(curAttackState == AttackState.NONE)
					{
						//! idle anim
						//! actions after finishing an attack and before check state
						if(isMultiRockThrow == true && prevAttackState == AttackState.THROWROCK)
						{
							MultiRockThrow(7);
						}

						else if(isAcidSpit == true && prevAttackState == AttackState.ACID)
						{
							Acid ();
						}
					}
					else if (curAttackState == AttackState.ROAR)
					{
						if(!isRoarAnim)
						{
							isRoarAnim = true;
							anim.SetTrigger("DoPrepareRoar");
						}
						Roar();
					}
					else if (curAttackState == AttackState.ACID)
					{
						//! isAcidAnim boolean here
						if(isAcidAnim == false)
						{
							//acidDelayTimer += Time.deltaTime;
							//if(acidDelayTimer >= acidDelayDuration)
							//{
							//	acidDelayTimer = 0.0f;
								isAcidAnim = true;
								anim.SetTrigger ("DoAcid");
							//}
						}
					}
					else if (curAttackState == AttackState.THROWROCK)
					{
						if(randRockThrow == false)
						{
							randRockThrow = true;
							randRockChoice = Random.Range(0, 2);
							anim.SetBool("DoThrowRock", true);
						}
					}
					else if (curAttackState == AttackState.CHECK)
					{
						//! Check walk func
						if(isRoarAnim)
							isRoarAnim = false;
						anim.SetTrigger("DoWalk");
						Walk(3.0f);
					}
				}
			}
		}
	}

	public void GOnHitSpark()
	{
		GameObject.FindGameObjectWithTag ("PlayerHitSpark").GetComponent<SpriteRenderer> ().enabled = true;
	}

	public void GOffHitSpark()
	{
		GameObject.FindGameObjectWithTag ("PlayerHitSpark").GetComponent<SpriteRenderer> ().enabled = false;
	}

	private float GetDistanceFromTarget ()
	{
		return Vector3.Distance (transform.position, target.transform.position);
	}

	public override void CheckDeath ()
	{
		if (HP <= 0) 
		{
			if(lives > 0)
			{
				lives--;
				HP = 500;
				isRoarDone = false;
				isRoarReady = true;
				roarDamageCheck = 0.0f;
//				UpdateAttackState(AttackState.CHECK);
			}
			else
			{	
				GetComponent<PolygonCollider2D>().enabled = false;
				isInvincible = true;
				anim.SetTrigger("DoDeath");// Death anim trigger named DoEnrage
				isAlive = false;
			}
		}
	}

	public void LoadAfterDeathAnim()
	{
		//Debug.Log("Next scene");
		SceneManager.LoadScene (4);
		Destroy (this.gameObject);
	}

	public void Transition()
	{
		anim.SetLayerWeight(1, 1);
		curBehaviorState = BehaviorState.ENRAGE;
		UpdateAttackState(AttackState.CHECK);
		randRockThrow = false;
	}

	public void ResetAnim()
	{
		//! Rock Reset
		if(curAttackState == AttackState.THROWROCK)
		{
			if (isMultiRockThrow == false)
			{
				anim.SetBool("DoThrowRock", false);
				UpdateAttackState(AttackState.CHECK);
			}
			else
			{
				UpdateAttackState(AttackState.NONE);
			}
		}
		else if (curAttackState == AttackState.ACID)
		{
			//! Acid Reset
			if (isAcidSpit == true)
			{
				UpdateAttackState(AttackState.NONE);
			}
		}
		isRoarAnim = false;
	}

	private void Walk (float walkDuration)
	{
		randRockThrow = false;
		isCheckMelee = true;
		if (walkTimer >= walkDuration)
		{
			isCheckMelee = false;
			walkTimer = 0.0f;
			UpdateAttackState(AttackState.THROWROCK);
		}
		else
		{
			walkTimer += Time.deltaTime;
			transform.Translate (Vector3.left * Time.deltaTime * speed);
		}
	}

	private void Swipe ()
	{
		if(GetDistanceFromTarget () <= meleeRange)
		{
			target.ReceiveDamage(100);
			//target.Knockback (Vector3.left, 2f);
			target.transform.Translate(-swipeKnockbackValue,0.0f,0.0f);
		}
	}

	void MultiRockThrow(int count) // Drop one after another
	{
		isCheckMelee = true;
		//! Check if > 5 rocks switch state else to this logic
		if ( rockCounter >= count )
		{
			isCheckMelee = false;
			rockThrowCounter++;
			rockCounter = 0;
			anim.SetBool("DoWait", false);
			if(!isEnraged && rockThrowCounter >= 3)
			{
				//! go to acid
//				anim.SetBool("DoThrowRock", false);
				rockThrowCounter = 0;
				isMultiRockThrow = false;
				UpdateAttackState(AttackState.ACID);
			}
			else if(isEnraged && rockThrowCounter >= 2)
			{
				//! go to acid
//				anim.SetBool("DoThrowRock", false);
				rockThrowCounter = 0;
				isMultiRockThrow = false;
				UpdateAttackState(AttackState.ACID);
			}
			else
			{
				anim.SetBool("DoThrowRock", false);
				isMultiRockThrow = false;
				UpdateAttackState(AttackState.CHECK);
			}
		}
		else
		{
			isCheckMelee = true;
			rockTimer += Time.deltaTime;
			if(rockTimer >= rockDuration)
			{
				rockCounter ++;
				temPos = target.transform.position;
				Instantiate(rockIndicatorPrefab,temPos + (Vector3.up * indicatorHeight), Quaternion.identity);
				Instantiate(rockPrefab, temPos + (Vector3.up * rockSpawnHeight), Quaternion.identity);
				rockTimer = 0.0f;
			}	
		}
	}

	public void SingleRockThrow() // Drop all together
	{
		if (randRockChoice == 0)
		{
			anim.SetBool("DoWait", false);
			int tempRockCount = 0;
			if(isEnraged)
			{
				tempRockCount = singleRockThrowCountEnrage;
			}
			else
			{
				tempRockCount = singleRockThrowCountNormal;
			}
			for( int i = 0; i < tempRockCount; i++)
			{
				temPos = target.transform.position;
				Instantiate(rockIndicatorPrefab,temPos + (Vector3.up * indicatorHeight) + (Vector3.right * (i - tempRockCount/2)), Quaternion.identity);
				Instantiate(rockPrefab, temPos + (Vector3.up * rockSpawnHeight) + (Vector3.right * (i - tempRockCount/2)), Quaternion.identity);
			}
			rockThrowCounter++;
			if(!isEnraged && rockThrowCounter >= 3)
			{
				rockThrowCounter = 0;
				UpdateAttackState(AttackState.ACID);
				rockCounter = 0;
//				anim.SetBool("DoThrowRock", false);
			}
			else if(isEnraged && rockThrowCounter >=2)
			{
				rockThrowCounter = 0;
				UpdateAttackState(AttackState.ACID);
				rockCounter = 0;
//				anim.SetBool("DoThrowRock", false);
			}
			else
			{
				UpdateAttackState(AttackState.CHECK);
				rockCounter = 0;
				anim.SetBool("DoThrowRock", false);
			}
		}
		else if (randRockChoice == 1)
		{
			anim.SetBool("DoWait", true);
			isMultiRockThrow = true;
		}
	}

	public void StartSpit()
	{
		isAcidSpit = true;
		spitPos = new Vector3 (this.transform.position.x - 3.81f, -2.53f, 0.0f);
		anim.SetBool("DoWait", true);
		Instantiate(acidProjectilePrefab, spitPos, Quaternion.identity);
	}

	private void Acid ()
	{
		//! 0.19 left of boss
		//! 2.46 between each acid
		//Instantiate (acidProjectilePrefab, transform.position, Quaternion.identity);
		acidTimer += Time.deltaTime;
		if(acidTimer >= acidDuration)
		{
			spitPos +=  (Vector3.left * 2.46f);
			acidTimer = 0.0f;

			if(spitPos.x < -11.0f)
			{
				isAcidSpit = false;
				isAcidAnim = false;
				anim.SetBool("DoThrowRock", false);
				anim.SetBool("DoWait", false);
				UpdateAttackState(AttackState.CHECK);
			}
			else
			{
				Instantiate(acidProjectilePrefab, spitPos, Quaternion.identity);
			}
		}	
	}

	public void StartRoar()
	{
		anim.SetBool("DoChargeRoar", false);
		isRoarReady = false;
		isRoarDone = true;
		roarPos = new Vector3 (-3.41f, 4.02f, 0.0f);
		Instantiate(RoarPrefab, this.transform.position + roarPos, Quaternion.identity);
		UpdateAttackState(AttackState.CHECK);
	}

	public void PrepareRoar()
	{
		if(!isRoarDone)
		{
			isRoarPrepare = true;
		}
	}

	private void Roar()
	{
		if (isRoarPrepare == true)
		{
			anim.SetBool("DoChargeRoar", true);
			roarChargeTimer += Time.deltaTime;
			if ( roarChargeTimer > roarChargeDuration)
			{
				roarDamageCheck = 0.0f;
				isRoarPrepare = false;		
				anim.SetTrigger ("DoRoarAttack");
				StartCoroutine (Immobolize (1.5f, true));
				roarChargeTimer = 0;
				//isRoarAnim = false;
				isRoarDone = true;
				isRoarReady = false;
			}
		}
	}

	public void Headbutt ()
	{
		if(GetDistanceFromTarget () <= meleeRange)
		{
			target.ReceiveDamage(120);
			//target.Knockback (Vector3.left, 0.1f);
			target.transform.Translate(-headButtKnockbackValue,0.0f,0.0f);
		}
	}

	/*private void Laser ()
	{
		attacked = true;
		FaceTarget ();
		bool isTopToBottom = (target.transform.position.y - transform.position.y >= 0) ? true : false;
		float offsetX = (faceLeft) ? -5f : 5f, offsetY = 2.5f;
		Vector3 initPos = (isTopToBottom) ? new Vector3 (transform.position.x + offsetX, transform.position.y + offsetY) : new Vector3 (transform.position.x + offsetX, transform.position.y - offsetY - 2.5f);
		float initAngle = (faceLeft) ? -45 : 45;
		initAngle *= (isTopToBottom) ? 1 : -1;
		anim.SetBool("IsTopToBottom", isTopToBottom);
		anim.SetTrigger("Laser");
		GameObject laserEye = Instantiate (eyeLaserPrefab, initPos, Quaternion.Euler (0, 0, initAngle));
		laserEye.GetComponent<EyeLaser> ().SetIsDirectionFromLeft (faceLeft);
		laserEye.GetComponent<EyeLaser> ().SetIsTopToBottom (isTopToBottom);
		StartCoroutine (Immobolize (4, false));
	}*/

	public override void ReceiveDamage (int value)
	{
		base.ReceiveDamage (value);
		if(isRoarPrepare == true)
		{
			Debug.Log("isRoarPrepare true");
			roarDamageCheck += value;
			if (roarDamageCheck >= roarDamageLimit)
			{
				Debug.Log("Cancel Roar");
				anim.SetTrigger("DoLightDamage");
				isRoarReady = false; // Added
				isRoarAnim = false;
				isRoarDone = true;
				isRoarPrepare = false;
				UpdateAttackState(AttackState.CHECK);
				roarDamageCheck = 0.0f;

			} 
			else
			{
				if(!isFlashes)
				{
					StartCoroutine (Flashes ());
				}
			}
		}
		else if(!isInvincible)
		{
			StartCoroutine (Flashes ());

			if (curAttackState == AttackState.CHECK || curAttackState == AttackState.NONE) 
			{
				anim.SetTrigger ("DoLightDamage");
			}
		}
	}

	private IEnumerator Flashes ()
	{
		isFlashes = true;
		this.GetComponent<SpriteRenderer>().enabled = false;
		yield return new WaitForSeconds (0.05f);
		this.GetComponent<SpriteRenderer>().enabled = true;
		isFlashes = false;
	}

	public IEnumerator Immobolize (float duration, bool invincible)
	{
		float temp = GetInitialSpeed ();
		SetSpeed (0);
		if (invincible)
			SetIsInvincible (true);
		yield return new WaitForSeconds (duration);
		SetIsInvincible (false);
		SetSpeed (temp);
	}

	public void PlayKnockbackAnimation ()
	{
		anim.SetTrigger ("Damage");
	}

	void UpdateAttackState(AttackState state)
	{
		if (isEnraged && state == AttackState.CHECK && curBehaviorState == BehaviorState.NORMAL)
		{
			isCheckMelee = false;
			prevAttackState = AttackState.NONE;
			curAttackState = AttackState.TRANSITION;
			StartCoroutine (Immobolize (5.0f, true)); //Invulnerable + Immoblize for 3s
		}
		else if (isRoarReady && state == AttackState.CHECK)
		{
			//Debug.Log("PrepareROar???");
			isCheckMelee = false;
			isRoarReady = false;
			prevAttackState = AttackState.NONE;
			curAttackState = AttackState.ROAR;
		}

		// Problem with this one, it interfered the ReceiveDamage's supposed UpdateAttackState(AttackState.CHECK)

		else
		{
			prevAttackState = curAttackState;
			curAttackState = state;
		}
	}

	//	public void UpdateBehaviourState ()
	//	{
	//		if(GetBehaviourState () == BehaviorState.NORMAL)
	//			if(GetRemainingHPPercentage () <= 50)
	//				curBehaviorState = BehaviorState.ENRAGE;
	//		else if(GetBehaviourState () == BehaviorState.ENRAGE)
	//			if(GetRemainingHPPercentage () <= 0)
	//				curBehaviorState = BehaviorState.DEATH;
	//	}

	private void UpdateMonsterCondition ()
	{
//		if (HP <= roarStartLimit && !isRoarReady && !isRoarDone)
//		{
//			Debug.Log("Roar2");
//			isRoarReady = true;
//		}
//		else
//		if (HP > roarStartLimit)
//		{
//			Debug.Log("Roar3 : " + !isRoarReady + " " + isRoarDone);
//			isRoarReady = false;
//			isRoarDone = false;
//		}

//		Debug.Log(GetHP () + (500 * lives));
		if (!isEnraged && (GetHP () + (500 * lives) <= enrageHpThreshold)) 
		{
			Debug.Log("Enrage");
			if(isRoarAnim)
			{
				isRoarAnim = false;
			}
			isEnraged = true;
		}
	}

	private float GetInitialSpeed ()
	{
		return this.initialSpeed;
	}

	private void SetSpeed (float speed)
	{
		this.speed = speed;
	}

//	private float GetSpeed ()
//	{
//		return this.speed;
//	}
//
//	public bool GetIsEnraged ()
//	{
//		if (GetBehaviourState () == BehaviorState.ENRAGE)
//			return true;
//		else
//			return false;
//	}

	public BehaviorState GetBehaviourState ()
	{
		return curBehaviorState;
	}

	void OnCollisionEnter2D (Collision2D target)
	{
		//		if (target.gameObject.CompareTag ("Player") && isCharging) 
		//		{
		//			target.gameObject.GetComponent<Mecha> ().ReceiveDamage (chargeDamage);
		//		}
	}

	void StompShake()
	{
		CameraShake._instance.shakeDuration = 1.0f;
		CameraShake._instance.shakeAmount = 0.1f;
	}

	void GoatzillaWalkingSFX()
	{
		SoundManagerScript.Instance.PlaySFX(AudioClipID.SFX_GOATZILLA_WALKING);
	}

	void GoatzillaRockTelegraphSFX()
	{
		SoundManagerScript.Instance.PlaySFX(AudioClipID.SFX_GOATZILLA_ROCK_TELEGRAPH);
	}

	void GoatzillaSwipingSFX()
	{
		SoundManagerScript.Instance.PlaySFX(AudioClipID.SFX_GOATZILLA_SWIPING);
	}

	void GoatzillaChargeRoarSFX()
	{
		SoundManagerScript.Instance.PlaySFX(AudioClipID.SFX_GOATZILLA_CHARGE_ROAR);
	}

	void GoatzillaRoarSFX ()
	{
		SoundManagerScript.Instance.PlaySFX (AudioClipID.SFX_GOATZILLA_ROAR);
	}
}