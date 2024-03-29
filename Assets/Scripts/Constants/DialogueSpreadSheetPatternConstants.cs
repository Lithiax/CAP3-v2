using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueSpreadSheetPatternConstants
{
    public static void AddEffect(string p_effect)
    {
        //if (DialogueSpreadSheetPatternConstants.effects.Count > 0)
        //{
        //    for (int i = 0; i < DialogueSpreadSheetPatternConstants.effects.Count; i++)
        //    {
        //        if (DialogueSpreadSheetPatternConstants.effects[i] == p_effect)
        //        {
        //            break;
        //        }
        //        if (i >= DialogueSpreadSheetPatternConstants.effects.Count)
        //        {
                    Debug.Log("ADDING EFFECT: " + p_effect);
                    DialogueSpreadSheetPatternConstants.effects.Add(p_effect);
        //        }
        //    }
        //}
        //else
        //{

        //    Debug.Log("ADDING EFFECT: " + p_effect);
        //    DialogueSpreadSheetPatternConstants.effects.Add(p_effect);
            
        //}
        
    

       // Debug.Log("ADDING EFFECT: " + p_effect);
        if (p_effect == "<progress>")
        {
       
            StaticUserData.ProgressionData.ProgressDate();
        }
        if (p_effect == "<ending>")
        {

            CharacterDialogueUI.creditsRoll.Invoke();//.GoBackMenu();
        }
        //else
        //{

        //    DialogueSpreadSheetPatternConstants.effects.Add(p_effect);
        //}
    }
    public static SO_Character cueCharacter;
    public static float penelopeHealth = 50;
    public static float bradHealth = 50;
    public static float liamHealth = 50;
    public static float maeveHealth = 50;

    public static List<string> effects = new List<string>();
    [Header("Google Sheet")]

    [Header("Settings")]
    public static string dialogueName = "[Dialogue]";
    public static string choiceName = "[Choice]";

    public static string gestureName = "Gesture";
    public static string voiceName = "Voice";
    public static string bodyPostureName = "Body Posture";
    public static string eyeContactName = "Eye Contact";
    public static string proxemityName = "Proxemity";

    [Header("Que Bank Column Patterns")]

    public const int isEnabledColumnPattern = 0;
    public const int gestureTypeColumnPattern = 1;
    public const int voiceTypeColumnPattern = 2;
    public const int bodyPostureColumnPattern = 3;
    public const int eyeContactColumnPattern = 4;
    public const int proxemityColumnPattern = 5;

    public const int isAutomaticHealthEvaluation = 6;

    [Header("Que Bank Row Patterns")]
    public const int cueBankRowPattern = 2;

    [Header("Characters Collumn Patterns")]
    public const int characterColumnPattern = 0;
    public const int faceEmotionColumnPattern = 1;
    public const int bodyEmotionColumnPattern = 2;
    public const int characterPositionColumnPattern = 3;
    public const int isFlippedColumnPattern = 4;
    public const int isSpeakingColumnPattern = 5;

    [Header("Characters Row Patterns")]
    public const int characterOneRowPattern = 2;
    public const int characterTwoRowPattern = 3;
    public const int characterThreeRowPattern = 4;

    [Header("Words Row Patterns")]
    public const int wordsRowPattern = 6;

    [Header("Misc Collumn Patterns")]
    public const int backgroundColumnPattern = 0;
    public const int eventTypeColumnPattern = 1;
    public const int eventParameterColumnPattern = 2;
    public const int backgroundMusicColumnPattern = 3;
    [Header("Misc Row Patterns")]
    public const int miscRowPattern = 8;

    [Header("Choice Collumn Patterns")]
    public const int choiceNameColumnPattern = 0;

    public const int nextDialogueSheetNameColumnPattern = 1;
    public const int healthModifierColumnPattern = 3;
    public const int effectIDColumnPattern = 4;
    public const int effectReferecedNameColumnPattern = 5;
    public const int isImmediateGoPhoneColumnPattern = 6;

    public const int isAutomaticEnabledColumnPattern = 0;

    public const int isHealthConditionInUseColumnPattern = 1;
    public const int healthCeilingConditionColumnPattern = 2;
    public const int healthFloorConditionColumnPattern = 3;

    public const int isEffectIDConditionInUseColumnPattern = 4;
    public const int effectIDConditionColumnPattern = 5;

    public const int popUpTitleColumnPattern = 0;
    public const int popUpContentColumnPattern = 1;

    [Header("Choice Row Patterns")]
    public const int choiceRowPattern = 2;
    public const int popUpRowPattern = 4;
    public const int choiceConditionRowPattern = 6;

}
