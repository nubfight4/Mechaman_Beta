using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class titleMenuScript : MonoBehaviour
{
    public Animator anim;
    bool walkAnimBool, punchAnimBool;
    public int playerNo;
        

    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
        walkAnimBool = false;
        punchAnimBool = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerNo == 1)
        {
            if (Input.GetButton("Bumper_Left_P1"))
            {
                Debug.Log("Player 1 holds the button~");
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
                Debug.Log("Player 2 holds the button~");
                punchAnimBool = true;
            }
            else
            {
                punchAnimBool = false;
            }
        }

        if (Input.GetButton("Bumper_Left_P1") && Input.GetButton("Bumper_Right_P2"))
        {
            SceneManager.LoadScene("MainMenuScene");
        }
        UpdateAnim();
    }

    void UpdateAnim()
    {
        anim.SetBool("punchAnimBool", punchAnimBool);
        anim.SetBool("WalkAnimBool", walkAnimBool);
    }
}
