using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserLevelData
{
    public int level;
    public List<ChatUser> ChatUsers;
}

public class FindRDataManager : MonoBehaviour
{
    public ChatUserManager ChatUserManager;
    List<ChatUser> ChatUsers = new List<ChatUser>();
    public List<UserLevelData> UserData;

    private void Start()
    {
        AudioManager.instance.AdditivePlayAudio("bgm", false);
    }

    private void OnDisable()
    {
        ChatUsers = ChatUserManager.SpawnedUsers;

        StaticUserData.ChatUserData.Clear();
        foreach (ChatUser user in ChatUsers)
        {
            StaticUserData.ChatUserData.Add(user.ChatData);
        }
    }
}
