using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    public PlayerStats playerStats;

    [Header("Player Status")]
    public float maxHealth;
    public float currentHealth;
    public float baseDamage;

    void Start()
    {
        if (playerStats != null)
        {
            maxHealth = playerStats.maxHealth;
            playerStats.currentHealth = playerStats.maxHealth;
            baseDamage = playerStats.baseDamage;
        }
        else
        {
            Debug.LogWarning("PlayerStats asset not assigned in PlayerStat!");
        }
    }

    
    public void Update()
    {
        maxHealth = playerStats.maxHealth;
        currentHealth = playerStats.currentHealth;
        baseDamage = playerStats.baseDamage;
    }
}
