using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
public class PopUpUI : MonoBehaviour
{
    [SerializeField] GameObject frame;
    [SerializeField] TMP_Text titleText;
    [SerializeField] TMP_Text contentText;

    public static Action<string,string> OnPopUpEvent;

    private void Awake()
    {
        OnPopUpEvent += Initialize;
    }
    public void Initialize(string p_title,string p_content)
    {
        Debug.Log("POPPING UP " + p_content);
        titleText.text = p_title;
        contentText.text = p_content;
        frame.SetActive(true);
    }

    public void CloseUI()
    {
        frame.SetActive(false);
    }


}
