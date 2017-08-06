using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeCollider : MonoBehaviour {
	public int damage;
	int specialCharge = 20;
	public Mecha mechaScript;
	public Goatzilla target;
	public float knockBackValue = 0.0f;
	//Vector3 knockBackValue = new Vector3(0.3f,0.0f,0.0f);

	void OnTriggerEnter2D(Collider2D col)
	{
		if(col.gameObject.CompareTag("Enemy"))
		{ 
			damage = mechaScript.dMG;
			col.gameObject.GetComponent<LifeObject>().ReceiveDamage(damage);
			//col.gameObject.transform.Translate(knockBackValue);
			if(mechaScript.dashPunch) //! replace after HCI
			{
				col.transform.Translate(knockBackValue,0.0f,0.0f);
				mechaScript.currentCharge += specialCharge;
			}

			if(mechaScript.isJumping)
			{
				col.transform.Translate(knockBackValue,0.0f,0.0f);
				mechaScript.currentCharge += specialCharge;
			}
			//this.gameObject.SetActive(false);
		}

	}
}
