using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class HealthUI : MonoBehaviour
{
    [SerializeField] private Image barUI;
    public int currentHealth = 100;
    public int maxHealth = 100;
    public Action<int> ModifyHealthEvent;
    private void Awake()
    {
        ModifyHealthEvent += ModifyHealth;
    }
    public void ModifyHealth(int p_modifier)
    {
        currentHealth += p_modifier;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else if (currentHealth < 0)
        {
            currentHealth = 0;
        }
        barUI.fillAmount = currentHealth / maxHealth;
    }
}
