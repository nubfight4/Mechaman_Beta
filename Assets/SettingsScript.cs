using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScript : MonoBehaviour {

    public Slider bgmSlider;
    public Slider sfxSlider;
    public Slider brightnessSlider;

    // Use this for initialization
    void Start ()
    {
        bgmSlider.value = PlayerPrefs.GetFloat("BGM_Slider");
        SoundManagerScript.Instance.bgmAudioSource.volume = bgmSlider.value;

        sfxSlider.value = PlayerPrefs.GetFloat("SFX_Slider");
        SoundManagerScript.Instance.sfxAudioSource.volume = sfxSlider.value;

        brightnessSlider.value = PlayerPrefs.GetFloat("BRIGHTNESS_Slider");
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void BGMChange()
    {
        PlayerPrefs.SetFloat("BGM_Slider", bgmSlider.value);
    }

    public void SFXChange()
    {
        PlayerPrefs.SetFloat("SFX_Slider", sfxSlider.value);
        Debug.Log("asd");
//        SoundManagerScript.Instance.PlaySFX(AudioClipID.SFX_INPUT_SFX_Volume);
    }

    public void BrightnessChange()
    {
        PlayerPrefs.SetFloat("BRIGHTNESS_Slider", brightnessSlider.value);
    }

}
