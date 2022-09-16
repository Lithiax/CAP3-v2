using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using TMPro;
public class SettingsUI : MonoBehaviour
{
    [Header("Settings")]
    public AudioMixer audioMixer;
    public Slider volumeSlider;
    public TMP_Text volumeText;
    public Button enableButton;
    public Button disableButton;
    public Sprite enabledSprite;
    public Sprite disabledSprite;


}
