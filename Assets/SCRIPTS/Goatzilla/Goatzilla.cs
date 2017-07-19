using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

[System.Serializable]

public class Goatzilla : LifeObject
{
	public BoxCollider2D box2d;
	public Rigidbody2D rb2d;

	public float initialSpeed = 1f;
	// Phase 1 initial speed
	public float walkTimer = 0.0f;

	//! Melee
	public float meleeRange = 3.5f;
	// Melee Distance 1f = 100px
	public float meleeTimer = 0.0f;
	public float meleeDuration = 1.0f;

	//! Rock
	public float rockTimer = 0.0f;
	public float rockDuration = 3.0f;
	public float rockSpawnHeight = 20.0f;
	private int rockCounter = 0;
	public int rockThrowCounter = 0;
	bool randRockThrow = false;
	int randRockChoice = -1;
	bool isThrowRock1 = false;
	public GameObject rockPrefab;
	//public GameObject rockIndicatorPrefab;

	//! Acid
	public float acidTimer = 0.0f;
	public float acidDuration = 1.0f;
	public float delayForAcidTimer = 0.0f;
	public float delayForAcidDuration = 2.0f;
	bool isAcidSpit = false;
	public GameObject acidProjectilePrefab;
	Vector3 spitPos;

	//! Roar
	public float roarChargeTimer = 0.0f;
	public float roarChargeDuration = 8.0f;
	private float roarDamageLimit = 500.0f;
	public GameObject RoarPrefab;
	Vector3 roarPos;

	//	public GameObject eyeLaserPrefab;

	private Mecha target;
	private float speed;
	private Direction movingDirection;
	public bool isEnraged;
	private int enrageHpThreshold;
	private bool nearToTarget;

	private Animator anim;
	public bool isWalkAnim = false;
	public bool isSwipeAnim = false;
	public bool isAcidAnim = false;
	public bool isRoarAnim = false;
	public bool isRoarPrepare = false;
	public bool isRoarCharge = false;
	public bool isRoarAttack = false;
	public bool isHeadbuttAnim = false;
	public bool isThrowRockAnim = false;
	public bool isEnrageAnim = false;

	bool isRock = false;

	Mecha mecha;

	enum Direction
	{
		LEFT,
		//		RIGHT,
		NONE,
	}

	public enum AttackState
	{
		SWIPE,
		HEADBUTT,
		THROWROCK,
		ACID,
		ROAR,
		NONE,
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
		mecha = FindObjectOfType<Mecha> ();
		target = GameObject.FindGameObjectWithTag ("Player").GetComponent<Mecha> ();
		anim = GetComponent<Animator> ();
		SetSpeed (GetInitialSpeed ());
		isEnraged = false;
		enrageHpThreshold = (GetHP () * lives) / 2; // Enrage HP
		//ReceiveDamage (1000);
		curAttackState = AttackState.NONE;
		prevAttackState = AttackState.NONE;
	}

	void Update ()
	{
		if (target != null) {
			//! new code!
			CheckDeath ();
			if (isAlive) {
				//! Normal
				UpdateMonsterCondition ();
				if (!mecha.isMinigame) {
					if (curBehaviorState == BehaviorState.NORMAL) { 				
						if (curAttackState == AttackState.NONE) {
							if (isSwipeAnim == false || isThrowRockAnim == false || isAcidAnim == false) {
								Walk (1.5f);
							}
						} else if (curAttackState == AttackState.THROWROCK) {
							if (randRockThrow == false) {
								randRockThrow = true;
								randRockChoice = Random.Range (0, 2);
							}
//						if(isThrowRockAnim == false)
//						{
//							isThrowRockAnim = true;
//						}
							if (isThrowRock1 == true) {
								//! Check if > 5 rocks switch state else to this logic
								if (rockCounter >= 5) {
									rockThrowCounter++;
									Debug.Log ("HEllo? : " + rockThrowCounter);
									rockCounter = 0;
									if (rockThrowCounter >= 3) {
										anim.SetBool ("DoRockThrow", false);
										rockThrowCounter = 0;
										UpdateAttackState (AttackState.ACID);
										isThrowRock1 = false;
									} else {
										anim.SetBool ("DoRockThrow", false);
										UpdateAttackState (AttackState.NONE);
										isThrowRock1 = false;
									}
								} else {
									rockTimer += Time.deltaTime;
									if (rockTimer >= rockDuration) {
										rockCounter++;
										Instantiate (rockPrefab, target.transform.position + (Vector3.up * rockSpawnHeight), Quaternion.identity);
										rockTimer = 0.0f;
									}	
								}
							}
						} else if (curAttackState == AttackState.ACID) {
							if (isAcidSpit == true) {
								Acid ();
							} else {
								if (isAcidAnim == false) {
									isAcidAnim = true;
									anim.SetTrigger ("DoAcid");
								}
							}
						} 
//					else if (curAttackState == AttackState.SWIPE && isRock == false) 
//					{
//					} 
					else if (curAttackState == AttackState.ROAR) {
							if (isRoarAnim == false) {
								isRoarAnim = true;
								anim.SetBool ("DoWalk", false);
								anim.SetTrigger ("DoPrepareRoar");
							}
							Roar ();
							//						UpdateAttackState(AttackState.NONE);
						}
					}
				}

				//! Enrage
				else if (curBehaviorState == BehaviorState.ENRAGE) {
					if (curAttackState == AttackState.NONE) {
						Walk (5.0f);
					} else if (curAttackState == AttackState.THROWROCK) {
						ThrowRock ();
					} else if (curAttackState == AttackState.ACID) {
						Acid ();
					} else if (curAttackState == AttackState.HEADBUTT) {
						Headbutt ();
					} else if (curAttackState == AttackState.ROAR) {
						Roar ();
					}
				}

			} else {

			}
		}

		if (canHurt) {
			if (hurtDurationTimer <= hurtDuration) {
				hurtDurationTimer += Time.deltaTime * 1000f;
			} else {
				hurtDurationTimer = 0f;
				canHurt = false;
			}
		}

	}

	private Direction SeekTarget ()
	{
		Vector3 targetDir = target.transform.position - transform.position;
		if (targetDir.x < 0)
			return Direction.LEFT;
		//else if (targetDir.x > 0)
		//	return Direction.RIGHT;
		else
			return Direction.NONE;
	}

	private float GetDistanceFromTarget ()
	{
		return Vector3.Distance (transform.position, target.transform.position);
	}

	public override void CheckDeath ()
	{
		if (HP <= 0) {
			if (lives > 0) {
				lives--;
				HP = 500;
			} else {
				Destroy (gameObject, 3f);
				isAlive = false;
				SceneManager.LoadScene (4);
			}
		}
	}

	public void ResetState ()
	{
		UpdateAttackState (AttackState.NONE);
	}

	public void ResetAnim ()
	{
		isRock = false;
		meleeTimer = 0.0f;
		isSwipeAnim = false;
		isAcidAnim = false;
		isRoarAnim = false;
		isRoarPrepare = false;
		isRoarCharge = false;
		isRoarAttack = false;
		isHeadbuttAnim = false;	
		isThrowRockAnim = false;
		isEnrageAnim = false;
	}

	private void Walk (float walkDuration)
	{
		randRockThrow = false;
		if (walkTimer >= walkDuration) {
			walkTimer = 0.0f;
			meleeTimer = 0.0f;
			anim.SetBool ("DoWalk", false);
			//!	KEVIN change to center of level
			if (this.transform.position.x < 1.8f) {
				UpdateAttackState (AttackState.ROAR);
			} else {
				//! DEBUG, REMOVE and swithc back to throwRock
				//UpdateAttackState(AttackState.ROAR);
				anim.SetBool ("DoRockThrow", true);
				UpdateAttackState (AttackState.THROWROCK);
			}
		} else {
			if (!isSwipeAnim) {
				if (GetDistanceFromTarget () <= meleeRange) {
					meleeTimer += Time.deltaTime;
				} else {
					meleeTimer = 0.0f;
				}

				if (meleeTimer >= meleeDuration) {
					meleeTimer = 0.0f;
					isSwipeAnim = true;
					anim.SetBool ("DoWalk", false);
					anim.SetBool ("DoSwipe", true);
				} else {
					walkTimer += Time.deltaTime;
					anim.SetBool ("DoWalk", true);
					transform.Translate (Vector3.left * Time.deltaTime * speed);
				}
			}
		}
	}

	private void Swipe ()
	{
//		Debug.Log("Chek");
		//StartCoroutine (ApplyDamage (20)); // Slash dmg, Delay to see hp decrease
		if (GetDistanceFromTarget () <= meleeRange) {
			target.ReceiveDamage (20);
			target.Knockback (Vector3.left, 0.1f);
		}
		anim.SetBool ("DoSwipe", false);
	}


	private void ThrowRock ()
	{
		if (randRockChoice == 0) {
			Instantiate (rockPrefab, target.transform.position + (Vector3.up * rockSpawnHeight), Quaternion.identity);
			Instantiate (rockPrefab, target.transform.position + (Vector3.up * rockSpawnHeight) + Vector3.right, Quaternion.identity);
			Instantiate (rockPrefab, target.transform.position + (Vector3.up * rockSpawnHeight) + Vector3.left, Quaternion.identity);
			rockThrowCounter++;
			if (rockThrowCounter >= 3) {
				anim.SetBool ("DoRockThrow", false);
				rockThrowCounter = 0;
				UpdateAttackState (AttackState.ACID);
				rockCounter = 0;
			} else {
				anim.SetBool ("DoWalk", true);
				anim.SetBool ("DoRockThrow", false);
				UpdateAttackState (AttackState.NONE);
				rockCounter = 0;
			}

		} else if (randRockChoice == 1) {
			isThrowRock1 = true;
		}
	}

	public void StartSpit ()
	{
		Debug.Log ("Spit");
		isAcidSpit = true;
		spitPos = new Vector3 (this.transform.position.x - 3.81f, -2.53f, 0.0f);
		Instantiate (acidProjectilePrefab, spitPos, Quaternion.identity);
	}

	private void Acid ()
	{
		//! 0.19 left of boss
		//! 2.46 between each acid
		//Instantiate (acidProjectilePrefab, transform.position, Quaternion.identity);
		acidTimer += Time.deltaTime;
		if (acidTimer >= acidDuration) {
			spitPos += (Vector3.left * 2.46f);
			acidTimer = 0.0f;

			if (spitPos.x < -11.0f) {
				isAcidSpit = false;
				UpdateAttackState (AttackState.NONE);
			} else {
				Instantiate (acidProjectilePrefab, spitPos, Quaternion.identity);
			}
		}	

	}

	public void StartRoar ()
	{
		Debug.Log ("Roar");
		anim.SetBool ("DoWalk", false);
		isRoarAttack = true;
		roarPos = new Vector3 (-2.8f, 4.02f, 0.0f);
		Instantiate (RoarPrefab, this.transform.position + roarPos, Quaternion.identity);
		UpdateAttackState (AttackState.NONE);
	}

	public void PrepareRoar ()
	{
		isRoarPrepare = true;
	}

	private void Roar ()
	{
		/* 1) play roar charge anim && isRoarAnim1 true
		 * 2) if after 8 seconds, play roar anim (if isRoarAnim 1 true)
		 * -> if isRoarAnim1 true & player damage boss for X HP, reset to None
		 * -> create extra variable to check if player damage boss when isRoarAnim1 true (put in ReceiveDamage for Boss)
		 * 
		 * 3) at x frame in Roar anim shoot Roar Projectile
		 * 4) Roar projectile will have damage and destroy itself when hit player
		 * RESET roarDamageLimit, isRoarAnim1 
		 */

		if (isRoarPrepare == true) {
			anim.SetBool ("DoChargeRoar", true);
			roarChargeTimer += Time.deltaTime;
			if (roarChargeTimer > roarChargeDuration) {
				anim.SetBool ("DoChargeRoar", false);
				isRoarCharge = true;
				anim.SetTrigger ("DoRoarAttack");
				roarChargeTimer = 0;
			}
			anim.SetBool ("DoWalk", false);
		}
		//anim.SetTrigger ("DoRoar");
		//UpdateAttackState(AttackState.NONE);
	}

	private IEnumerator Headbutt ()
	{
		isHeadbuttAnim = true;
		//FaceTarget ();
		//for (int i = 0; i < 3; i++) 
		//{
		yield return new WaitForSeconds (1); // 1 sec headbutt once
		//	Debug.Log ("Enemy used headbutt! x" + (i + 1));
		anim.SetTrigger ("DoHeadbutt");
		//			StartCoroutine (ApplyDamage (70));
		//}
		//yield return new WaitForSeconds (2); // Rest 2s
		UpdateAttackState (AttackState.NONE);
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
		if (isRoarPrepare == true) {
			roarDamageLimit += value;
			UpdateAttackState (AttackState.NONE);
		}
		roarDamageLimit = 0.0f;
		isRoarPrepare = false;
	}

	public IEnumerator Immobolize (float duration, bool invincible)
	{
		SetSpeed (0);
		if (invincible)
			SetIsInvincible (true);
		yield return new WaitForSeconds (duration);
		SetIsInvincible (false);
	}

	public void PlayKnockbackAnimation ()
	{
		anim.SetTrigger ("Damage");
	}

	void UpdateAttackState (AttackState state)
	{
		prevAttackState = curAttackState;
		curAttackState = state;
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
		if (!isEnraged && (GetHP () * lives) <= enrageHpThreshold) {
			isEnraged = true;
			isEnrageAnim = true;
			StartCoroutine (Immobolize (3f, true)); //Invulnerable + Immoblize for 3s
			anim.SetTrigger ("DoEnrage");
			Debug.Log ("Monster is enraged!");
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

	public bool canHurt = false;
	public float hurtDuration = 500f;
	public float hurtDurationTimer = 0f;

	public int jumpPunchDamage = 170;
	public int dashPunchDamage = 170;
	public int fistRocketDamage = 250;

	//take damage
	void OnTriggerEnter2D (Collider2D target)
	{
		if (target.tag == ("JumpPunchAttack") && this.tag == ("Enemy") && !canHurt) {
			ReceiveDamage (jumpPunchDamage);
			canHurt = true;
		}
		if (target.tag == ("DashPunchAttack") && this.tag == ("Enemy") && !canHurt) {
			ReceiveDamage (dashPunchDamage);
			canHurt = true;
		}
		if (target.tag == ("FistRocketAttack") && this.tag == ("Enemy") && !canHurt) {
			ReceiveDamage (fistRocketDamage + mecha.tempBonusDamage); //include bonus damage
			mecha.tempBonusDamage = 0;
			mecha.stopMove = false;
			mecha.canSpecial = false;
			canHurt = true;
		}
	}
}