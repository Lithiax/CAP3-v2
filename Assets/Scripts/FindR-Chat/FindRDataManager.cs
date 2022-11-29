using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindRDataManager : MonoBehaviour
{
    public ChatUserManager ChatUserManager;
    List<ChatUser> ChatUsers = new List<ChatUser>();

    private void Awake()
    {
        ChatUserManager.OnSpawnedUsers += OnUsersSpawned;
    }

    private void Start()
    {
        AudioManager.instance.AdditivePlayAudio("bgm", false);
    }

    void OnUsersSpawned(List<ChatUser> users)
    {
        ChatUsers = users;
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
