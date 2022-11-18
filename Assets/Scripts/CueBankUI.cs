using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CueBankUI : MonoBehaviour
{
    public GameObject frame;

    [SerializeField]
    private GameObject cueBankContainer;
    [SerializeField]
    public GameObject blackOverlay;
    [SerializeField] private TMP_Text hapticText;
    [SerializeField] private TMP_Text vocalicText;
    [SerializeField] private TMP_Text kinesicText;
    [SerializeField] private TMP_Text oculesicText;
    [SerializeField] private TMP_Text physicalAppearanceText;

    [SerializeField] public bool cueBankOpenable = false;

    [SerializeField]
    public GameObject speakerDialogueUI;
    [SerializeField]
    public GameObject healthUI;
    [SerializeField]
    public GameObject choicesUI;
    [SerializeField]
    public GameObject popUpUI;
    [SerializeField]
    public GameObject extras;
    [SerializeField]
    public GameObject extrasButton;
    public void ResetCueBankUI()
    {
        cueBankContainer.gameObject.SetActive(false);
        blackOverlay.SetActive(false);
    }

    public void ToggleCueBankUI()
    {
        if (StorylineManager.currentSO_Dialogues.cueBankData.isEnabled)
        {
            if (cueBankOpenable)
            {
                cueBankContainer.SetActive(!cueBankContainer.activeSelf);
                blackOverlay.SetActive(cueBankContainer.activeSelf);

                speakerDialogueUI.SetActive(!cueBankContainer.activeSelf);
                healthUI.SetActive(!cueBankContainer.activeSelf);
                choicesUI.SetActive(!cueBankContainer.activeSelf);
                popUpUI.SetActive(!cueBankContainer.activeSelf);
                extras.SetActive(!cueBankContainer.activeSelf);
                extrasButton.SetActive(!cueBankContainer.activeSelf);
            }

        }

    }

    public void SetCueBank(SO_Dialogues p_characterDatas)
    {

        cueBankOpenable = true;
        hapticText.text = p_characterDatas.cueBankData.gestureType.ToString();
        vocalicText.text = p_characterDatas.cueBankData.voiceType.ToString();
        kinesicText.text = p_characterDatas.cueBankData.bodyPostureType.ToString();
        oculesicText.text = p_characterDatas.cueBankData.eyeContactType.ToString();
        physicalAppearanceText.text = p_characterDatas.cueBankData.proxemityType.ToString();


    }
}
