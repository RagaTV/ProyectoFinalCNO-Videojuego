using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats instance;

    public float moveSpeed { get; private set; }
    public float maxHealth { get; private set; }
    public float healthRegen { get; private set; }
    public float damageMultiplier { get; private set; } 
    public float pickupRange { get; private set; }
    public float projectileSizeMultiplier { get; private set; } 
    public float armor { get; private set; } 

    [SerializeField] private float baseMoveSpeed = 5f;
    [SerializeField] private float baseMaxHealth = 100f;
    [SerializeField] private float baseHealthRegen = 0f;
    [SerializeField] private float baseDamage = 1f; 
    [SerializeField] private float basePickupRange = 0.5f;
    [SerializeField] private float baseProjectileSize = 1f; 
    [SerializeField] private float baseArmor = 0f;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        moveSpeed = baseMoveSpeed;
        maxHealth = baseMaxHealth;
        healthRegen = baseHealthRegen;
        damageMultiplier = baseDamage;
        pickupRange = basePickupRange;
        projectileSizeMultiplier = baseProjectileSize;
        armor = baseArmor;
    }

    public void ApplyStatsForPassive(PassiveItem passive, int level)
    {
        PassiveStatLevel stats = passive.levels[level];

        switch (passive.type)
        {
            case PassiveType.MoveSpeed:
                moveSpeed = baseMoveSpeed * (1f + stats.multiplier);
                break;
                
            case PassiveType.MaxHealth:
                maxHealth = baseMaxHealth * (1f + stats.multiplier);
                
                if (PlayerHealthController.instance != null)
                {
                    PlayerHealthController.instance.UpdateMaxHealth();
                }
                break;

            case PassiveType.HealthRegen:
                healthRegen = baseHealthRegen + stats.multiplier; 
                break;
                
            case PassiveType.Damage:
                damageMultiplier = baseDamage * (1f + stats.multiplier);
                break;

            case PassiveType.PickupRange:
                pickupRange = basePickupRange * (1f + stats.multiplier);
                break;
                
            case PassiveType.ProjectileSize:
                projectileSizeMultiplier = baseProjectileSize * (1f + stats.multiplier);
                break;
                
            case PassiveType.Armor:
                armor = baseArmor + stats.multiplier; 
                break;
        }
    }
}
