using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum SpeechTransitionType
{
    Typewriter,
    None,
}

public enum CharacterEmotionType
{
    happy,
    thinking,
    yawn,
    exclamation,
    lightbulb,
    question,
    heart,
    sad,
    mad,
    misc,
    none,
}

public enum CharacterPositionType
{
    left,
    center,
    right
}
[System.Serializable]
public class CharacterData
{
    public SO_Character character;
    public CharacterEmotionType emotion;
    public CharacterPositionType characterPosition;
    public bool isSpeaking;
}
[System.Serializable]
public class Dialogue
{
    public List< CharacterData >  characterDatas = new List< CharacterData >();

    //public SO_Character character;
    //public CharacterEmotionType emotion;

    [TextArea]
    public string words;

    public Sprite backgroundSprite;
    public SpeechTransitionType speechTransitionType;
}