using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidPrefab : MonoBehaviour 
{
	public float acidSpitDuration = 2f;
	private float acidDamage = 3f;
	public float acidSpitTimer = 0.0f ;
	bool isDamaged = false;

	private Mecha mechaman;

	void Start () 
	{
		mechaman = GameObject.FindGameObjectWithTag ("Player").GetComponent<Mecha>();
		Destroy (this.gameObject, 2.0f);
		acidDamage = 30 * acidSpitDuration; 
	}

	void Update()
	{
		if (isDamaged == true)
		{
			acidSpitTimer += Time.deltaTime;
			if(acidSpitTimer > acidSpitDuration)
			{
				acidSpitTimer = 0.0f;
				mechaman.ReceiveDamage((int)acidDamage);
			}
		}
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.gameObject.CompareTag ("Player"))
		{
			Debug.Log("Enter");
			isDamaged = true;
		}
	}

	void OnTriggerExit2D (Collider2D other)
	{
		if (other.gameObject.CompareTag ("Player"))
		{
			isDamaged = false;
		}
	}

//	void OnCollisionEnter2D (Collision2D target) 
//	{
//		if (target.gameObject.CompareTag ("Player")) 
//		{
//			target.gameObject.GetComponent<Mecha> ().ReceiveDamage (acidDamage);
//		}
//	}
}
