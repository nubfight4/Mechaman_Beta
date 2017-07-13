using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeObject : MonoBehaviour 
{

	public int lives = 0;
	protected int maxHP;
	protected int HP;
	protected Vector3 objposition;
	protected bool isAlive = true;
	protected bool isInvincible = false;

	public virtual void CheckDeath ()
	{
		if (HP <= 0) 
		{
			Destroy (gameObject);
			isAlive = false;
		}
	}

	public int GetMaxHP ()
	{
		return this.maxHP;
	}

	public void SetMaxHP (int value)
	{
		this.maxHP = value;
	}

	public int GetHP ()
	{
		return this.HP;
	}

	public void SetHP (int value)
	{
		this.HP = value;
	}

	public float GetRemainingHPPercentage ()
	{
		return GetHP () * 100f / GetMaxHP ();
	}

	public virtual void ReceiveDamage (int value)
	{
		if (!isInvincible)
			this.HP -= value;

	}

	public virtual void ReceiveDamage (int value, Collider2D col) { }

	public void Knockback(Vector3 knockbackDir, float knockbackPower)
	{
		float resetTimer = 0.0f;
		float resetDuration = 1.0f;

		Vector2 v2 = new Vector2 (knockbackDir.x, knockbackDir.y);
		Rigidbody2D rb = GetComponent<Rigidbody2D> ();
		rb.velocity = v2 * knockbackPower;
	}
		

	public bool GetIsInvincible ()
	{
		return isInvincible;
	}

	public void SetIsInvincible (bool isInvincible)
	{
		this.isInvincible = isInvincible;
	}

	public IEnumerator Invincible (float duration)
	{
		SetIsInvincible (true);
		yield return new WaitForSeconds (duration);
		SetIsInvincible (false);
	}
}
