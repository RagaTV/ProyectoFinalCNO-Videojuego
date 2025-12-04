using UnityEngine;

public abstract class BossBase : MonoBehaviour, IDamageable
{
    [Header("Stats Base")]
    public float maxHealth = 1000f;
    [HideInInspector]
    public float currentHealth;
    
    protected virtual void Start()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(float damageToTake)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damageToTake;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void TakeDamage(float damageToTake, bool shouldKnockBack)
    {
        // Bosses typically ignore knockback
        TakeDamage(damageToTake);
    }

    protected virtual void Die()
    {
        Debug.Log(gameObject.name + " has been defeated!");
        
        if (StoryManager.instance != null)
        {
            StoryManager.instance.OnBossDefeated(gameObject.name);
        }

        Destroy(gameObject);
    }
}
