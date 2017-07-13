using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour 
{

	public int damage = 20;
	private float lifeTime = 1.95f;

	void Start ()
	{
		Destroy (this.gameObject, lifeTime);
	}

	void OnCollisionEnter2D (Collision2D target)
	{
		if (target.gameObject.CompareTag ("Player")) 
		{
			target.gameObject.GetComponent<Mecha> ().ReceiveDamage (damage);
			Destroy (this.gameObject);
		} 
		else if (target.gameObject.CompareTag ("Enemy") || target.gameObject.CompareTag ("Ground"))
		{
			Physics2D.IgnoreCollision (target.gameObject.GetComponent<Collider2D> (), GetComponent<Collider2D> ());
			damage = 0;
		}
	}
}