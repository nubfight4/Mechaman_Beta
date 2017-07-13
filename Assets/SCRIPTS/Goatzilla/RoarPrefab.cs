using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoarPrefab : MonoBehaviour 
{
	public int roarDamage = 150;
	public float roarSpeed = 1.0f;

	void Start ()
	{
	}

	void Update()
	{
		transform.position += Vector3.left * roarSpeed * Time.deltaTime;
		if (transform.position.x <= -12.0f)
		{
			Debug.Log("Destroy?");
			Destroy(this.gameObject);
		}
			
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		Debug.Log("Enter");
		if (other.gameObject.CompareTag ("Player"))
		{
			other.gameObject.GetComponent<Mecha> ().ReceiveDamage (roarDamage);
			Destroy (this.gameObject);
		}
	}
}
