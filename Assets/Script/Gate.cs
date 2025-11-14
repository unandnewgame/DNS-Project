using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class Gate : MonoBehaviour
{
    public int maxHealth = 100;
    public float healthRegen;

    [SerializeField] private float currentHealths;
    [SerializeField] private int currentHealth;

    public Slider slider;
    public bool isUnderAttack;

    void Start()
    {
        currentHealth = maxHealth;
        currentHealths = currentHealth;
        slider.maxValue = maxHealth;
        isUnderAttack = false;
    }
    void Update()
    {
        slider.value = currentHealth;
        if(!isUnderAttack)
        {
            if(currentHealth >= maxHealth)
            {
                currentHealth = maxHealth;
            }
            else if(currentHealth <= maxHealth)
            {
                currentHealths += healthRegen * Time.deltaTime;
                currentHealth = Mathf.FloorToInt(currentHealths);
            }
        }
        else
        {
            currentHealths = currentHealth;
        }
    }

    public void SetMaxHealth()
    {
        slider.value = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"Gate took {damage} damage! Remaining HP: {currentHealth}");
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("💀 Gate Destroyed! Game Over.");
        // TODO: Add GameManager call for Game Over state
    }
}
