using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class AccountCreationUI : MonoBehaviour
{
    [SerializeField] TMP_InputField nameField;
    [SerializeField] TMP_Dropdown bDay;
    [SerializeField] TMP_Dropdown mDay;
    [SerializeField] TMP_Dropdown yDay;

    public void CompletePressed()
    {
        StaticUserData.name = nameField.text;
        StaticUserData.b_day = bDay.options[bDay.value].text;
        StaticUserData.b_month = mDay.options[mDay.value].text;
        StaticUserData.b_year = yDay.options[yDay.value].text;

        if (nameField.text != "")
        {
            SceneManager.LoadScene("FindR");
        }
    }
}
