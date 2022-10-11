using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CueBankUI : MonoBehaviour
{
    public GameObject frame;
    [SerializeField] CharacterDialogueUI characterDialogueUI;
    [SerializeField]
    private GameObject cueBankContainer;
    [SerializeField] private TMP_Text hapticText;
    [SerializeField] private TMP_Text vocalicText;
    [SerializeField] private TMP_Text kinesicText;
    [SerializeField] private TMP_Text oculesicText;
    [SerializeField] private TMP_Text physicalAppearanceText;

    [SerializeField] public bool cueBankOpenable = false;
    public void ResetCueBankUI()
    {
        cueBankContainer.gameObject.SetActive(false);
    }

    public void ToggleCueBankUI()
    {
        if (characterDialogueUI.currentSO_Dialogues.isEnabled)
        {
            if (cueBankOpenable)
            {
                cueBankContainer.SetActive(!cueBankContainer.activeSelf);
            }

        }

    }

    public void SetCueBank(SO_Dialogues p_characterDatas)
    {

        cueBankOpenable = true;
        hapticText.text = p_characterDatas.hapticType.ToString();
        vocalicText.text = p_characterDatas.vocalicType.ToString();
        kinesicText.text = p_characterDatas.kinesicType.ToString();
        oculesicText.text = p_characterDatas.oculesicType.ToString();
        physicalAppearanceText.text = p_characterDatas.physicalApperanceType.ToString();


    }
}
