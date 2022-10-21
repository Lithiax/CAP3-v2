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
    public List<UserLevelData> UserData;
    void Start()
    {
        
    }
}
