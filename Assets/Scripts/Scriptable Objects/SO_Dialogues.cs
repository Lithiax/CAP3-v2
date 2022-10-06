using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class ChoiceData
{
    public string words;
    //public string branchDialogueName;
    public int damage;
    public string eventID;
    public int healthCondition;
    public SO_Dialogues branchingToSO_Dialogues;

}
[CreateAssetMenu(fileName = "New Dialogues Scriptable Object", menuName = "Scriptable Objects/Dialogues")]
public class SO_Dialogues : ScriptableObject
{

    public List<Dialogue> dialogues = new List<Dialogue>();
    public List<ChoiceData> choiceDatas = new List<ChoiceData>();
    public bool isEnabled;
    public HapticType hapticType;
    public VocalicType vocalicType;
    public KinesicType kinesicType;
    public OculesicType oculesicType;
    public PhysicalApperanceType physicalApperanceType;
}
