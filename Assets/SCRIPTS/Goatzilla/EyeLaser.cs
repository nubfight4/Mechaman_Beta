using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeLaser : MonoBehaviour {

	public int damage = 100;
	private float radius;
	private float lifeTime = 3f;
	private float angularSpeed;
	private float pi = 3.1416f;
	private bool isDirectionFromLeft;
	private bool isTopToBottom;

	void Start ()
	{
		radius = transform.localScale.x * 8 / 2;
		Destroy (this.gameObject, lifeTime);
		angularSpeed = 50 * Time.deltaTime / lifeTime;
	}

	void Update ()
	{
		if (isTopToBottom) {
			transform.Translate (0, DegreeToRad (-angularSpeed * radius) + 0.3f * Time.deltaTime, 0);
			if (isDirectionFromLeft)
				transform.Rotate (0, 0, angularSpeed);
			else
				transform.Rotate (0, 0, -angularSpeed);
		} else {
			transform.Translate (0, DegreeToRad (angularSpeed * radius) - 0.3f * Time.deltaTime, 0);
			if (isDirectionFromLeft)
				transform.Rotate (0, 0, -angularSpeed);
			else
				transform.Rotate (0, 0, angularSpeed);
		}
	}

	public void SetIsDirectionFromLeft (bool isDirectionFromLeft)
	{
		this.isDirectionFromLeft = isDirectionFromLeft;
	}

	public void SetIsTopToBottom (bool isTopToBottom)
	{
		this.isTopToBottom = isTopToBottom;
	}

	private float DegreeToRad (float deg)
	{
		return deg * pi / 180f;
	}

	void OnCollisionEnter2D (Collision2D target)
	{
		Physics2D.IgnoreCollision (target.gameObject.GetComponent<Collider2D> (), GetComponent<Collider2D> ());
		if (target.gameObject.CompareTag ("Player"))
			target.gameObject.GetComponent<Mecha> ().ReceiveDamage (damage);
	}
}
