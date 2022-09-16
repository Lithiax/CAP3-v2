using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class RoomEnteredEvent : UnityEvent<Passageway> { }

public class Room : MonoBehaviour
{
    public int currentRoomID;
    public string roomName;
    public string roomDescription;

    [SerializeField] public Transform cameraPanLimitUpperRightTransform;    
    [HideInInspector] public Vector2 cameraPanLimit;


    [NonReorderable] [SerializeField] public List<PassagewayData> passagewayDatas = new List<PassagewayData>();



    private ObjectRequirer objectRequirer;

    public event Action OnRoomDiscovered;
    public event Action OnRoomCleared;

    public void Awake()
    {
        objectRequirer = GetComponent<ObjectRequirer>();
    }

    public void OnEnable()
    {
        foreach (PassagewayData currentPassageways in passagewayDatas)
        {
            currentPassageways.passageway.OnFirstTimeEntered += RoomDiscovered;
            OnRoomDiscovered += currentPassageways.passageway.Close;
            OnRoomCleared += currentPassageways.passageway.Open;



        }
        objectRequirer.OnAllRequirementsMet += RoomCleared;
        OnRoomDiscovered += objectRequirer.StartRequiring;
    }

    public void OnDisable()
    {
        foreach (PassagewayData currentPassageways in passagewayDatas)
        {
            currentPassageways.passageway.OnFirstTimeEntered -= RoomDiscovered;
            OnRoomDiscovered -= currentPassageways.passageway.Close;
            OnRoomCleared -= currentPassageways.passageway.Open;

        }
        objectRequirer.OnAllRequirementsMet -= RoomCleared;
        OnRoomDiscovered -= objectRequirer.StartRequiring;
        //OnRoomDiscovered -= objectRequirer.OnStartRequiring;

        cameraPanLimit = Vector2Abs(transform.position - cameraPanLimitUpperRightTransform.position);

        //foreach (ResourceNodeSpawner resourceNodeSpawner in resourceNodeSpawners)
        //{
        //    resourceNodeSpawner.AssignRoom(this);

        //}
        foreach (PassagewayData passagewayInfo in passagewayDatas)
        {
            Room room = this;
            Transform playerDestinationPosition;

            Passageway connectedToPassageway;

            passagewayInfo.GetPassagewayData(out playerDestinationPosition,
                out connectedToPassageway);
            passagewayInfo.passageway.AssignPassageway(room,
                                                        playerDestinationPosition,
                                                        transform.position,
                                                        cameraPanLimit,
                                                        connectedToPassageway);
        }
    }

    public void RoomDiscovered()
    {
        Debug.Log("ROOM DESIC");
        OnRoomDiscovered.Invoke();
    }
    public void RoomCleared()
    {
        OnRoomCleared.Invoke();
    }

    public void GetRoomInfo(out string p_roomName, out string p_roomDescription)
    {
        p_roomName = roomName;
        p_roomDescription = roomDescription;

    }

    Vector2 Vector2Abs(Vector2 p_vector2)
    {
        Vector2 answer = new Vector2(Mathf.Abs(p_vector2.x), Mathf.Abs(p_vector2.y));
        return answer;
    }

}
