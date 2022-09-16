using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
public class Passageway : MonoBehaviour
{

    [HideInInspector] public Room room;
    private Transform playerDestinationPosition;
    public Vector2 cameraDestinationPosition { get; private set; }
    public Vector2 cameraPanLimit { get; private set; }
    [HideInInspector] public Passageway connectedToPassageway = null;



    private SpriteRenderer doorSprite;

    private bool canExit = false;

    private Vector2 originalDirection;

    //OLD 
  
    [SerializeField] public CardinalDirection cardinalDirection;
    private bool isHorizontalPassageway = false;
    private bool explored = false; // looks obselete




    public Action OnFirstTimeEntered;

    //OLD





    //NEW
    public void AssignPassageway(Room p_room,
                                 Transform p_playerDestinationPosition,
                                 Vector2 p_cameraDestinationPosition,
                                 Vector2 p_cameraPanLimit,
                                 Passageway p_connectedToPassageway)
    {
        room = p_room;
        playerDestinationPosition = p_playerDestinationPosition;
        cameraDestinationPosition = p_cameraDestinationPosition;
        cameraPanLimit = p_cameraPanLimit;


        connectedToPassageway = p_connectedToPassageway;
    }
  
 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (canExit)
            {
                collision.gameObject.transform.position = connectedToPassageway.playerDestinationPosition.position;

                EventBUS.onRoomEnteredEvent.Invoke(connectedToPassageway);
                EventBUS.onUpdateCurrentRoomIDEvent.Invoke(connectedToPassageway.room.currentRoomID);
            }
        
        }

        //old
        //if (collision.gameObject.GetComponent<Player>())
        //{
        //    originalDirection = (transform.position - collision.transform.position); //Destination - Origin
        //}
        //old
    }

    //NEW



    //old
    public void Awake()
    {
        doorSprite = GetComponent<SpriteRenderer>();
       // boxCollider = GetComponent<BoxCollider2D>();

        if (cardinalDirection == CardinalDirection.North ||
            cardinalDirection == CardinalDirection.South ||
            cardinalDirection == CardinalDirection.Center ||
            cardinalDirection == CardinalDirection.None)
        {
            isHorizontalPassageway = true;
        }
        else
        {
            isHorizontalPassageway = false;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        //if (collision.gameObject.GetComponent<Player>())
        //{
        //    Vector2 latestDirection = (collision.transform.position - transform.position);

        //    float originalChosenAxis = 0;
        //    float latestChosenAxis = 0;

        //    if (isHorizontalPassageway)//Decide if vertical/horizontal axis direction will it compare
        //    {
        //        latestChosenAxis = latestDirection.x;
        //        originalChosenAxis = originalDirection.x;
        //    }
        //    else
        //    {
        //        latestChosenAxis = latestDirection.y;
        //        originalChosenAxis = originalDirection.y;
        //    }

        //    //Compare if player proceeded with the same direction as he started with
        //    if (Mathf.Sign(latestChosenAxis) == Mathf.Sign(originalChosenAxis)) //If he is on the same direction as he started, it means he entered
        //    {
        //        explored = true;
        //        OnFirstTimeEntered.Invoke();
        //    }

        //}
    }

    public void Open()
    {
        doorSprite.enabled = false;
        canExit = true;

    }

    public void Close()
    {
        doorSprite.enabled = true;
        canExit = false;

    }

    //old
}
