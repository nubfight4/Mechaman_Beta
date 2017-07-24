using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

[System.Serializable]

public class Goatzilla : LifeObject 
{
	public BoxCollider2D box2d;
	public Rigidbody2D rb2d;

	public float initialSpeed = 1f; // Phase 1 initial speed
	public float walkTimer = 0.0f;
	public float knockBackPower = 0.0f;

	//! Melee
	[Header("Melee")]
	public float meleeRange = 3.5f; // Melee Distance 1f = 100px
	public float meleeTimer = 0.0f;
	public float meleeDurationNormal = 1.0f;
	public float meleeDurationEnrage = 2.0f;
	public bool isCheckMelee = true;

	//! Rock
	[Header("Rock")]
	public GameObject rockPrefab;
	public GameObject indicator;
	Vector3 targetPosition;
	public float rockSpawnHeight = 20.0f;
	public int singleRockThrowCountNormal = 3;
	public int singleRockThrowCountEnrage = 5;
	int randRockChoice = -1;
	bool randRockThrow = false;
	bool isMultiRockThrow = false;
	float rockTimer = 0.0f;
	int rockCounter = 0;
	int rockThrowCounter = 0;
	public float rockDuration = 3.0f;
	public int rockThrowCounterLimit = 3;

	//! Acid
	[Header("Acid")]
	public float acidTimer = 0.0f;
	public float acidDuration = 1.0f;
	public float acidDelayTimer = 0.0f;
	public float acidDelayDuration = 2.0f;
	bool isAcidSpit = false;
	public GameObject acidProjectilePrefab;
	Vector3 spitPos;

	//! Roar
	[Header("Roar")]
	public bool isRoarReady = false;
	public bool isRoarDone = false;
	public float roarChargeTimer = 0.0f;
	public float roarChargeDuration = 8.0f;
	public float roarDamageCheck = 0.0f;
	public float roarDamageLimit = 500.0f;
	public float roarStartLimit = 300.0f;
	public GameObject RoarPrefab;
	Vector3 roarPos;

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

	//	public GameObject eyeLaserPrefab;

	private Mecha target;
	private float speed;
	//private Direction movingDirection;

	private float enrageHpThreshold = 1500.0f;
	private bool nearToTarget;

//	enum Direction
//	{
//		LEFT,
//		RIGHT,
//		NONE,
//	}

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

	public AttackState prevAttackState;
	public AttackState curAttackState;
	public BehaviorState curBehaviorState;

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
//		enrageHpThreshold = (GetHP() * lives) / 2 ; // Enrage HP
		//ReceiveDamage (1000);
		curAttackState = AttackState.CHECK;
		prevAttackState =  AttackState.CHECK;
//		anim.SetLayerWeight(1, 1);
	}

	void Boundary ()
	{

		if (transform.position.x > 4.7) {
			transform.position = new Vector3 (4.7f, transform.position.y);
		}

		if (transform.position.x < -4.7) {
			transform.position = new Vector3 (-4.7f, transform.position.y);
		}


	}

	void Update () 
	{
		Boundary ();
		if (target != null) 
		{
			if(CameraShake._instance.shakeDuration > 0)
			{
				CameraShake._instance.shakeDuration -= Time.deltaTime;
			}
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
						anim.SetTrigger("DoPrepareRoar");
						Roar();
					}
					else if (curAttackState == AttackState.ACID)
					{
						//! isAcidAnim boolean here
						if(isAcidAnim == false)
						{
							acidDelayTimer += Time.deltaTime;
							if(acidDelayTimer >= acidDelayDuration)
							{
								acidDelayTimer = 0.0f;
								isAcidAnim = true;
								anim.SetTrigger ("DoAcid");
							}
						}
					}
					else if (curAttackState == AttackState.THROWROCK) //Throw Rock
					{
						if(randRockThrow == false)
						{
							randRockThrow = true;
							randRockChoice = Random.Range(0, 2);
							anim.SetBool ("DoThrowRock", true);
							CameraShake._instance.shakeDuration = 0.2f;
							CameraShake._instance.shakeAmount = 0.1f;
						}
					}
					else if (curAttackState == AttackState.CHECK)
					{
						//! Check walk func
						Walk(3.0f);
					}
				}
				//! Enrage
				else if (curBehaviorState == BehaviorState.ENRAGE)
				{
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
						anim.SetTrigger("DoPrepareRoar");
						Roar();
					}
					else if (curAttackState == AttackState.ACID)
					{
						//! isAcidAnim boolean here
						if(isAcidAnim == false)
						{
							acidDelayTimer += Time.deltaTime;
							if(acidDelayTimer >= acidDelayDuration)
							{
								acidDelayTimer = 0.0f;
								isAcidAnim = true;
								anim.SetTrigger ("DoAcid");
							}
						}
					}
					else if (curAttackState == AttackState.THROWROCK) //throw
					{
						if(randRockThrow == false)
						{
							randRockThrow = true;
							randRockChoice = Random.Range(0, 2);
							anim.SetBool ("DoThrowRock", true);
						}
					}
					else if (curAttackState == AttackState.CHECK)
					{
						//! Check walk func
						Walk(5.0f);
					}
				}
			}
		}
	}

//	private Direction SeekTarget ()
//	{
//		Vector3 targetDir = target.transform.position - transform.position;
//		if (targetDir.x < 0)
//		{
//			return Direction.LEFT;
//		}
//		else
//			return Direction.NONE;
//	}

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
			}
			else
			{
				Destroy (gameObject, 3f);
				isAlive = false;
				SceneManager.LoadScene (4);
			}
		}
	}

	public void Transition()
	{
		anim.SetLayerWeight(1, 1);
		curBehaviorState = BehaviorState.ENRAGE;
		UpdateAttackState(AttackState.CHECK);
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
			target.ReceiveDamage(50);
			target.transform.Translate(knockBackPower,0.0f,0.0f);
			//target.Knockback (Vector3.left, 0.1f);
		}
	}

	void MultiRockThrow(int count)
	{
		//! Check if > 5 rocks switch state else to this logic
		if ( rockCounter >= count )
		{
			isCheckMelee = false;
			rockThrowCounter++;
			rockCounter = 0;
			if(rockThrowCounter >= 3)
			{
				//! go to acid
				anim.SetBool("DoThrowRock", false);
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
				targetPosition = target.transform.position + (Vector3.up * rockSpawnHeight);
				RockIndicator(targetPosition,0.83f);
				Instantiate(rockPrefab, targetPosition, Quaternion.identity);
				rockTimer = 0.0f;
			}	
		}
	}

	public void SingleRockThrow()
	{
		if (randRockChoice == 0)
		{
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
				targetPosition = target.transform.position + (Vector3.up * rockSpawnHeight) + (Vector3.right * (i - tempRockCount/2));
				RockIndicator(targetPosition,0.83f);
				Instantiate(rockPrefab, targetPosition, Quaternion.identity);
			}
			rockThrowCounter++;
			if(rockThrowCounter >= 3)
			{
				rockThrowCounter = 0;
				UpdateAttackState(AttackState.ACID);
				rockCounter = 0;
				anim.SetBool("DoThrowRock", false);
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
			isMultiRockThrow = true;
		}
	}

	void RockIndicator(Vector3 position,float yPos)
	{
		indicator.transform.position = new Vector3(position.x, yPos, position.z);
		Instantiate(indicator,indicator.transform.position,Quaternion.identity);
		//enable the indicator

		//disable it after the duration

		//spawn the rocks
	}

	public void StartSpit()
	{
		isAcidSpit = true;
		spitPos = new Vector3 (this.transform.position.x - 3.81f, -2.53f, 0.0f);
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
		isRoarReady = false;
		isRoarDone = true;
		roarPos = new Vector3 (-4.86f, 4.02f, 0.0f);
		Instantiate(RoarPrefab, this.transform.position + roarPos, Quaternion.identity);
		UpdateAttackState(AttackState.CHECK);
	}

	public void PrepareRoar()
	{
		isRoarPrepare = true;
	}

	private void Roar()
	{
		if (isRoarPrepare == true)
		{
			anim.SetBool("DoChargeRoar", true);
			roarChargeTimer += Time.deltaTime;
			if ( roarChargeTimer > roarChargeDuration)
			{
				isRoarPrepare = false;
				anim.SetBool("DoChargeRoar", false);
				anim.SetTrigger ("DoRoarAttack");
				roarChargeTimer = 0;
			}
		}
	}

	public void Headbutt ()
	{
		if(GetDistanceFromTarget () <= meleeRange)
		{
			target.ReceiveDamage(70);
			target.transform.Translate(knockBackPower,0.0f,0.0f);
			//target.Knockback (Vector3.left, 0.1f);
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
		if(!isInvincible)
		{
			anim.SetTrigger("DoLightDamage");
		}
		if(isRoarPrepare == true)
		{
			roarDamageCheck += value;
			if (roarDamageCheck >= roarDamageLimit)
			{
				isRoarPrepare = false;
				roarDamageCheck = 0.0f;
				isRoarDone = true;
				isRoarReady = true;
				UpdateAttackState(AttackState.CHECK);
			}
		}
	}

	public IEnumerator Immobolize (float duration, bool invincible)
	{
		SetSpeed (0);
		if (invincible)
			SetIsInvincible (true);
		yield return new WaitForSeconds (duration);
		SetIsInvincible (false);
	}

//	public void PlayKnockbackAnimation ()
//	{
//		anim.SetTrigger ("Damage");
//	}
//
	void UpdateAttackState(AttackState state)
	{
		if(isRoarReady && state == AttackState.CHECK)
		{
			isCheckMelee = false;
			prevAttackState = AttackState.NONE;
			curAttackState = AttackState.ROAR;
		}
		else if (isEnraged && state == AttackState.CHECK && curBehaviorState == BehaviorState.NORMAL)
		{
			isCheckMelee = false;
			StartCoroutine (Immobolize (5f, true)); //Invulnerable + Immoblize for 3s
			prevAttackState = AttackState.NONE;
			curAttackState = AttackState.TRANSITION;
		}
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
		if (HP <= roarStartLimit && !isRoarReady && isRoarDone == false)
		{
			isRoarReady = true;
		}
		else if (HP > roarStartLimit)
		{
			isRoarReady = false;
			isRoarDone = false;
		}
			
		if (!isEnraged && (GetHP () + (500 * lives) <= enrageHpThreshold)) 
		{
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

	private float GetSpeed ()
	{
		return this.speed;
	}


	public bool GetIsEnraged ()
	{
		if (GetBehaviourState () == BehaviorState.ENRAGE)
			return true;
		else
			return false;
	}

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
		
    void Goatzilla_Stomp()
    {
        SoundManagerScript.Instance.PlaySFX(AudioClipID.SFX_GOATZILLA_STOMP);
    }
    void Goatzilla_Swipe()
    {
        SoundManagerScript.Instance.PlaySFX(AudioClipID.SFX_GOATZILLA_SWIPING);
    }
    void Goatzilla_Walking()
    {
        SoundManagerScript.Instance.PlaySFX(AudioClipID.SFX_GOATZILLA_WALKING);
    }
}