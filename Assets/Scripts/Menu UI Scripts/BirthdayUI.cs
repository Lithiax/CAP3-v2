using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BirthdayUI : MonoBehaviour
{
    [SerializeField] TMP_Dropdown monthDropdown;
    [SerializeField] TMP_Dropdown dayDropdown;
    [SerializeField] TMP_Dropdown yearDropdown;

    void Start()
    {
        int year = System.DateTime.Now.Year;

        PopulateField(monthDropdown, 1, 12);
        PopulateField(dayDropdown, 1, 31);
        PopulateField(yearDropdown, 1990, year);
    }

    void PopulateField(TMP_Dropdown dropdown, int minRange, int maxRange)
    {
        dropdown.ClearOptions();
        List<string> data = new List<string>();

        for (int i = minRange; i <= maxRange; i++)
        {
            data.Add(i.ToString());
        }

        dropdown.AddOptions(data);
    }
}
