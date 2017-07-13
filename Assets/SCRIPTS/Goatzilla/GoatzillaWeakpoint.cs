using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class GoatzillaWeakpoint : MonoBehaviour {

	public int extraMeleeDamage;
	public int extraRangeDamage;
	public bool forEnrage;
	private Goatzilla g;
	public bool tummy;
	public bool horn;

	void Start () {
		g = GameObject.FindGameObjectWithTag ("Enemy").GetComponent<Goatzilla> ();
	}

	void Update () {
		//if (!forEnrage && g.GetIsEnraged ())
		//	Destroy (this.gameObject);

/*		if (g.GetMaxHP >= 500) 
		{
			tummy = true;
			horn = false;
		} 

		else 
		{
			tummy = false;
			horn = true;
		}
	}

	void OnCollisionEnter2D (Collision2D target)
	{
		/*if (forEnrage && g.GetIsEnraged ()) {
			// Enraged.
		} else if (!forEnrage && !g.GetIsEnraged ()) {
			Physics2D.IgnoreCollision (target.gameObject.GetComponent<Collider2D> (), GetComponent<Collider2D> ());
			if (target.gameObject.CompareTag ("Bullet")) {
				g.ReceiveDamage (extraRangeDamage);
				Debug.Log ("Weakness point hit!");
			}
		}*/
	}
}
