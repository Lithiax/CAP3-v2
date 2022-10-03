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
    public Action<int> DamageHealthEvent;
    private void Awake()
    {
        DamageHealthEvent += TakeDamage;
    }
    public void TakeDamage(int p_modifier)
    {
        currentHealth -= p_modifier;
        barUI.fillAmount = currentHealth / maxHealth;
    }
}
