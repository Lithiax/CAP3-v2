using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class CueBankData
{
    public bool isEnabled;

    public string gestureType;
    public string voiceType;
    public string bodyPostureType;
    public string eyeContactType;
    public string proxemityType;


    public string GetCueValue(CueType cueType)
    {
        if (cueType == CueType.Voice)
        {
            return voiceType;

        }
        else if (cueType == CueType.Body_Posture)
        {
            return bodyPostureType;
        }
        else if (cueType == CueType.Proxemic)
        {
            return proxemityType;
        }
        else if (cueType == CueType.Eye_Contact)
        {
            return eyeContactType;
        }
        else if (cueType == CueType.Gesture)
        {
            return gestureType;
        }
        return "ERROR";
    }
}
[System.Serializable]
public class ChoiceData
{
    public string words;
    public SO_Dialogues branchDialogue;
    public string branchDialogueName;
    public int healthModifier;
    public string effectID;
    public string effectReferenceName;

    public bool isIsImmediateGoPhone;

    public bool isAutomaticEnabledColumnPattern;

    public bool isHealthConditionInUseColumnPattern;
    public int healthCeilingCondition;
    public int healthFloorCondition;

    public bool isEffectIDConditionInUseColumnPattern;
    public string effectIDCondition;

    public string popUpTitle;
    public string popUpContent;
}

[CreateAssetMenu(fileName = "New Dialogues Scriptable Object", menuName = "Scriptable Objects/Dialogues")]
public class SO_Dialogues : ScriptableObject
{
    public List<Dialogue> dialogues = new List<Dialogue>();
    public List<ChoiceData> choiceDatas = new List<ChoiceData>();
    public CueBankData cueBankData;
}
