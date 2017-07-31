using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour 
{
	public int rockDamage = 20;
	//private float lifeTime = 1.95f;
	private Animator anim;
	public bool isRockBreak = false;
	float rotateAmount = 0.0f;

	void Start ()
	{
		anim = GetComponent<Animator> ();
		float direction = 0.0f;
		if (Random.Range(0, 2) == 0)
		{
			direction = -1.0f;
		}
		else
		{
			direction = 1.0f;
		}
		rotateAmount = Random.Range(180.0f, 360.0f) * direction;
	}

	void Update ()
	{
		if(!isRockBreak) 
		{
			transform.Rotate (0, 0, rotateAmount * Time.deltaTime);
			if (transform.position.y < -2.9f)
			{
				GetComponent<Rigidbody2D>().velocity = Vector2.zero;
				GetComponent<Rigidbody2D>().gravityScale = 0.0f;
				transform.rotation = Quaternion.Euler(Vector3.zero);
				isRockBreak = true;
				anim.SetTrigger("DoRockBreak");
				rockDamage = 0;
			}
		}
	}

	public void DestroyRock()
	{
		Destroy(this.gameObject);
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		if(!isRockBreak)
		{
			if (other.gameObject.CompareTag ("Player")) 
			{
				GetComponent<Rigidbody2D>().velocity = Vector2.zero;
				GetComponent<Rigidbody2D>().gravityScale = 0.0f;
				transform.rotation = Quaternion.Euler(Vector3.zero);
				rotateAmount = 0.0f;
				other.gameObject.GetComponent<Mecha> ().ReceiveDamage (rockDamage);
				isRockBreak = true;
				anim.SetTrigger("DoRockBreak");
				//Destroy (this.gameObject);
			} 
			else if (other.gameObject.CompareTag ("Enemy"))
			{
				Physics2D.IgnoreCollision (other.gameObject.GetComponent<Collider2D> (), GetComponent<Collider2D> ());
				rockDamage = 0;
			}
		}
	}
}