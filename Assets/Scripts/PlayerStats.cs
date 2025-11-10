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
    public int enemiesKilled { get; private set; }
    public float totalDamageDone { get; private set; }
    public float luck { get; private set; }
    public float xpMultiplier { get; private set; }
    public float coinMultiplier { get; private set; }
    public Dictionary<Weapon, float> weaponDamageStats { get; private set; }


    [SerializeField] private float baseMoveSpeed = 5f;
    [SerializeField] private float baseMaxHealth = 100f;
    [SerializeField] private float baseHealthRegen = 0f;
    [SerializeField] private float baseDamage = 1f; 
    [SerializeField] private float basePickupRange = 0.5f;
    [SerializeField] private float baseProjectileSize = 1f;
    [SerializeField] private float baseArmor = 0f;
    [SerializeField] private float baseLuck = 1f; // 1f = 100%
    [SerializeField] private float baseXP = 1f;   // 1f = 100%
    [SerializeField] private float baseCoins = 1f; // 1f = 100%
    
    

    private void Awake()
    {
        instance = this;

        moveSpeed = baseMoveSpeed;
        maxHealth = baseMaxHealth;
        healthRegen = baseHealthRegen;
        damageMultiplier = baseDamage;
        pickupRange = basePickupRange;
        projectileSizeMultiplier = baseProjectileSize;
        armor = baseArmor;
        luck = baseLuck;
        xpMultiplier = baseXP;
        coinMultiplier = baseCoins;

        enemiesKilled = 0;
        totalDamageDone = 0f;

        weaponDamageStats = new Dictionary<Weapon, float>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
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
            case PassiveType.Luck:
                luck = baseLuck + stats.multiplier; 
                break;
            case PassiveType.XPMultiplier:
                xpMultiplier = baseXP * (1f + stats.multiplier);
                break;
            case PassiveType.CoinMultiplier:
                coinMultiplier = baseCoins * (1f + stats.multiplier);
                break;
        }
    }

    public void AddKill()
    {
        enemiesKilled++;
    }

    public void AddDamageDone(float damage)
    {
        totalDamageDone += damage;
    }

    public void AddDamageForWeapon(Weapon weapon, float damage)
    {
        totalDamageDone += damage;

        if (weaponDamageStats.ContainsKey(weapon))
        {
            weaponDamageStats[weapon] += damage;
        }
        else
        {
            weaponDamageStats[weapon] = damage;
        }
    }

    public void PrintWeaponDamageStats()
    {
        Debug.Log("--- REPORTE DE DAÑO POR ARMA ---");
        
        // Recorre cada 'par' (arma y daño) en el diccionario
        foreach (KeyValuePair<Weapon, float> pair in weaponDamageStats)
        {
            // pair.Key es el 'Weapon' (el scriptable object)
            // pair.Value es el 'daño' (el float)
            Debug.Log(pair.Key.name + ": " + pair.Value.ToString("F0")); // "F0" = número sin decimales
        }
    }
}
