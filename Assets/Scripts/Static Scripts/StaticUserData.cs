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

    public static ProgressionData ProgressionData;

    public static List<string> UsedEffects = new List<string>();
}
