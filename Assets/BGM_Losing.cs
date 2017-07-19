using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM_Losing : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
        SoundManagerScript.Instance.PlayBGM(AudioClipID.BGM_LOSE);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
