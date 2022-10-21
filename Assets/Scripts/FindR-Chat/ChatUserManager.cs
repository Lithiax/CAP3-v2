using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ChatUserManager : MonoBehaviour
{
    //Keep UserData empty on runtime, only putshit for testing
    [SerializeField] List<ChatUserSO> UserDataTesting;
    [SerializeField] GameObject UserParent;
    [SerializeField] GameObject UserPrefab;
    [SerializeField] ToggleGroup toggleGroup;
    [SerializeField] ChatManagerUI chatManager;
    [SerializeField] FindREventsManager eventsManager;

    //To set a new user, just add it in the static script
    List<ChatUser> SpawnedUsers = new List<ChatUser>();

    private void Awake()
    {
        foreach (ChatUserData data in StaticUserData.ChatUserData)
        {
            if (UserDataTesting.Contains(data.UserSO)) continue;

            UserDataTesting.Add(data.UserSO);
        }
        foreach (ChatUserSO user in UserDataTesting)
        {
            GenerateUser(user);
        }

        if (SpawnedUsers.Count > 0)
        {
            SpawnedUsers[0].toggle.Select();
        }
    }

    void GenerateUser(ChatUserSO data)
    {
        foreach (string s in DialogueSpreadSheetPatternConstants.effects)
        {
            Debug.Log(s);
        }
        GameObject UserObj = Instantiate(UserPrefab, UserParent.transform);
        ChatUser UserComp = UserObj.GetComponent<ChatUser>();
        SpawnedUsers.Add(UserComp);

        UserComp.Init(data, eventsManager, chatManager, toggleGroup);
    }
}
