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
        if (PlayerPrefs.HasKey("MasterVol"))
        {
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(PlayerPrefs.GetFloat("MasterVol")) * 100f);
            masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVol");
        }
        else
        {
            PlayerPrefs.SetFloat("MasterVol", 1);
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(1)*100f);
            masterVolumeSlider.value = 1;
        }
        int tempValue = Mathf.CeilToInt(masterVolumeSlider.value * 100f);
        masterText.text = tempValue.ToString();

        if (PlayerPrefs.HasKey("MusicVol"))
        {
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(PlayerPrefs.GetFloat("MusicVol")) * 100f);
            bgmVolumeSlider.value = PlayerPrefs.GetFloat("MusicVol");
        }
        else
        {
            PlayerPrefs.SetFloat("MusicVolume", 1);
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(1) * 100f);
            bgmVolumeSlider.value = 1;
        }
        tempValue = Mathf.CeilToInt(bgmVolumeSlider.value * 100f);
        bgmText.text = tempValue.ToString();

        if (PlayerPrefs.HasKey("SFXVol"))
        {
            audioMixer.SetFloat("SFXVolume", Mathf.Log10(PlayerPrefs.GetFloat("SFXVol")) * 100f);
            sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVol");
        }
        else
        {
            PlayerPrefs.SetFloat("SFXVolume", 1);
            audioMixer.SetFloat("SFXVolume", Mathf.Log10(1) * 100f);
            sfxVolumeSlider.value = 1;
        }

        tempValue = Mathf.CeilToInt(sfxVolumeSlider.value * 100f);
        sfxText.text = tempValue.ToString();
    }
    public void SetMasterAudio()
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(masterVolumeSlider.value) * 100f);
        PlayerPrefs.SetFloat("MasterVol", masterVolumeSlider.value);
        int tempValue = Mathf.CeilToInt(masterVolumeSlider.value * 100f);
        masterText.text = tempValue.ToString();
    }

    public void SetSFXAudio()
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(sfxVolumeSlider.value) * 100f);
        PlayerPrefs.SetFloat("SFXVol", sfxVolumeSlider.value);
        int tempValue = Mathf.CeilToInt(sfxVolumeSlider.value * 100f);
        sfxText.text = tempValue.ToString();
    }
    public void SetBGMAudio()
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(bgmVolumeSlider.value) * 100f);
        PlayerPrefs.SetFloat("MusicVol", bgmVolumeSlider.value);
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
