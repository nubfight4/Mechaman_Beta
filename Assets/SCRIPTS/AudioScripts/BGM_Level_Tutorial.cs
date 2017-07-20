using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM_Level_Tutorial : MonoBehaviour {
    
	// Use this for initialization
	void Start () {
        SoundManagerScript.Instance.PlayBGM(AudioClipID.BGM_TUTORIAL);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
