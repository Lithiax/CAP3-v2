using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class CueBankData
{
    public bool isEnabled;
    public string hapticType;
    public string vocalicType;
    public string kinesicType;
    public string oculesicType;
    public string physicalApperanceType;
}
[System.Serializable]
public class ChoiceData
{
    public string words;
    public SO_Dialogues branchDialogue;
    public string branchDialogueName;
    public int healthModifier;
    public int healthCeilingCondition;
    public int healthFloorCondition;

    public string effectID;

    public string popUpTitle;
    public string popUpContent;
}

[CreateAssetMenu(fileName = "New Dialogues Scriptable Object", menuName = "Scriptable Objects/Dialogues")]
public class SO_Dialogues : ScriptableObject
{
    public List<Dialogue> dialogues = new List<Dialogue>();
    public bool isAutomaticHealthEvaluation;
    public List<ChoiceData> choiceDatas = new List<ChoiceData>();
    public CueBankData cueBankData;
}
