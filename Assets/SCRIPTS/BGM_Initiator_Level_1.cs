using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM_Initiator_Level_1 : MonoBehaviour {

	// Use this for initialization
	void Start () {
		SoundManagerScript.Instance.PlayBGM(AudioClipID.BGM_LEVEL_1);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
