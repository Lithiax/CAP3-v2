using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class ObjectRequirement : MonoBehaviour
{
    public ObjectRequirer requirer;
    public Action OnStartRequirement;
    public Action<ObjectRequirement> OnRequirementMet;

    public void Spawned(ObjectRequirer p_requirer)
    {
        requirer = p_requirer;
        OnRequirementMet += requirer.RequirementMet;
        requirer.OnStartRequiring += StartRequirement;
        requirer.requirementCount++;
        Debug.Log(gameObject.name.ToString() + " - REGISTERED EVENTS ON SPAWNED");
    }

    public virtual void OnEnable()
    {
        Debug.Log("RAWWWWWW");


        if (!requirer)
        {
            Debug.Log(gameObject.name + " STILL MISSING REQUIRER");
        }
        else
        {
            OnRequirementMet += requirer.RequirementMet;
            requirer.OnStartRequiring += StartRequirement;
            requirer.requirementCount++;
            Debug.Log(gameObject.name.ToString() + " - REGISTERED EVENTS ON ENABLED");
        }




    }

    public void StartRequirement()
    {

        Debug.Log("start require");
        OnStartRequirement?.Invoke();

    }
    public void RequirementCleared()
    {


        OnRequirementMet.Invoke(this);

    }
}
