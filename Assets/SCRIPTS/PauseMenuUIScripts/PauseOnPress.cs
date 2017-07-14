using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseOnPress : MonoBehaviour {

	private static PauseOnPress mInstance;

	public static PauseOnPress Instance
	{
		get
		{
			return mInstance;
		}
	}

	void Awake()
	{
		if(mInstance == null)
		{
			mInstance = this;
		}
		else if(mInstance != this)
		{
			Destroy(this.gameObject);
		}
	}

    [SerializeField] public GameObject PauseMenuPanel;
    public bool paused;

     void Start()
    {
        PauseMenuPanel.SetActive(false);
        paused = false;
    }

     void Update()
    {
//		if (Input.GetButtonDown("Pause"))
//        {
//            if (!PauseMenuPanel.activeInHierarchy)
//            {
//                PauseGame();
//            }
//            else if (PauseMenuPanel.activeInHierarchy)
//            {
//                ContinueGame();
//            }
//        }
		if (Input.GetButtonDown("Pause"))
		{
			if(!paused)
			{
				if(!PauseMenuPanel.activeInHierarchy)
				{
					PauseMenuPanel.SetActive(true);
				}
				else if(PauseMenuPanel.activeInHierarchy)
				{
					PauseMenuPanel.SetActive(false);
				}
				paused = !paused;
			}
		}
		if(paused)
		{
			PauseGame();
			Debug.Log("Timescale paused");
		}
		else{
			ContinueGame();
			Debug.Log("Timescale resumed");
		}
    }

    public void PauseGame()
    {
		paused = true;
		Time.timeScale = 0.0f;
    }

    public void ContinueGame()
    {
		paused = false;
		PauseMenuPanel.SetActive(false);
		Time.timeScale = 1.0f;
    }

	public void TogglePause()
	{
		paused = !paused;
	}
}
