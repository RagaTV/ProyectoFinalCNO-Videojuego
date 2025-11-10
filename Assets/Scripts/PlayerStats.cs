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

    public void PrintGameReport()
    {
        // Obtenemos el tiempo final de la partida
        float timeSurvived = UIController.instance.gameTimer;
        float totalDamage = totalDamageDone;
        float finalDPS = 0f;

        // 1. Cálculo de DPS (Daño por Segundo)
        if (timeSurvived > 0.1f)
        {
            finalDPS = totalDamage / timeSurvived;
        }

        Debug.Log("=====================================");
        Debug.Log("======= REPORTE FINAL DE PARTIDA ======");
        Debug.Log("=====================================");
        
        // ---------------------------------------------
        // 1. TIEMPO, KILLS Y DAÑO GLOBAL
        // ---------------------------------------------
        Debug.Log($"Tiempo Sobrevivido: {System.TimeSpan.FromSeconds(timeSurvived).ToString(@"mm\:ss")}");
        Debug.Log($"Enemigos Eliminados: {enemiesKilled}");
        Debug.Log($"Daño Total Infligido: {totalDamage.ToString("F0")}");
        Debug.Log($"Daño por Segundo (DPS): {finalDPS.ToString("F1")}");
        
        Debug.Log("-------------------------------------");
        Debug.Log("------ DAÑO DETALLADO POR ARMA ------");

        // 2. Daño por Arma
        if (weaponDamageStats.Count > 0)
        {
            foreach (KeyValuePair<Weapon, float> pair in weaponDamageStats)
            {
                // Usamos el nombre del objeto para el reporte
                Debug.Log($"  {pair.Key.gameObject.name}: {pair.Value.ToString("F0")}");
            }
        } else {
            Debug.Log("   (No se registró daño con armas)");
        }
        
        Debug.Log("-------------------------------------");
        Debug.Log("--- ESTADÍSTICAS FINALES DE JUGADOR ---");

        // 3. Estadísticas Finales (Las que el jugador terminó la partida)
        
        // --- Stats de Supervivencia ---
        Debug.Log($"  Vida Máxima: {maxHealth.ToString("F0")}");
        Debug.Log($"  Armadura: {armor.ToString("F1")}");
        Debug.Log($"  Regeneración: {healthRegen.ToString("F2")}");

        // --- Stats Ofensivos ---
        // Lo mostramos como porcentaje de bono total
        Debug.Log($"  Multiplicador de Daño: {(damageMultiplier * 100).ToString("F0")}%"); 
        Debug.Log($"  Tamaño Proyectil: {(projectileSizeMultiplier * 100).ToString("F0")}%");
        
        // --- Stats de Utilidad ---
        Debug.Log($"  Velocidad de Movimiento: {moveSpeed.ToString("F2")}");
        Debug.Log($"  Rango de Recolección: {pickupRange.ToString("F2")}");
        
        // --- Stats de Economía y Suerte ---
        // Los multiplicadores los mostramos como porcentaje (ej. 1.15 = 115%)
        Debug.Log($"  Suerte (Rarity): {(luck * 100).ToString("F0")}%");
        Debug.Log($"  Multiplicador de EXP: {(xpMultiplier * 100).ToString("F0")}%");
        Debug.Log($"  Multiplicador de Monedas: {(coinMultiplier * 100).ToString("F0")}%"); 

        Debug.Log("=====================================");
    }
}
