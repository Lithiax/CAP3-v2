using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using System;
public class ChoiceUI : MonoBehaviour
{
    [SerializeField] private TMP_Text choiceText;

    public void InitializeValues(string p_text)
    {
        string te = p_text.Replace("<comma>", ",");
        choiceText.text = te;
    }

}
