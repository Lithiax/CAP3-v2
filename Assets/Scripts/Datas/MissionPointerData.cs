using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class MissionPointerData
{
    public bool firstTimeWork = false;
    public bool isSeen = false;
  //  [NonReorderable][SerializeField] public OnEventDoTransform doTransform;
    public Vector2 currentPosition;
    public Vector2 objectivePosition;
    [SerializeField] public Image _missionPointerImage;
    [SerializeField] public RectTransform _missionDistanceTransform;
    [SerializeField] public RectTransform _missionPointerTransform;
    [SerializeField] public GameObject _missionPointerGameObject;

}
