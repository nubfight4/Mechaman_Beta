using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum AudioClipID
{
	BGM_MAIN_MENU = 0,
	BGM_TUTORIAL = 1,
	BGM_LEVEL_1 = 2,
	BGM_LOSE = 3,
	BGM_WIN = 4,

	SFX_INPUT_SFX_Volume = 100,
	SFX_HIT_HOLOGRAM = 102,
    SFX_HOLOGRAM_POP_UP = 103,
    SFX_HOVER = 104,
    SFX_GOATZILLA_CHARGE_ROAR = 105,
    SFX_GOATZILLA_ROAR = 106,
    SFX_GOATZILLA_HIT_BY_PUNCH = 107,
    SFX_GOATZILLA_HIT_BY_ROCKET_FIST = 108,
    SFX_GOATZILLA_ROCK_TELEGRAPH = 109,
    SFX_GOATZILLA_ROCK_CRASH = 110,
    SFX_GOATZILLA_STOMP = 111,
    SFX_GOATZILLA_SWIPING = 112,
    SFX_GOATZILLA_VOMIT_ACID = 113,
    SFX_GOATZILLA_WALKING = 114,
    SFX_MECHAMAN_CHARGING_FIST_ROCKET = 115,
    SFX_MECHAMAN_DASH_PUNCH = 116,
    SFX_MECHAMAN_GETTING_HEADBUTTED = 117,
    SFX_MECHAMAN_GETTING_SWIPED = 118,
    SFX_MECHAMAN_GETTING_ROARED = 119,
    SFX_MECHAMAN_HEAVY_PUNCH = 120,
    SFX_MECHAMAN_NORMAL_PUNCH = 121,
    SFX_MECHAMAN_HIT_BY_ROCK = 122,
    SFX_MECHAMAN_HURT_BY_ACID = 123,
    SFX_MECHAMAN_ROCKET_FIST_LAUNCH = 124,
    SFX_MECHAMAN_ULTIMATE_MODE = 125,
    SFX_MECHAMAN_WALKING = 126,


    TOTAL = 9001
}

[System.Serializable]
public class AudioClipInfo
{
	public AudioClipID audioClipID;
	public AudioClip audioClip;
}

public class SoundManagerScript : MonoBehaviour 
{
	private static SoundManagerScript mInstance;

	public static SoundManagerScript Instance
	{
		get
		{
			if(mInstance == null)
			{
				GameObject tempObject = GameObject.FindWithTag("SoundManager");

				if(tempObject == null)
				{
					tempObject = Instantiate(PrefabManagerScript.Instance.soundManagerPrefab, Vector3.zero, Quaternion.identity);
				}
				mInstance = tempObject.GetComponent<SoundManagerScript>();
				DontDestroyOnLoad(mInstance.gameObject);
			}
			return mInstance;
		}
	}
	public static bool CheckInstanceExist()
	{
		return mInstance;
	}

	public float bgmVolume = 1.0f;
	public float sfxVolume = 1.0f;


	public List<AudioClipInfo> audioClipInfoList = new List<AudioClipInfo>();

	public AudioSource bgmAudioSource;
	public AudioSource sfxAudioSource;

	public List<AudioSource> sfxAudioSourceList = new List<AudioSource>();
	public List<AudioSource> bgmAudioSourceList = new List<AudioSource>();

	// Preload before any Start() runs in other scripts
	void Awake () 
	{
		if(SoundManagerScript.CheckInstanceExist())
		{
			Destroy(this.gameObject);
		}

		AudioSource[] audioSourceList = this.GetComponentsInChildren<AudioSource>();

		if(audioSourceList[0].gameObject.name == "BGMAudioSource")
		{
			bgmAudioSource = audioSourceList[0];
			sfxAudioSource = audioSourceList[1];
		}
		else 
		{
			bgmAudioSource = audioSourceList[1];
			sfxAudioSource = audioSourceList[0];
		}
	}

	// Use this for initialization
	void Start () 
	{

	}

	// Update is called once per frame
	void Update () 
	{
        bgmVolume = PlayerPrefs.GetFloat("BGM_Slider");
        sfxVolume = PlayerPrefs.GetFloat("SFX_Slider");
    }

	AudioClip FindAudioClip(AudioClipID audioClipID)
	{
		for(int i=0; i<audioClipInfoList.Count; i++)
		{
			if(audioClipInfoList[i].audioClipID == audioClipID)
			{
				return audioClipInfoList[i].audioClip;
			}
		}

		Debug.LogError("Cannot Find Audio Clip : " + audioClipID);

		return null;
	}

	//! BACKGROUND MUSIC (BGM)
	public void PlayBGM(AudioClipID audioClipID)
    {   
        bgmAudioSource.clip = FindAudioClip(audioClipID);
		bgmAudioSource.volume = bgmVolume;
		bgmAudioSource.loop = true;
		bgmAudioSource.Play();
	}

	public void PauseBGM()
	{
		if(bgmAudioSource.isPlaying)
		{
			bgmAudioSource.Pause();
		}
	}

	public void StopBGM()
	{
		if(bgmAudioSource.isPlaying)
		{
			bgmAudioSource.Stop();
		}
	}


	//! SOUND EFFECTS (SFX)
	public void PlaySFX(AudioClipID audioClipID)
	{
		sfxAudioSource.PlayOneShot(FindAudioClip(audioClipID), sfxVolume);
	}

	public void PlayLoopingSFX(AudioClipID audioClipID)
	{
		AudioClip clipToPlay = FindAudioClip(audioClipID);

		for(int i=0; i<sfxAudioSourceList.Count; i++)
		{
			if(sfxAudioSourceList[i].clip == clipToPlay)
			{
				if(sfxAudioSourceList[i].isPlaying)
				{
					return;
				}

				sfxAudioSourceList[i].volume = sfxVolume;
				sfxAudioSourceList[i].Play();
				return;
			}
		}

		AudioSource newInstance = gameObject.AddComponent<AudioSource>();
		newInstance.clip = clipToPlay;
		newInstance.volume = sfxVolume;
		newInstance.loop = true;
		newInstance.Play();
		sfxAudioSourceList.Add(newInstance);
	}

	public void PauseLoopingSFX(AudioClipID audioClipID)
	{
		AudioClip clipToPause = FindAudioClip(audioClipID);

		for(int i=0; i<sfxAudioSourceList.Count; i++)
		{
			if(sfxAudioSourceList[i].clip == clipToPause)
			{
				sfxAudioSourceList[i].Pause();
				return;
			}
		}
	}	

	public void StopLoopingSFX(AudioClipID audioClipID)
	{
		AudioClip clipToStop = FindAudioClip(audioClipID);

		for(int i=0; i<sfxAudioSourceList.Count; i++)
		{
			if(sfxAudioSourceList[i].clip == clipToStop)
			{
				sfxAudioSourceList[i].Stop();
				return;
			}
		}
	}

	public void ChangePitchLoopingSFX(AudioClipID audioClipID, float value)
	{
		AudioClip clipToStop = FindAudioClip(audioClipID);

		for(int i=0; i<sfxAudioSourceList.Count; i++)
		{
			if(sfxAudioSourceList[i].clip == clipToStop)
			{
				sfxAudioSourceList[i].pitch = value;
				return;
			}
		}
	}

	public void SetBGMVolume(float value)
	{
		bgmVolume = value;
	}

	public void SetSFXVolume(float value)
	{
		sfxVolume = value;
	}
}