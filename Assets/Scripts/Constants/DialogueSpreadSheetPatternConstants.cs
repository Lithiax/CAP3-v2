using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueSpreadSheetPatternConstants 
{
    public static List<string> effects = new List<string>();
    [Header("Google Sheet")]

    [Header("Settings")]
    public static string dialogueName = "[Dialogue]";
    public static string choiceName = "[Choice]";

    [Header("Que Bank Collumn Patterns")]
    public const int isEnabledCollumnPattern = 0;
    public const int hapticTypeCollumnPattern = 1;
    public const int vocalicTypeCollumnPattern = 2;
    public const int kinesicCollumnPattern = 3;
    public const int oculesicCollumnPattern = 4;
    public const int physicalAppearanceCollumnPattern = 5;

    [Header("Que Bank Row Patterns")]
    public const int cueBankRowPattern = 2;

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

    [Header("Words Row Patterns")]
    public const int wordsRowPattern = 6;

    [Header("Misc Collumn Patterns")]
    public const int backgroundCollumnPattern = 0;

    [Header("Misc Row Patterns")]
    public const int miscRowPattern = 8;

    [Header("Choice Collumn Patterns")]
    public const int choiceNameCollumnPattern = 0;
    public const int nextDialogueSheetNameCollumnPattern = 1;
    public const int choiceDamagePattern = 3;
    public const int eventID = 4;
    public const int healthCondition = 5;

    [Header("Choice Row Patterns")]
    public const int choiceRowPattern = 2;

}
