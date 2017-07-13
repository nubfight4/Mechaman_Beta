using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpenOnInput : MonoBehaviour {
    
    // Update is called once per frame
    void Update () {
		
		if(Input.GetButton("Bumper_Left_P1") && Input.GetButton("Bumper_Right_P2"))
        {
			SceneManager.LoadScene("MainMenuScene");
        }
	}
}
