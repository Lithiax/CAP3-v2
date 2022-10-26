using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
[System.Serializable]
public class CueUIPresetData
{
    [SerializeField]
    public CueType cueType;
    [SerializeField]
    public Sprite cueIcon;
    [SerializeField]
    public Vector2 position;
}
public class ActionUIs : MonoBehaviour
{
    [SerializeField] private CueUI currentCueUI;
    [SerializeField] private List<CueUIPresetData> cueUIPresetDatas;
    CueUIPresetData GetCueUIPresetData(CueType p_cueType)
    {

        for (int i = 0; i < cueUIPresetDatas.Count; i++)
        {
            if (cueUIPresetDatas[i].cueType == p_cueType)
            {
                return cueUIPresetDatas[i];
            }
        }
        return null;
    }
 
    public void Entered(ActionUI test)
    {
        //ToggleContainer();
        CueUIPresetData chosenCueUIPresetData = GetCueUIPresetData(test.cueType);
        currentCueUI.Initialize(test.cueType.ToString(), chosenCueUIPresetData.cueIcon, test.cueValue, chosenCueUIPresetData.position);
        
        StartCoroutine(Ent());
       
    }

    IEnumerator Ent()
    {
        yield return new WaitForSeconds(1f);
        currentCueUI.gameObject.SetActive(true);
        CharacterDialogueUI.OnInspectingEvent.Invoke();
       //actionsContainer.SetActive(true);
    }

    public void ClosedUI()
    {
        StopAllCoroutines();
        CharacterDialogueUI.OnDeinspectingEvent.Invoke();
        currentCueUI.gameObject.SetActive(false);
    }
}
