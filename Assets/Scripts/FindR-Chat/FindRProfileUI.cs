using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FindRProfileUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameField;
    [SerializeField] TextMeshProUGUI bDay;
    void Start()
    {
        nameField.text = StaticUserData.name;

        bDay.text = StaticUserData.b_month + "/" + StaticUserData.b_day + "/" + StaticUserData.b_year;
    }
}
