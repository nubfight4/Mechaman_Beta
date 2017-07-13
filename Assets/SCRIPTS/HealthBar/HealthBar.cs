using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour 
{
	public LifeObject target;
	private Image bar;

	void Start ()
	{
		bar = GetComponent<Image> ();
	}

	void Update () 
	{
		if (target != null) {
			bar.fillAmount = target.GetRemainingHPPercentage () / 100;
		} else {
			bar.fillAmount = 0;
		}
	}
}
