using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticUserData
{
    public static string name = "NO_NAME";

    public static string b_day = "0";
    public static string b_month = "0";
    public static string b_year = "0";

    //Adding to this will automatically set it up for the FindR Chat.
    public static List<ChatUserData> ChatUserData = new List<ChatUserData>();

    public static List<ChatUserSO> UserSOs = new List<ChatUserSO>();

    public static ProgressionData ProgressionData = new ProgressionData(1, 1);

    public static List<string> UsedEffects = new List<string>();

    public static void Reset()
    {
        UsedEffects.Clear();
        ChatUserData.Clear();
        UserSOs.Clear();
        ProgressionData = new ProgressionData(1,1);
    }

    public static void Save(ref GameData data)
    {
        data.ChatUserData = ChatUserData;
        data.ProgressionData = ProgressionData;
    }
}
