using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ChatUserManager : MonoBehaviour, IDataPersistence
{
    //Keep UserData empty on runtime, only putshit for testing
    [SerializeField] List<ChatUserSO> AllUsers;
    Dictionary<int, ChatUserSO> UserDict = new Dictionary<int, ChatUserSO>();

    [SerializeField] List<ChatUserSO> UserDataTesting;
    [SerializeField] GameObject UserParent;
    [SerializeField] GameObject UserPrefab;
    [SerializeField] ToggleGroup toggleGroup;
    [SerializeField] ChatManagerUI chatManager;
    [SerializeField] FindREventsManager eventsManager;

    //To set a new user, just add it in the static script
    [HideInInspector] public List<ChatUser> SpawnedUsers = new List<ChatUser>();
    List<GameObject> SpawnedUserObjects = new List<GameObject>();
    List<int> IDs = new List<int>();
    List<string> effectsToRemove = new List<string>();
    [HideInInspector] public bool DataLoaded = false;

    GameData gameData = null;
    private void Awake()
    {
        chatManager.InitializeTransforms();
    }
    private void Start()
    {
        foreach (ChatUserData data in StaticUserData.ChatUserData)
        {
            if (UserDataTesting.Contains(data.UserSO)) continue;

            UserDataTesting.Add(data.UserSO);
        }

        foreach (ChatUserSO so in StaticUserData.UserSOs)
        {
            if (UserDataTesting.Contains(so)) continue;

            UserDataTesting.Add(so);
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
            Debug.Log("Active Effects: " + s);
        }
        GameObject UserObj = Instantiate(UserPrefab, UserParent.transform);
        ChatUser UserComp = UserObj.GetComponent<ChatUser>();
        SpawnedUsers.Add(UserComp);
        UserComp.OnRemoveEffect += AddToEffectsRemoveLog;
        UserComp.AllowLoadingData(gameData);


        IDs.Add(data.ID);

        UserComp.Init(data, eventsManager, chatManager, toggleGroup);
    }

    public void AddToEffectsRemoveLog(string s)
    {
        effectsToRemove.Add(s);
    }

    private void OnDisable()
    {
        foreach (string e in DialogueSpreadSheetPatternConstants.effects)
        {
            Debug.Log(e);
        }

        RemoveUsedEffects();
    }

    void RemoveUsedEffects()
    {
        Debug.Log("Remove Effects " + effectsToRemove.Count);
        DialogueSpreadSheetPatternConstants.effects.RemoveAll(x => effectsToRemove.Contains(x));

        foreach (string e in DialogueSpreadSheetPatternConstants.effects)
        {
            Debug.Log(e);
        }

        foreach (ChatUser user in SpawnedUsers)
        {
            user.OnRemoveEffect -= AddToEffectsRemoveLog;
        }
    }

    public void LoadData(GameData data)
    {
        this.gameData = data;
        UserDataTesting.Clear();
        UserDict.Clear();

        DataLoaded = true;
        if (data.ChatUserIDs.Length <= 0) return;

        foreach (ChatUserSO user in AllUsers)
        {
            UserDict.Add(user.ID, user);
        }

        foreach (int id in data.ChatUserIDs)
        {
            UserDataTesting.Add(UserDict[id]);
        }

        effectsToRemove = data.EffectsUsed;

        RemoveUsedEffects();
    }

    public void SaveData(ref GameData data)
    {
        data.CurrentSceneName = "FindR";
        data.ChatUserIDs = IDs.ToArray();
        data.EffectsUsed = effectsToRemove;

        foreach (int id in data.ChatUserIDs)
        {
            Debug.Log(id);

        }
    }
}
