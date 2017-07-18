﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TextManager : MonoBehaviour
{
    bool levelTutorial = true;
    [SerializeField]
    bool textScroll = true;

    //Tutorial Level Bool
	bool tutorNormalAtk;
	bool tutorHeavyAtk;
	bool tutorComboAtk;
	bool tutorSpecialAtk;
	bool tutorSyncAtk;

	//Other Bools
	int mechaTimePressedNormal;
	public Vector3 gamepadPos;
	bool startReset;
	float resetTimer;
	float resetDuration;
	public int doorspeed;
   
   // int threeWayCombo = 0;

    public GameObject textBox;

    public Text theText;

    public TextAsset textFile;
    public string[] textLine;

    public int currentLine;
    public int endAtLine;


    // Use this for initialization
    void Start()
    {
		
		tutorNormalAtk = false;
		tutorHeavyAtk = false;
		tutorComboAtk = false;
		tutorSpecialAtk = false;
		tutorSyncAtk = false;

		startReset = false;
		mechaTimePressedNormal = 0;
		resetTimer = 0f;
		resetDuration = 0.5f;

        if (textFile != null)
        {
            textLine = (textFile.text.Split('\n'));
        }

        if (endAtLine == 0)
        {
            endAtLine = textLine.Length - 1;
        }

        if (levelTutorial == true)
        {
            currentLine = 0;
        }

    }

    void Update()
	{
		gamepadPos.x = Input.GetAxis ("Horizontal");
		gamepadPos.y = Input.GetAxis ("Vertical");

        theText.text = textLine[currentLine];

		if (Input.GetButtonDown ("Skip_Tutorial")) 
		{
			textScroll = true;
			currentLine = 12;
		}

		if (textScroll == true) {
			if (Input.GetButtonDown ("Enter")) {
				currentLine += 1;
			} 
			GameObject.FindGameObjectWithTag ("Button_B").GetComponent<SpriteRenderer> ().enabled = true;	
		} 
		else 
		{
			GameObject.FindGameObjectWithTag ("Button_B").GetComponent<SpriteRenderer> ().enabled = false;
		}



        if (currentLine > endAtLine)
        {
            textBox.SetActive(false);
        }

		if(startReset)
		{
			resetTimer += Time.deltaTime;
			if(resetTimer >= resetDuration)
			{
				resetTimer = 0;
				mechaTimePressedNormal = 0;
				startReset = false;
			}
		}

        levelSelection();
        tutorial();
    }

    void levelSelection()
    {
        if (currentLine == 14) //level 1
        {
            SceneManager.LoadScene("GameScene");
            //currentLine = 20;
        }
    }

    void tutorial()
    {
		if (currentLine == 3) 
		{
			GameObject.FindGameObjectWithTag ("TutorialDummy").GetComponent<SpriteRenderer> ().enabled = true;
			GameObject.FindGameObjectWithTag ("TutorialDummyLight").GetComponent<SpriteRenderer> ().enabled = true;
		}

        if (currentLine == 5) //Movement
        {
            textScroll = false;

			if (gamepadPos.x > 0.01 || gamepadPos.x < -0.01) 
			{
					textScroll = true;
					currentLine += 1;
			}

        }

        if (currentLine == 8) //Attack n Block
        {
            textScroll = false;

			if (Input.GetButtonDown("Normal Attack"))
			{ 
				tutorNormalAtk = true;
			}
				
			if (Input.GetButtonDown("Heavy Attack"))
			{
				tutorHeavyAtk = true;
			}
				

			if (tutorNormalAtk == true && tutorHeavyAtk == true)
			{
				textScroll = true;
				currentLine += 1;
			}


        }

        if (currentLine == 9) //Combo Attack
        {
            textScroll = false;


			if (Input.GetButtonDown("Normal Attack"))
			{ 
				mechaTimePressedNormal++;
			}


			if (mechaTimePressedNormal == 3) 
			{
				tutorComboAtk = true;
			}

			if (tutorComboAtk == true)
			{
				textScroll = true;
				currentLine += 1;
			}


        }

        if (currentLine == 11) //Sync Attack
        {
            textScroll = false;

			if(gamepadPos.y > 0.01)
			{
				if(Input.GetButtonDown("Normal Attack"))
				{
					tutorSyncAtk = true;
				}
			}

			if (tutorSyncAtk == true)
			{
				textScroll = true;
				currentLine += 1;
			}


        }

		if (currentLine == 13) 
		{
			GameObject.FindGameObjectWithTag ("TutorialDoor").GetComponent<Transform> ().Translate (Vector3.up * Time.deltaTime * doorspeed);
		}



    }
}


