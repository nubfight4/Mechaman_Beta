using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResumeButtonScript1 : MonoBehaviour {

	[SerializeField] public GameObject PauseMenuPanel;
	public bool paused;

	public void ContinueGame()
	{
		PauseMenuPanel.SetActive(false);
		Debug.Log("Timescale ");
		Time.timeScale = 1.0f;
	}
}
