using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FaceEmotionData
{
    public CharacterEmotionType type;
    public int index;
    public Sprite image;
}

[System.Serializable]
public class BodyEmotionData
{
    public CharacterEmotionType type;
    public Sprite image;
}

[CreateAssetMenu(fileName = "New Character Scriptable Object", menuName = "Scriptable Objects/Character")]
public class SO_Character : ScriptableObject
{
    public new string name;
    public Sprite avatar;
    public CharacterObject prefab;
    public List<FaceEmotionData> faceEmotionDatas;
    public List<BodyEmotionData> bodyEmotionDatas;
}
