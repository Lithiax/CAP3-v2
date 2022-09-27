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
    laugh,
    smile,
}

public enum CharacterPositionType
{
    left,
    center,
    right,
    none
}

public enum HapticType
{
    soft,
    hard,
    none
}

public enum VocalicType
{
    soft,
    hard,
    none
}

public enum KinesicType
{
    soft,
    hard,
    none
}

public enum OculesicType
{
    soft,
    hard,
    none
}


public enum PhysicalApperanceType
{
    soft,
    hard,
    none
}


[System.Serializable]
public class CharacterData
{
    public SO_Character character;
    public CharacterEmotionType faceEmotion;
    public CharacterEmotionType bodyEmotion;

    public CharacterPositionType characterPosition;
    public bool isFlipped;
    public bool isSpeaking;
}
[System.Serializable]
public class Dialogue
{
    public List< CharacterData >  characterDatas = new List< CharacterData >();

    public HapticType hapticType;
    public VocalicType vocalicType;
    public KinesicType kinesicType;
    public OculesicType oculesicType;
    public PhysicalApperanceType physicalApperanceType;
    [TextArea]
    public string words;

    public Sprite backgroundSprite;
    public SpeechTransitionType speechTransitionType;
}