using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
public class SettingsUI : MonoBehaviour
{
    [Header("Settings")]
    public AudioMixer audioMixer;
    public TMP_Text masterText;
    public Slider masterVolumeSlider;
    public TMP_Text sfxText;
    public Slider sfxVolumeSlider;
    public TMP_Text bgmText;
    public Slider bgmVolumeSlider;

    public void SetMasterAudio()
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(masterVolumeSlider.value) * 20);
        masterText.text = masterVolumeSlider.value.ToString();
    }

    public void SetSFXAudio()
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(sfxVolumeSlider.value) * 20);
        sfxText.text = sfxVolumeSlider.value.ToString();
    }
    public void SetBGMAudio()
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(bgmVolumeSlider.value) * 20);
        bgmText.text = bgmVolumeSlider.value.ToString();
    }
}
