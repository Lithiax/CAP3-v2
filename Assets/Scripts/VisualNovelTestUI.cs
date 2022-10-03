using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class VisualNovelTestUI : MonoBehaviour
{
    public TextMeshProUGUI oneThreadRowOneText;
    public TextMeshProUGUI oneThreadRowTwoText;
    public TextMeshProUGUI oneThreadRowThreeText;
    public TextMeshProUGUI oneThreadRowFourText;
    public TextMeshProUGUI specificOneAText;
    public TextMeshProUGUI specificOneBText;
    public TextMeshProUGUI specificOneCText;
    public TextMeshProUGUI specificOneDText;

    [Header("Google Sheet")]
    public string dialogueName;

    [Header("Characters Collumn Patterns")]
    public int characterCollumnPattern;
    public int faceEmotionCollumnPattern;
    public int bodyEmotionCollumnPattern;
    public int characterPositionCollumnPattern;
    public int isFlippedCollumnPattern;
    public int isSpeakingCollumnPattern;

    [Header("Characters Row Patterns")]
    public int characterOneRowPattern;
    public int characterTwoRowPattern;
    public int characterThreeRowPattern;

    [Header("Que Bank Collumn Patterns")]
    public int hapticTypeCollumnPattern;
    public int vocalicTypeCollumnPattern;
    public int kinesicCollumnPattern;
    public int oculesicCollumnPattern;
    public int physicalAppearanceCollumnPattern;

    [Header("Que Bank Row Patterns")]
    public int cueBankRowPattern;

    [Header("Words Row Patterns")]
    public int wordsRowPattern;

    [Header("Misc Collumn Patterns")]
    public int backgroundCollumnPattern;
    public int speechTransitionCollumnPattern;

    [Header("Misc Row Patterns")]
    public int miscRowPattern;

    public int currentIndex = 0;

    public List<int> dialogueIndexInSheet = new List<int>();
    public void OnEnable()
    {
        dialogueName = dialogueName.ToLower();
        SpreadSheetAPI.OnFinishedLoadingValues += SettingValues;

    }

    public void SettingValues()
    {

        oneThreadRowOneText.text = SpreadSheetAPI.GetRowString(0);
        oneThreadRowTwoText.text = SpreadSheetAPI.GetRowString(1);
        oneThreadRowThreeText.text = SpreadSheetAPI.GetRowString(2);
        oneThreadRowFourText.text = SpreadSheetAPI.GetRowString(3);

        specificOneAText.text = SpreadSheetAPI.GetCellString(1, 0);
        specificOneBText.text = SpreadSheetAPI.GetCellString(1, 1);
        specificOneCText.text = SpreadSheetAPI.GetCellString(1, 2);
        specificOneDText.text = SpreadSheetAPI.GetCellString(1, 3);
      
        //string test = SpreadSheetAPI.GetCellString(0, 0).ToLower();
        //if (test.Contains(dialogueName))
        //{
        //    Debug.Log("TEST");
        //}

        int dialogueCount = 0;
        for (int i = 0; i < SpreadSheetAPI.instance.testerFormattedSheetRows.Length; i++)
        {
            if (SpreadSheetAPI.instance.testerFormattedSheetRows[i].ToLower().Contains(dialogueName))
            {
                dialogueCount++;
                dialogueIndexInSheet.Add(i);

            }


        }

        for (int i = 0; i < dialogueCount; i++)
        {
            int currentDialogueIndex = dialogueIndexInSheet[currentIndex];
            Debug.Log("DIALOGUE:" + SpreadSheetAPI.GetCellString(currentDialogueIndex, 0));
            Debug.Log("CHARACTER 1: FACE EMOTION: " + SpreadSheetAPI.GetCellString(currentDialogueIndex + characterOneRowPattern, characterCollumnPattern));
            Debug.Log("CHARACTER 2: FACE EMOTION: " + SpreadSheetAPI.GetCellString(currentDialogueIndex + characterTwoRowPattern, characterCollumnPattern));
            Debug.Log("CHARACTER 3: FACE EMOTION: " + SpreadSheetAPI.GetCellString(currentDialogueIndex + characterThreeRowPattern, characterCollumnPattern));
       

        }
        Debug.Log("dialogue: " + dialogueCount);
     

    }
}
