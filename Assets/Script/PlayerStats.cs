using UnityEngine;

[CreateAssetMenu(menuName = "Stats/PlayerStats")]
public class PlayerStats : ScriptableObject
{
    public float maxHealth = 100f;
    public float currentHealth;
    public float baseDamage = 10f;

    public void Start()
    {
        currentHealth = maxHealth;
    }
    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }
}
