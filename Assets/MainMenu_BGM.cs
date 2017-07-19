using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu_BGM : MonoBehaviour {

	// Use this for initialization
	void Start () {
        SoundManagerScript.Instance.PlayBGM(AudioClipID.BGM_MAIN_MENU);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
