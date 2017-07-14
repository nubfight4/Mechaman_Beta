using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FistRocketScript : MonoBehaviour
{

	//when created, will quickly move forwards to enemy position
	//upon touching enemy, will move slowly backwards to player

	Rigidbody2D rb2d;

	public Goatzilla goatzilla;
	//compare current position with goatzilla
	public Mecha mecha;

	public float attackSpeed = 20f;
	public float returnSpeed = 2f;

	public bool touchEnemy;
	public bool returnPlayer;

	public Vector2 goatzillaPos;
	public Vector2 specialFistPos;
	public Vector2 mechaPos;

	void Start ()
	{
		rb2d = gameObject.GetComponent<Rigidbody2D> ();
		goatzilla = FindObjectOfType<Goatzilla> ();
		mecha = FindObjectOfType<Mecha> ();
	}

	void Update ()
	{
	}

	void FixedUpdate ()
	{
		UpdatePos ();
		if (goatzilla != null) {
			MoveTowardsGoatzilla ();
		}
		ReturnToPlayer ();
	}

	void UpdatePos ()
	{
		specialFistPos = this.transform.position;
		if (goatzilla != null) {
			goatzillaPos = goatzilla.transform.position;
		}
		mechaPos = mecha.transform.position;
	}

	void MoveTowardsGoatzilla () //crashes when goatzilla is killed, suggest create empty as reference
	{
		if (!touchEnemy) {
			Vector2 direction = goatzillaPos - specialFistPos; //special fist move forwards
			transform.LookAt (transform.forward + transform.position, direction);

			if (specialFistPos != goatzillaPos) {
				transform.Translate (Vector2.up * Time.deltaTime * attackSpeed); //special fist move faster when attack
			}
		}
	}

	void ReturnToPlayer ()
	{
		if (touchEnemy) {

			Vector2 direction = specialFistPos - mechaPos; //special fist move backwards
			transform.LookAt (transform.forward + transform.position, direction);

			if (specialFistPos != mechaPos) {
				transform.Translate (Vector2.down * Time.deltaTime * returnSpeed); //special fist slowly return to player
			}
		}

		if (returnPlayer) {
			Destroy (this.gameObject);
		}
	}

	public float punchDelay = 100f;
	//special fist gets temporarily stuck on enemy
	public float punchCounter;

	void OnTriggerEnter2D (Collider2D target)
	{
		if (target.gameObject.tag == "Enemy") {
			punchCounter += Time.deltaTime * 1000f;
			if (punchCounter > punchDelay) {
				touchEnemy = true;
			}
		}
		if (touchEnemy) {
			if (target.gameObject.tag == "Player") {
				returnPlayer = true;
			}
		}
	}

	void OnTriggerStay2D (Collider2D target)
	{
		if (target.gameObject.tag == "Enemy") {
			punchCounter += Time.deltaTime * 1000f;
			if (punchCounter > punchDelay) {
				touchEnemy = true;
			}
		}
		if (touchEnemy) {
			if (target.gameObject.tag == "Player") {
				returnPlayer = true;
			}
		}
	}
}
