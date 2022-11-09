using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;
public class BackgroundUI : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;
    public static Action<Sprite> onSetBackgroundEvent;// = new onLoadAvatarsEvent

    private void Awake()
    {
        onSetBackgroundEvent += SetBackground;
    }
    void SetBackground(Sprite p_backgroundSprite)
    {
        if (p_backgroundSprite != null)
        {
            backgroundImage.sprite = p_backgroundSprite;
            backgroundImage.color = new Color32(255, 255, 255, 255);
        }
        else if (p_backgroundSprite == null)
        {
            backgroundImage.color = new Color32(0, 0, 0, 0);
        }
    }
}
