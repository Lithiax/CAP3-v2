using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class RoomInfoUI : MonoBehaviour
{

    public GameObject roomNameGO;
    public TMP_Text roomDescriptionText;
    public TMP_Text roomNameText;

    public GridLayoutGroup gridLayout;

    private bool isFirstTime;
    private Passageway firstTimeCached;

    private void Start()
    {

        isFirstTime = true;
        

        gameObject.SetActive(false);
    }

    private void Awake()
    {
       // PlayerManager.onRoomEnteredEvent.AddListener(RoomEntered);
    }
    private void Destroy()
    {
       // PlayerManager.onRoomEnteredEvent.RemoveListener(RoomEntered);
    }

    
    public void RoomEntered(Passageway p_passageway)
    {

        string roomName;
        string roomDescription;
        p_passageway.room.GetRoomInfo(out roomName, out roomDescription);
        Vector2 cameraPosition = new Vector2(p_passageway.cameraDestinationPosition.x,
                                            p_passageway.cameraDestinationPosition.y);
        Vector2 cameraPanLimit = new Vector2(p_passageway.cameraPanLimit.x,
                                            p_passageway.cameraPanLimit.y
                                            );

        //PlayerManager.instance.currentRoomID = p_passageway.room.currentRoomID;
        gameObject.SetActive(true);
        StartCoroutine(Co_RoomInfoUITransition(roomName, roomDescription, cameraPosition, cameraPanLimit));
        
       


    }
    IEnumerator Co_RoomInfoUITransition(string p_roomName, string p_roomDescription, Vector2 p_cameraPos, Vector2 p_cameraPanLimit)
    {
       

        TransitionUI.onFadeTransition.Invoke(1);
        yield return new WaitForSeconds(0.5f);



        roomNameText.text = p_roomName;
        roomDescriptionText.text = p_roomDescription;

        Sequence te = DOTween.Sequence();
        te.Join(roomNameText.DOFade(1f, 0.75f));
        te.Join(roomDescriptionText.DOFade(1f, 0.75f));
        te.Play();


     
        CameraManager.onCameraMovedEvent.Invoke(p_cameraPos,
            p_cameraPanLimit);
        yield return new WaitForSeconds(3.75f);
        Sequence t = DOTween.Sequence();
        t.Join(roomNameText.DOFade(0f, 0.5f));
        t.Join(roomDescriptionText.DOFade(0f, 0.5f));
        t.Play();
       
 
        yield return t.WaitForCompletion();
      
        gameObject.SetActive(false);
        TransitionUI.onFadeTransition.Invoke(0, false);
        //UIManager.onGameplayModeChangedEvent.Invoke(false);
      
    }
}