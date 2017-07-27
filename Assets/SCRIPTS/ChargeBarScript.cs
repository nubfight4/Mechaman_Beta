using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChargeBarScript : MonoBehaviour
{

	public Mecha mecha;
	private Image chargeBar;

	void Start ()
	{
		chargeBar = GetComponent<Image> ();
	}

	void Update ()
	{
		if (mecha != null) {
			chargeBar.fillAmount = mecha.GetCurrentChargePercentage () / 100;
		} else {
			chargeBar.fillAmount = 0;
		}
	}
}
