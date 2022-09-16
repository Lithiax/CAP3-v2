using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthModifyEvent : UnityEvent<float> { }

public class HealthModifiedEvent : UnityEvent<bool, float, float> { }

public class DeathEvent : UnityEvent { }
public class Health : MonoBehaviour
{
    [HideInInspector] public HealthOverheadUI healthOverheadUI;
    [HideInInspector] public bool isAlive;
    private float currentHealth;
    private float maxHealth;

    
    public HealthModifyEvent onHealthModifyEvent = new HealthModifyEvent();
    public HealthModifiedEvent onHealthModifiedEvent = new HealthModifiedEvent();

    public DeathEvent OnDeathEvent = new DeathEvent();
    private void OnEnable()
    {
        //InitializeValues();
        onHealthModifyEvent.AddListener(ModifyHealth);
        //OnDeath.AddListener(Death);
    }


    private void OnDisable()
    {


        onHealthModifyEvent.RemoveListener(ModifyHealth);
        //OnDeath.AddListener(Death);
        
    }
    public void SetValues(float p_maxHealth)
    {
        maxHealth = p_maxHealth;
    }
    public void InitializeValues()
    {
        isAlive = true;
        currentHealth = maxHealth;
    }

    public void ModifyHealth(float p_modifier)
    {
        currentHealth += p_modifier;
        onHealthModifiedEvent.Invoke(isAlive,currentHealth,maxHealth);
        CheckHealth();
    }
   

    public void CheckHealth()
    {
        if (currentHealth <= 0)
        {
            Death();
            
            OnDeathEvent.Invoke();
        }
    }

    public void Death()
    {
        isAlive = false;
    }
}
