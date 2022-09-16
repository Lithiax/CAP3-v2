using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using TMPro;
//using System.Threading.Tasks;
public class TitlescreenUI : MonoBehaviour
{
    [Header("Settings")]
    public GameObject settingsScreen;


    [Header("Credits")]
    public GameObject creditsScreen;
    private void Start()
    {
        //TransitionUI.onFadeTransition.Invoke(0);
    }
    //public async void OnPlayButtonUIClicked()
    //{

    //    Task te = TransitionUI.onFadeTransition.Invoke(1);
    //    await te;

    //    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    //}

    public void OnPlayButtonUIClicked()
    {
        StartCoroutine(Co_OnPlayButtonUIClicked());
    }

    IEnumerator Co_OnPlayButtonUIClicked()
    {
        TransitionUI.onFadeTransition.Invoke(1);
        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    #region Settings
    //public void OnSettingsButtonClicked(bool p_bool)
    //{
    //    settingsScreen.SetActive(p_bool);
    //}
    //public void OnVolumeSliderChange(float value)
    //{
    //    volumeText.text = ((int)(value * 100)).ToString();
    //    audioMixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20);
    //}
    //public void OnVolumeButtonsClicked(bool p_bool)
    //{
    //    if (p_bool)
    //    {
    //        enableButton.GetComponentInChildren<TMP_Text>().color = Color.white;
    //        enableButton.image.sprite = enabledSprite;

    //        disableButton.GetComponentInChildren<TMP_Text>().color = Color.black;
    //        disableButton.image.sprite = disabledSprite;

    //        volumeSlider.value = volumeSlider.maxValue;
    //        volumeText.text = ((int)volumeSlider.maxValue * 100).ToString();
    //        audioMixer.SetFloat("MasterVolume", 0);
    //    }
    //    else
    //    {
    //        disableButton.GetComponentInChildren<TMP_Text>().color = Color.white;
    //        disableButton.image.sprite = enabledSprite;

    //        enableButton.GetComponentInChildren<TMP_Text>().color = Color.black;
    //        enableButton.image.sprite = disabledSprite;
            
    //        volumeSlider.value = volumeSlider.minValue;
    //        volumeText.text = ((int)volumeSlider.minValue * 100).ToString();
    //        audioMixer.SetFloat("MasterVolume", -80);
    //    }
    //}
    #endregion

    public void OnCreditsButtonClicked(bool p_bool)
    {
        creditsScreen.SetActive(p_bool);
    }

    public void OnQuitButtonUIClicked()
    {
        Debug.Log("Game quit!");
        Application.Quit();
    }

}
