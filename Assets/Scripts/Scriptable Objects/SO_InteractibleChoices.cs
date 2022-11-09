using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Interactible Choices Scriptable Object", menuName = "Scriptable Objects/InteractibleChoices")]

public class SO_InteractibleChoices : ScriptableObject
{
    public List<ChoiceData> gesutreChoiceDatas = new List<ChoiceData>();
    public List<ChoiceData> voiceChoiceDatas = new List<ChoiceData>();
    public List<ChoiceData> bodyPostureChoiceDatas = new List<ChoiceData>();
    public List<ChoiceData> eyeContactChoiceDatas = new List<ChoiceData>();
    public List<ChoiceData> proxemityChoiceDatas = new List<ChoiceData>();
}
