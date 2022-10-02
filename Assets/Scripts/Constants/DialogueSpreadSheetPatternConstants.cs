using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueSpreadSheetPatternConstants 
{
    [Header("Google Sheet")]

    [Header("Settings")]
    public static string dialogueName = "[Dialogue]";
    public static string choiceName = "[Choice]";

    [Header("Characters Collumn Patterns")]
    public const int characterCollumnPattern = 0;
    public const int faceEmotionCollumnPattern = 1;
    public const int bodyEmotionCollumnPattern = 2;
    public const int characterPositionCollumnPattern = 3;
    public const int isFlippedCollumnPattern = 4;
    public const int isSpeakingCollumnPattern = 5;

    [Header("Characters Row Patterns")]
    public const int characterOneRowPattern = 2;
    public const int characterTwoRowPattern = 3;
    public const int characterThreeRowPattern = 4;

    [Header("Que Bank Collumn Patterns")]
    public const int hapticTypeCollumnPattern = 0;
    public const int vocalicTypeCollumnPattern = 1;
    public const int kinesicCollumnPattern = 2;
    public const int oculesicCollumnPattern = 3; 
    public const int physicalAppearanceCollumnPattern = 4;

    [Header("Que Bank Row Patterns")]
    public const int cueBankRowPattern = 6;

    [Header("Words Row Patterns")]
    public const int wordsRowPattern = 8;

    [Header("Misc Collumn Patterns")]
    public const int backgroundCollumnPattern = 0;

    [Header("Misc Row Patterns")]
    public const int miscRowPattern = 10;

    [Header("Choice Collumn Patterns")]
    public const int choiceNameCollumnPattern = 0;
    public const int nextDialogueSheetNameCollumnPattern = 1;

    [Header("Choice Row Patterns")]
    public const int choiceRowPattern = 2;

}
