using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform trans;
    [HideInInspector] 
    public Vector2 offset;

    [HideInInspector] 
    public Vector2 panLimit;

    private void Awake()
    {
        CameraManager.onCameraMovedEvent.AddListener(CameraMoved);
    }

    private void OnDestroy()
    {
        CameraManager.onCameraMovedEvent.RemoveListener(CameraMoved);
    }
   
    public void CameraMoved(Vector2 p_newPosition, Vector2 p_panLimit)
    {
        offset = new Vector3(p_newPosition.x, p_newPosition.y);
        panLimit = p_panLimit;
    }
    private void Update()
    {
        Vector3 pos = trans.position;
        pos.x = Mathf.Clamp(pos.x, offset.x + -panLimit.x, offset.x + panLimit.x);
        pos.y = Mathf.Clamp(pos.y, offset.y + -panLimit.y, offset.y + panLimit.y);
        pos.z = transform.position.z;        
        transform.position = pos;
    }

    #region TEST
    //void test()
    //{
    //    if (currentDialogueCharacters.Count > so_Characters.Count)
    //    {
    //        //Mark the Characters to Add and Characters that Exists
    //        IdentifyCharactersToAdd(currentDialogueCharacters, so_Characters, tempCharacters, charactersToBeAdded);
    //    }
    //    else
    //    {
    //        //Mark the Characters to Remove
    //        IdentifyCharactersToRemove(so_Characters, currentDialogueCharacters, charactersToBeRemoved);

    //    }

    //    RemoveAvatar(charactersToBeRemoved);
    //    AddAvatar(charactersToBeAdded);

    //    so_Characters.Clear();
    //    for (int i = 0; i < characterPresetDatas.Count; i++)
    //    {
    //        so_Characters.Add(characterPresetDatas[i].so_Character);
    //        if (characterPresetDatas[i].so_Character != null)
    //        {
    //            SetSpeakerTint(currentSO_Dialogues.dialogues[currentDialogueIndex].characterDatas[i].isSpeaking, currentSO_Dialogues.dialogues[currentDialogueIndex].characterDatas[i].character);
    //        }
    //    }
    //}
        
    #endregion
}
