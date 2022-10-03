using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : Character
{

    public RectTransform avatarRectTransform;
    public Image avatarImage;

    private void Awake()
    {
        avatarRectTransform = avatarRectTransform ? avatarRectTransform : GetComponent<RectTransform>();
        avatarImage = avatarImage ? avatarImage : GetComponent<Image>();
    }
   
}
