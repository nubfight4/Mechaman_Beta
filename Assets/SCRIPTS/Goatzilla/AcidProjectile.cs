using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidProjectile : MonoBehaviour 
{
	public GameObject acidPrefab;
	private Mecha target;
	private Vector3 totalDistanceFromTarget;
	private Vector3 currentDistanceFromTarget;
	private float timer;

	// Projectile
	private float speed;
	private float lifeTime = 1.5f;

	void Start () 
	{
		Destroy (this.gameObject, lifeTime);
		target = GameObject.FindGameObjectWithTag ("Player").GetComponent<Mecha>();
//		Vector3 pos = new Vector3(Range.Random(min, max), Range,Random(min, max));
		totalDistanceFromTarget = GetDistanceFromTarget ();
		timer = 0;
		speed = totalDistanceFromTarget.x / lifeTime;
	}

	void Update () 
	{
		if (timer > lifeTime) 
		{
			Instantiate (acidPrefab, transform.position, Quaternion.identity);
			Destroy (gameObject);
		}	
		currentDistanceFromTarget = GetDistanceFromTarget ();
		transform.Translate ( Vector3.up * Time.deltaTime * ArcSpeedYFactor () );
		transform.Translate ( Vector3.right * Time.deltaTime * speed );
		timer += Time.deltaTime;
	}

	private Vector3 GetDistanceFromTarget ()
	{
		Vector3 d = new Vector3 (target.transform.position.x - this.transform.position.x, target.transform.position.y - this.transform.position.y);
		return d;
	}

	private float ArcSpeedYFactor ()
	{
		float temp = currentDistanceFromTarget.x - totalDistanceFromTarget.x / 2;
		return temp / totalDistanceFromTarget.x;
	}
}
