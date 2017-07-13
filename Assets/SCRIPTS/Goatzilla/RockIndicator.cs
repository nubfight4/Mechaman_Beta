//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//
//public class RockIndicator : MonoBehaviour
//{
//
//	public GameObject rockPrefab;
//	public float lifeTime = 1.0f;
//	private float timer;
//
//	void Start () 
//	{
//		timer = 0;
//	}
//
//	void Update () 
//	{
//		if (timer >= lifeTime) 
//		{
//			Vector3 initPos = new Vector3 (transform.position.x, 5); //Throw rock from height 5 - basically the time used for rock to drop
//			Instantiate (rockPrefab, initPos, Quaternion.identity);
//			Destroy (this.gameObject);
//		}
//
//		timer += Time.deltaTime;
//	}
//}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockIndicator : MonoBehaviour 
{

	public GameObject rockPrefab;
	public float lifeTime = 1.0f;
	private float timer;

	void Start () 
	{
		timer = 0;
	}

	void Update () 
	{
		if (timer >= lifeTime) 
		{
			Vector3 initPos = new Vector3 (transform.position.x, 5);
			Instantiate (rockPrefab, initPos, Quaternion.identity);
			Destroy (this.gameObject);
		}

		timer += Time.deltaTime;
	}
}
