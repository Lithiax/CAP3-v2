using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class MissionPointerManager : MonoBehaviour
{

    public Transform temporaryReplacement;

    private Camera _cam;
    public Camera uiCam;
    private Transform _camTransform;
    [NonReorderable]
    [SerializeField]
    private List<MissionPointerData> missionPointerData;
    [SerializeField]
    private RectTransform topScreen;
    [SerializeField]
    private RectTransform botScreen;
    [SerializeField]
    private RectTransform rightScreen;
    [SerializeField]
    private RectTransform leftScreen;

    [SerializeField]
    private RectTransform screenSize;
    [SerializeField]
    private float xBorderSize = 125f;
    [SerializeField]
    private float yTopBorderSize = 90f;
    [SerializeField]
    private float yBotBorderSize = 25f;

    bool isInUse = false;

    public Vector3 posOffset;
    public float mult = 1f;
    public Transform upTownToMidTown;
    public Transform midTownToUpTown;
    public Transform midTownToDownTown;
    public Transform midTownToPandayRoom;
    public Transform downTownToMidTown;
    public Transform pandayRoomToMidTown;
    private void Awake()
    {
        _cam = _cam ? _cam : Camera.main;

        _camTransform = _camTransform ? _camTransform : _cam.transform;
    
    }
    private void OnDestroy()
    {
     
    }
    private void OnGameplayModeChangedEvent(bool p_isActive)
    {

        if (!p_isActive == true)
        {
            
            isInUse = true;
            for (int i = 0; i < missionPointerData.Count; i++)
            {
                if (missionPointerData[i].firstTimeWork)
                {
                    ShowPointer(i);
                }

            }
            

        }
        else if (!p_isActive == false)
        {
            isInUse = false;
            for (int i = 0; i < missionPointerData.Count; i++)
            {
                if (missionPointerData[i].firstTimeWork)
                {
                    HidePointer(i);
                }

            }
        }
    }

    void ShowPointer(int index)
    {
        missionPointerData[index].isSeen = true;
        missionPointerData[index]._missionPointerGameObject.SetActive(true);
    }
    void HidePointer(int index)
    {
        missionPointerData[index].isSeen = false;
        missionPointerData[index]._missionPointerGameObject.SetActive(false);
    }
    void UpdateMissionPointerInSameRoom(int i)
    {
        bool canSee = true;
        Vector3 posOffset = new Vector3(0, 0, 0);
        if (missionPointerData[i].isSeen)
        {
           
            UpdateMissionPointer(i, missionPointerData[i].currentPosition, posOffset, canSee);
        }
    }
    void UpdateMissionPointerPosition(int targetIndex = -1)
    {
        if (targetIndex == -1)
        {
            for (int i = 0; i < missionPointerData.Count; i++)
            {
                UpdateMissionPointerInSameRoom(i);

            }
        }
        else
        {
            UpdateMissionPointerInSameRoom(targetIndex);
        }

    }
    void UpdateMissionPointersR(int i)
    {
        UpdateMissionPointers();
    }
    void UpdateMissionPointers()
    {
        
        if (isInUse == false)
        {
            for (int i = 0; i < missionPointerData.Count; i++)
            {
                if (missionPointerData[i].firstTimeWork)
                {
                    ShowPointer(i);
                }

            }
        }
        isInUse = true;
        UpdateMissionPointerPosition();
        
      

    }
    void UpdateMissionPointer(int index, Vector3 givenPosition, Vector3 offset, bool canSee)
    {
        Vector3 giv = givenPosition;
        if (givenPosition.x <= temporaryReplacement.position.x - 31.5)
        {
            giv.x = giv.x * mult;
        }
        Vector3 targetPositionScreenPoint =
     _cam.WorldToScreenPoint(giv - posOffset);

        //bool isOffScreen = targetPositionScreenPoint.x <= leftScreen.anchoredPosition.x ||
        //    targetPositionScreenPoint.x >= rightScreen.anchoredPosition.x ||
        //    targetPositionScreenPoint.y <= botScreen.anchoredPosition.y ||
        //    targetPositionScreenPoint.y >= topScreen.anchoredPosition.y;
        bool isOffScreen = givenPosition.x <= temporaryReplacement.position.x - 31.5 ||
                           givenPosition.x >= temporaryReplacement.position.x + 31.5 ||
                           givenPosition.y <= temporaryReplacement.position.y - 22.5 ||
                           givenPosition.y >= temporaryReplacement.position.y + 22.5;
        if (isOffScreen) //outside screen
        {


            if (!missionPointerData[index]._missionPointerImage.enabled)
            {
                missionPointerData[index]._missionPointerImage.enabled = true;
            }



            RotatePointer(index, (givenPosition - posOffset), offset);
            Vector3 cappedTargetScreenPosition =
            targetPositionScreenPoint;

            cappedTargetScreenPosition.x = Mathf.Clamp(cappedTargetScreenPosition.x, leftScreen.anchoredPosition.x, rightScreen.anchoredPosition.x);
            cappedTargetScreenPosition.y = Mathf.Clamp(cappedTargetScreenPosition.y, botScreen.anchoredPosition.y, topScreen.anchoredPosition.y);
            //cappedTargetScreenPosition.x = Mathf.Clamp(cappedTargetScreenPosition.x, Screen.width, Screen.width - 100f);
            //cappedTargetScreenPosition.y = Mathf.Clamp(cappedTargetScreenPosition.y, Screen.height, Screen.height - 100f);

            Vector3 pointerWorldPosition = cappedTargetScreenPosition;
            //Debug.Log(givenPosition + " | " +
            //        cappedTargetScreenPosition + " | " +
            //        leftScreen.position.x + " | " +
            //        rightScreen.position.x + " | " +
            //        botScreen.position.y + " | " +
            //        topScreen.position.y);
            missionPointerData[index]._missionPointerTransform.anchoredPosition = new Vector3(pointerWorldPosition.x, pointerWorldPosition.y, 0f);

            if (!missionPointerData[index]._missionDistanceTransform.gameObject.activeSelf)
            {
                missionPointerData[index]._missionDistanceTransform.gameObject.SetActive(true);
            }

        }
        else
        {
            if (canSee)
            {
                if (missionPointerData[index]._missionPointerImage.enabled)
                {
                    missionPointerData[index]._missionPointerImage.enabled = false;
                }
                if (!missionPointerData[index]._missionDistanceTransform.gameObject.activeSelf)
                {
                    missionPointerData[index]._missionDistanceTransform.gameObject.SetActive(true);
                }


                RotatePointer(index, (givenPosition - offset), offset);
                //Vector3 cappedTargetScreenPosition =
                //targetPositionScreenPoint;

                ////cappedTargetScreenPosition.x = Mathf.Clamp(cappedTargetScreenPosition.x, leftScreen.anchoredPosition.x, rightScreen.anchoredPosition.x);
                ////cappedTargetScreenPosition.y = Mathf.Clamp(cappedTargetScreenPosition.y, botScreen.anchoredPosition.y, topScreen.anchoredPosition.y);
                //cappedTargetScreenPosition.x = Mathf.Clamp(cappedTargetScreenPosition.x, Screen.width, Screen.width - 100f);
                //cappedTargetScreenPosition.y = Mathf.Clamp(cappedTargetScreenPosition.y, Screen.height, Screen.height - 100f);

                //Vector3 pointerWorldPosition = uiCam.ScreenToWorldPoint(cappedTargetScreenPosition);
                ////Debug.Log(givenPosition + " | " +
                ////        cappedTargetScreenPosition + " | " +
                ////        leftScreen.position.x + " | " +
                ////        rightScreen.position.x + " | " +
                ////        botScreen.position.y + " | " +
                ////        topScreen.position.y);
                //missionPointerData[index]._missionPointerTransform.position = pointerWorldPosition;
                //missionPointerData[index]._missionPointerTransform.localPosition = new Vector3(pointerWorldPosition.x, pointerWorldPosition.y, 0f);


            }
            else
            {
                if (missionPointerData[index]._missionPointerImage.enabled)
                {
                    missionPointerData[index]._missionPointerImage.enabled = false;
                }
                if (missionPointerData[index]._missionDistanceTransform.gameObject.activeSelf)
                {
                    missionPointerData[index]._missionDistanceTransform.gameObject.SetActive(false);
                }
            }




        }
    }

    private void RotatePointer(int index, Vector2 targetPositionWorld, Vector3 offset)
    {
        Vector2 originPosition = temporaryReplacement.position - offset;
        Vector2 dir = (targetPositionWorld - originPosition).normalized;
        float angle = (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg) % 360;
        missionPointerData[index]._missionPointerTransform.localEulerAngles = new Vector3(0f, 0f, angle);
        missionPointerData[index]._missionDistanceTransform.localEulerAngles = new Vector3(0f, 0f, -angle);
    }
}
