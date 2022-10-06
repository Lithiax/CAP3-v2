using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class LogTextUI : MonoBehaviour
{
    [SerializeField] TMP_Text speakerText;
    [SerializeField] TMP_Text dialogueText;
    public void Initialize(string p_speakerName, string p_dialogueText)
    {
        speakerText.text = p_speakerName;
        dialogueText.text = p_dialogueText;
    }
}
