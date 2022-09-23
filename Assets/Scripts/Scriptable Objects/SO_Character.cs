using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EmotionData
{
    public CharacterEmotionType type;
    public string triggerName;
}
[CreateAssetMenu(fileName = "New Character Scriptable Object", menuName = "Scriptable Objects/Character")]
public class SO_Character : ScriptableObject
{
    public new string name;
    public List<Sprite> avatars = new List<Sprite>();
    public Sprite avatar;
    public GameObject prefab;
    public List<EmotionData> emotionDatas;
}
