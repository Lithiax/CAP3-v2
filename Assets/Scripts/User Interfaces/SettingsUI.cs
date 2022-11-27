using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
public class SettingsUI : MonoBehaviour
{
    [Header("Volume Settings")]
    public AudioMixer audioMixer;
    public TMP_Text masterText;
    public Slider masterVolumeSlider;
    public TMP_Text sfxText;
    public Slider sfxVolumeSlider;
    public TMP_Text bgmText;
    public Slider bgmVolumeSlider;

    [Header("Window Settings")]
    public TMP_Dropdown windowModeOptions;
    public TMP_Dropdown resolutionOptoins;

    private void Awake()
    {
        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            audioMixer.SetFloat("MasterVolume", PlayerPrefs.GetFloat("MasterVolume"));
        }
        else
        {
            audioMixer.SetFloat("MasterVolume", 0);
        }

        if (PlayerPrefs.HasKey("MusicVol"))
        {
            audioMixer.SetFloat("MusicVolume", PlayerPrefs.GetFloat("MusicVolume"));
        }
        else
        {
            audioMixer.SetFloat("MusicVolume", 0);
        }
        if (PlayerPrefs.HasKey("SFX"))
        {
            audioMixer.SetFloat("SFXVolume", PlayerPrefs.GetFloat("SFXVolume"));
        }
        else
        {
            audioMixer.SetFloat("SFXVolume", 0);
        }
    }
    public void SetMasterAudio()
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(masterVolumeSlider.value) * 20);
        PlayerPrefs.SetFloat("MasterVolume", Mathf.Log10(masterVolumeSlider.value) * 20);
        int tempValue = Mathf.CeilToInt(masterVolumeSlider.value * 100f);
        masterText.text = tempValue.ToString();
    }

    public void SetSFXAudio()
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(sfxVolumeSlider.value) * 20);
        PlayerPrefs.SetFloat("SFXVolume", Mathf.Log10(sfxVolumeSlider.value) * 20);
        int tempValue = Mathf.CeilToInt(sfxVolumeSlider.value * 100f);
        sfxText.text = tempValue.ToString();
    }
    public void SetBGMAudio()
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(bgmVolumeSlider.value) * 20);
        PlayerPrefs.SetFloat("MusicVolume", Mathf.Log10(bgmVolumeSlider.value) * 20);
        int tempValue = Mathf.CeilToInt(bgmVolumeSlider.value * 100f);
        bgmText.text = tempValue.ToString();
    }

    public void SetWindowMode(int value)
    {
        Screen.fullScreenMode = (FullScreenMode)value;
    }

    public void SetResolution(int value)
    {
        switch (value)
        {
            case 0:
                Screen.SetResolution(1920, 1080, Screen.fullScreen);
            break;
            case 1:
                Screen.SetResolution(2560, 1440, Screen.fullScreen);
                break;
            case 2:
                Screen.SetResolution(1280, 720, Screen.fullScreen);
                break;
        }
        
    }
}
