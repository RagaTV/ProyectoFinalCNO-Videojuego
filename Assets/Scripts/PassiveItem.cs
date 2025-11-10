using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPassive", menuName = "Survivors/Create New Passive")]
public class PassiveItem : ScriptableObject
{
    public Sprite icon;
    public string passiveName;
    public PassiveType type; 
    public int maxLevels = 20;
    public PassiveStatLevel baseStats = new PassiveStatLevel();

    [HideInInspector]
    public List<PassiveStatLevel> levels;
    private float bonusAmount;

    // --- Tus probabilidades ---
    private float rareChance = 0.25f;
    private float epicChance = 0.12f;
    private float legendaryChance = 0.03f;

    public void InitializeStats()
    {
        levels = new List<PassiveStatLevel>();
        levels.Add(baseStats);
    }

    public PassiveStatLevel GenerateNextLevelStats()
    {
        PassiveStatLevel prevLevel = levels[levels.Count - 1];
        PassiveStatLevel newLevel = new PassiveStatLevel(prevLevel);

        // 1. Obtiene la suerte actual (con seguridad)
        float playerLuck = 1f;
        try { playerLuck = PlayerStats.instance.luck; }
        catch { playerLuck = 1f; }
        
        // 2. Calcula las probabilidades
        float finalLegendaryChance = legendaryChance * playerLuck;
        float finalEpicChance = epicChance * playerLuck;
        float finalRareChance = rareChance * playerLuck;
        
        float roll = Random.value;
        bonusAmount = 0f;

        //  Definimos los tipos de stats
            bool isFlatStat = type == PassiveType.Armor || type == PassiveType.HealthRegen;
            bool isHealthStat = type == PassiveType.MaxHealth;
            bool isDamageStat = type == PassiveType.Damage || type == PassiveType.XPMultiplier || type == PassiveType.CoinMultiplier;
            // (Si no es ninguno de esos, es 'Utility')

            //  Aplicamos bonos diferentes basados en el tipo
            if (roll < legendaryChance) // 3%
            {
                newLevel.rarity = UpgradeRarity.Legendaria;
                if (isFlatStat) { bonusAmount = 1.5f; }        // Plano
                else if (isHealthStat) { bonusAmount = 0.75f; } // Vida (+75%)
                else if (isDamageStat) { bonusAmount = 0.30f; } // Daño (+30%)
                else { bonusAmount = 0.20f; }                   // Utilidad (+20%)
            }
            else if (roll < legendaryChance + epicChance) // 12%
            {
                newLevel.rarity = UpgradeRarity.Epica;
                if (isFlatStat) { bonusAmount = 1f; }
                else if (isHealthStat) { bonusAmount = 0.60f; } // Vida (+60%)
                else if (isDamageStat) { bonusAmount = 0.20f; } // Daño (+20%)
                else { bonusAmount = 0.15f; }                   // Utilidad (+15%)
            }
            else if (roll < legendaryChance + epicChance + rareChance) // 25%
            {
                newLevel.rarity = UpgradeRarity.Rara;
                if (isFlatStat) { bonusAmount = 0.5f; }
                else if (isHealthStat) { bonusAmount = 0.40f; } // Vida (+40%)
                else if (isDamageStat) { bonusAmount = 0.15f; } // Daño (+15%)
                else { bonusAmount = 0.10f; }                   // Utilidad (+10%)
            }
            else // 60%
            {
                newLevel.rarity = UpgradeRarity.Comun;
                if (isFlatStat) { bonusAmount = 0.25f; }
                else if (isHealthStat) { bonusAmount = 0.25f; } // Vida (+25%)
                else if (isDamageStat) { bonusAmount = 0.10f; } // Daño (+10%)
                else { bonusAmount = 0.05f; }                   // Utilidad (+5%)
            }
        
        newLevel.multiplier = prevLevel.multiplier + bonusAmount;
        
        string spanName = GetSpanish(type);

            if (isFlatStat)
            {
                newLevel.upgradeText =
                    $"+{bonusAmount:F1} {spanName} (Total: {newLevel.multiplier:F1})";
            }
            else
            {
                newLevel.upgradeText =
                    $"+{bonusAmount:P0} {spanName} (Total: {newLevel.multiplier:P0})";
            }
        
        // 3. DEVUELVE el nivel generado (¡no lo añade a la lista!)
        return newLevel;
    }

    // "Confirma" y aplica la mejora
    public void ApplyLevel(PassiveStatLevel statsToApply)
    {
        levels.Add(statsToApply);
        
        // Aplica el stat (¡el pasivo se aplica a sí mismo!)
        int currentLevelIndex = levels.Count - 1;
        PlayerStats.instance.ApplyStatsForPassive(this, currentLevelIndex);
        
        UIController.instance.UpdateInventoryUI();
    }
    
    public string GetSpanish(PassiveType type)
    {
        switch (type)
        {
            case PassiveType.MoveSpeed:
                return "Velocidad de Movimiento";
            case PassiveType.MaxHealth:
                return "Vida Máxima";
            case PassiveType.HealthRegen:
                return "Regeneración de Vida";
            case PassiveType.Damage:
                return "Daño";
            case PassiveType.PickupRange:
                return "Rango de Recolección";
            case PassiveType.ProjectileSize:
                return "Tamaño de Proyectil";
            case PassiveType.Armor:
                return "Armadura";
            case PassiveType.Luck:
                return "Suerte";
            case PassiveType.XPMultiplier:
                return "Multiplicador de EXP";
            case PassiveType.CoinMultiplier:
                return "Multiplicador de Monedas";
            default:
                return type.ToString();
        }
    }
}
public enum PassiveType
{
    MoveSpeed,
    MaxHealth,
    HealthRegen,
    Damage,
    PickupRange,
    ProjectileSize,
    Armor,
    Luck,
    XPMultiplier,
    CoinMultiplier
}