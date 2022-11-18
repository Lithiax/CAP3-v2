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
    none,
    angry,
    blush,
    cry,
    disgust,
    laughing,
    idle
}

public enum CharacterPositionType
{
    left,
    center,
    right,
    none
}


public enum SpecificEventType
{
    fadeInNOutEffect,
    fadeInEffect,
    fadeOutEffect,
    soundEffect,
    inputNameEvent,
    phoneEvent,
    shakeEffect,
    none,
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


    [TextArea]
    public string words;

    public Sprite backgroundSprite;

    public SpecificEventType specificEventType;
    public string specificEventParameter;
    public string backgroundMusic;
   // public string 
}