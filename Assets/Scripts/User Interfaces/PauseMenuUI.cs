using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class PauseMenuUI : MonoBehaviour
{
    public GameObject pauseMenuPanel;
    public GameObject mapGameObject;
    //public TutorialPanelUI tutorialPanelUI;
    public GameObject skipTutorialButton;

    [Header("Settings")]
    public GameObject settingsGameObject;
    public AudioMixer audioMixer;
    public Slider volumeSlider;
    public TMP_Text volumeText;
    public Button enableButton;
    public Button disableButton;
    public Sprite enabledSprite;
    public Sprite disabledSprite;

    private void Awake()
    {
        

    }
    public void BackToPauseMenuButton()
    {

        mapGameObject.SetActive(false);

        settingsGameObject.SetActive(false);
        pauseMenuPanel.SetActive(true);
    }

 


    public void PauseMenuButton(bool p_bool)
    {
 
        // UIManager.onGameplayModeChangedEvent.Invoke(p_bool);
            
        pauseMenuPanel.SetActive(p_bool);
        
       
    }

    public void QuitButton()
    {
        Debug.Log("Game quit!");
        Application.Quit();
    }
  
 
    #region Settings
    public void SettingsButton()
    {
        //Debug.Log("Settings Button open!");
        settingsGameObject.SetActive(true);
    }
    public void OnVolumeSliderChange(float value)
    {
        volumeText.text = ((int)(value * 100)).ToString();
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20);
    }
    public void OnVolumeButtonsClicked(bool p_bool)
    {
        if (p_bool)
        {
            enableButton.GetComponentInChildren<TMP_Text>().color = Color.white;
            enableButton.image.sprite = enabledSprite;

            disableButton.GetComponentInChildren<TMP_Text>().color = Color.black;
            disableButton.image.sprite = disabledSprite;

            volumeSlider.value = volumeSlider.maxValue;
            volumeText.text = ((int)volumeSlider.maxValue * 100).ToString();
            audioMixer.SetFloat("MasterVolume", 0);
        }
        else
        {
            disableButton.GetComponentInChildren<TMP_Text>().color = Color.white;
            disableButton.image.sprite = enabledSprite;

            enableButton.GetComponentInChildren<TMP_Text>().color = Color.black;
            enableButton.image.sprite = disabledSprite;

            volumeSlider.value = volumeSlider.minValue;
            volumeText.text = ((int)volumeSlider.minValue * 100).ToString();
            audioMixer.SetFloat("MasterVolume", -80);
        }
    }
    #endregion

    public void MapButton(bool p_bool)
    {
        //if (p_bool) Debug.Log("Map open!");
        //else Debug.Log("Map closed!");
        mapGameObject.SetActive(p_bool);
    }
}
