using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using System;
public class ChoiceUI : MonoBehaviour
{
    [SerializeField] private TMP_Text choiceText;
    private string eventID = "0";

    public void InitializeValues(string p_text,string p_eventID)
    {
        choiceText.text = p_text;
        eventID = p_eventID;
    }

}
