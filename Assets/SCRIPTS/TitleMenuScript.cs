using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleMenuScript : MonoBehaviour
{
	public Animator anim;
	bool walkAnimBool, punchAnimBool;
	public int playerNo;
	public float delayTimer;
	float duration;

	// Use this for initialization
	void Start()
	{
		anim = GetComponent<Animator>();
		walkAnimBool = false;
		punchAnimBool = false;
		delayTimer = 0.0f;
		duration = 1.5f;
	}

	// Update is called once per frame
	void Update()
	{
		if (playerNo == 1)
		{
			if (Input.GetButton("Bumper_Left_P1"))
			{
				walkAnimBool = true;
			}
			else
			{
				walkAnimBool = false;
			}
		}
		if (playerNo == 2)
		{
			if (Input.GetButton("Bumper_Right_P2"))
			{
				punchAnimBool = true;
			}
			else
			{
				punchAnimBool = false;
			}
		}

		if (Input.GetButton("Bumper_Left_P1") && Input.GetButton("Bumper_Right_P2"))
		{
			delayTimer += Time.deltaTime;
			if(delayTimer > duration)
			{
				SceneManager.LoadScene("MainMenuScene");
			}
		}
		UpdateAnim();
	}

	void UpdateAnim()
	{
		anim.SetBool("punchAnimBool", punchAnimBool);
		anim.SetBool("WalkAnimBool", walkAnimBool);
	}
}
