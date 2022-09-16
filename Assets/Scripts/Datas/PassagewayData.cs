using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class PassagewayData
{
    [SerializeField] private string name;
    [SerializeField] public Passageway passageway;
    [SerializeField] private Transform playerDestinationPosition;
    [SerializeField] private Passageway connectedToPassageway;

    public void GetPassagewayData(
                                out Transform p_playerDestinationPosition,

                                out Passageway p_connectedToPassageway
                                )
    {

        p_playerDestinationPosition = playerDestinationPosition;

        p_connectedToPassageway = connectedToPassageway;
    }
}

