using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TestUI : MonoBehaviour
{
    public TextMeshProUGUI oneThreadRowOneText;
    public TextMeshProUGUI oneThreadRowTwoText;
    public TextMeshProUGUI oneThreadRowThreeText;
    public TextMeshProUGUI oneThreadRowFourText;
    public TextMeshProUGUI specificOneAText;
    public TextMeshProUGUI specificOneBText;
    public TextMeshProUGUI specificOneCText;
    public TextMeshProUGUI specificOneDText;
    public TextMeshProUGUI specificTwoAText;
    public TextMeshProUGUI specificTwoBText;
    public TextMeshProUGUI specificTwoCText;
    public TextMeshProUGUI specificTwoDText;

    public void OnEnable()
    {
        SpreadSheetReader.OnFinishedLoadingValues += SettingValues;
    }

    public void SettingValues()
    {

        oneThreadRowOneText.text = SpreadSheetReader.GetRowString(0);
        oneThreadRowTwoText.text = SpreadSheetReader.GetRowString(1);
        oneThreadRowThreeText.text = SpreadSheetReader.GetRowString(2);
        oneThreadRowFourText.text = SpreadSheetReader.GetRowString(3);

        specificOneAText.text = SpreadSheetReader.GetCellString(1, 0);
        specificOneBText.text = SpreadSheetReader.GetCellString(1, 1);
        specificOneCText.text = SpreadSheetReader.GetCellString(1, 2);
        specificOneDText.text = SpreadSheetReader.GetCellString(1, 3);

        specificTwoAText.text = SpreadSheetReader.GetCellString(2, 0);
        specificTwoBText.text = SpreadSheetReader.GetCellString(2, 1);
        specificTwoCText.text = SpreadSheetReader.GetCellString(2, 2);
        specificTwoDText.text = SpreadSheetReader.GetCellString(2, 3);

    }
}
