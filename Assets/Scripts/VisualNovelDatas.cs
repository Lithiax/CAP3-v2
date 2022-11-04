using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualNovelDatas : MonoBehaviour
{
    public static VisualNovelDatas instance;
    public void Awake()
    {
        instance = this;
    }
    public SO_Character mainCharacter;
    public List<SO_Character> characters = new List<SO_Character>();
    public List<Sprite> backgroundImages = new List<Sprite>();
    public static SO_Character FindCharacter(string p_name)
    {
        for (int i = 0; i < instance.characters.Count; i++)
        {
            if (instance.characters[i].idName.ToString().ToLower() == p_name.ToLower())
            {
                //Debug.Log("Returning " + instance.characters[i].name); 
                return instance.characters[i];
            }
        }
        return null;
    }

    public static CharacterEmotionType FindFaceEmotion(string p_name)
    {
        for (int i = 0; i < CharacterEmotionType.GetValues(typeof(CharacterEmotionType)).Length; i++)
        {
            if (((CharacterEmotionType)i).ToString().ToLower() == p_name.ToLower())
            {
                return (CharacterEmotionType)i;
            }
        }
        return CharacterEmotionType.none;
    }

    public static CharacterPositionType FindBodyPosition(string p_name)
    {
        for (int i = 0; i < CharacterPositionType.GetValues(typeof(CharacterEmotionType)).Length; i++)
        {
            if (((CharacterPositionType)i).ToString().ToLower() == p_name.ToLower())
            {
                return (CharacterPositionType)i;
            }
        }
        return CharacterPositionType.none;
    }

    public static bool TranslateIsFlipped(string p_name)
    {
        bool isFlipped = p_name.ToLower() == "true";
        return isFlipped;
    }

  

    public static HapticType FindHapticType(string p_name)
    {
        for (int i = 0; i < HapticType.GetValues(typeof(HapticType)).Length; i++)
        {
            if (((HapticType)i).ToString().ToLower() == p_name.ToLower())
            {
                return (HapticType)i;
            }
        }
        return HapticType.none;
    }

    public static VocalicType FindVocalicType(string p_name)
    {
        for (int i = 0; i < VocalicType.GetValues(typeof(VocalicType)).Length; i++)
        {
            if (((VocalicType)i).ToString().ToLower() == p_name.ToLower())
            {
                return (VocalicType)i;
            }
        }
        return VocalicType.none;
    }

    public static KinesicType FindKinesicType(string p_name)
    {
        for (int i = 0; i < KinesicType.GetValues(typeof(KinesicType)).Length; i++)
        {
            if (((KinesicType)i).ToString().ToLower() == p_name.ToLower())
            {
                return (KinesicType)i;
            }
        }
        return KinesicType.none;
    }
    public static OculesicType FindOculesicType(string p_name)
    {
        for (int i = 0; i < OculesicType.GetValues(typeof(OculesicType)).Length; i++)
        {
            if (((OculesicType)i).ToString().ToLower() == p_name.ToLower())
            {
                return (OculesicType)i;
            }
        }
        return OculesicType.none;
    }

    public static PhysicalApperanceType FindPhysicalAppearanceType(string p_name)
    {
        for (int i = 0; i < PhysicalApperanceType.GetValues(typeof(PhysicalApperanceType)).Length; i++)
        {
            if (((PhysicalApperanceType)i).ToString().ToLower() == p_name.ToLower())
            {
                return (PhysicalApperanceType)i;
            }
        }
        return PhysicalApperanceType.none;
    }

    public static Sprite FindBackgroundSprite(string p_name)
    {
        if (p_name != "none")
        {
            for (int i = 0; i < instance.backgroundImages.Count; i++)
            {
                if (instance.backgroundImages[i].name.ToString().ToLower() == p_name.ToLower())
                {
                    return instance.backgroundImages[i];
                }
            }
        }
        Debug.LogError(p_name + " NONE");
        return null;
    }

    public static SpecificEventType FindEventType(string p_name)
    {
        p_name = p_name.Replace(" ", string.Empty);
        for (int i = 0; i < SpecificEventType.GetValues(typeof(SpecificEventType)).Length; i++)
        {
            if (((SpecificEventType)i).ToString().ToLower() == p_name.ToLower())
            {
                return (SpecificEventType)i;
            }
        }
        return SpecificEventType.none;
    }
}
