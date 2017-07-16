using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeCollider : MonoBehaviour {
	public int damage;
	public Mecha mechaScript;
	public Goatzilla target;
	Vector3 knockBackValue = new Vector3(0.3f,0.0f,0.0f);

	void OnTriggerEnter2D(Collider2D col)
	{
		if(col.gameObject.CompareTag("Enemy"))
		{ 
			damage = mechaScript.dMG;
			col.gameObject.GetComponent<LifeObject>().ReceiveDamage(damage);
			//col.gameObject.transform.Translate(knockBackValue);
		}
	}
}
