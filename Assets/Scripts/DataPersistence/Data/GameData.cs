using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public string CurrentSceneName;
    public int[] ChatUserIDs;
    public string[] GameEffects;

    public List<ChatUserData> ChatUserData;

    public GameData()
    {
        CurrentSceneName = "";
        ChatUserIDs = new int[0];
        GameEffects = new string[0];
    }

    public void DebugLogData()
    {
        //foreach (ChatUserData userData in ChatUserData)
        //{
        //    Debug.Log(userData.UserSO.profileName);
        //    Debug.Log(userData.CurrentNode.BaseNodeData.Name);
        //    Debug.Log(userData.ChatObjects.Count);
        //}
    }
}
