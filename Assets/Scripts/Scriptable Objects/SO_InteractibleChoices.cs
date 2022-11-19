using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CueChoiceData
{
    [SerializeField]
    public CueType cueType;
    public List<ChoiceData> choiceDatas = new List<ChoiceData>();
}
[CreateAssetMenu(fileName = "New Interactible Choices Scriptable Object", menuName = "Scriptable Objects/InteractibleChoices")]


public class SO_InteractibleChoices : ScriptableObject
{
    public CharacterData characterData;
    public SO_Dialogues deathSheet;
    public List<CueChoiceData> choiceDatas = new List<CueChoiceData>();

    public List<ChoiceData> GetChoiceData(string p_cueTypeValue)
    {
        for (int i = 0; i < choiceDatas.Count; i++)
        {
            if (choiceDatas[i].cueType.ToString().ToLower() == p_cueTypeValue.ToLower())
            {
                Debug.Log("ddd: RETURN SOMETHING");
                return choiceDatas[i].choiceDatas;
               
            }
        }
        Debug.Log("ddd: RETURN NOTHING " + p_cueTypeValue);

        return CreateNewCategory(p_cueTypeValue);
    }

    List<ChoiceData> CreateNewCategory(string cueType)
    {
        CueChoiceData newCh = new CueChoiceData();
 
        for (int x = 0; x < CueType.GetValues(typeof(CueType)).Length - 1; x++)
        {
            string target = ((CueType)x).ToString().ToLower();
            if (cueType.ToLower() == target)
            {
                Debug.Log(cueType);
                newCh.cueType = (CueType)x;
                break;

            }
        }
     
        choiceDatas.Add(newCh);
   
        return newCh.choiceDatas;
    }

}
