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
    public PassiveStatLevel baseStats; 
    
    [HideInInspector] 
    public List<PassiveStatLevel> levels;

    // --- Tus probabilidades ---
    private float rareChance = 0.25f;
    private float epicChance = 0.12f;
    private float legendaryChance = 0.03f;

    public void GenerateLevelPath()
    {
        if (levels != null && levels.Count > 0) return;

        levels = new List<PassiveStatLevel>();
        levels.Add(baseStats); // Nivel 1 (índice 0)

        for (int i = 1; i < maxLevels; i++)
        {
            PassiveStatLevel prevLevel = levels[i - 1];
            PassiveStatLevel newLevel = new PassiveStatLevel(prevLevel);

            float roll = Random.value;
            float bonusAmount = 0f;

            bool isFlatStat = (type == PassiveType.Armor || type == PassiveType.HealthRegen);

            if (roll < legendaryChance) // 3%
            {
                newLevel.rarity = UpgradeRarity.Legendaria;
                // Si es plano, da +1. Si es %, da +20%.
                bonusAmount = isFlatStat ? 1.5f : 0.20f; 
            }
            else if (roll < legendaryChance + epicChance) // 12%
            {
                newLevel.rarity = UpgradeRarity.Epica;
                // Si es plano, da +1. Si es %, da +15%.
                bonusAmount = isFlatStat ? 1f : 0.15f;
            }
            else if (roll < legendaryChance + epicChance + rareChance) // 25%
            {
                newLevel.rarity = UpgradeRarity.Rara;
                // Si es plano, da +0.5. Si es %, da +10%.
                bonusAmount = isFlatStat ? 0.5f : 0.10f;
            }
            else // 60%
            {
                newLevel.rarity = UpgradeRarity.Comun;
                // Si es plano, da +0.25. Si es %, da +5%.
                bonusAmount = isFlatStat ? 0.25f : 0.05f;
            }

            newLevel.multiplier += bonusAmount;

            string spanName = GetSpanish(type);
            
            if (isFlatStat)
            {
                // Muestra el número plano, ej: "+0.5 Armadura"
                newLevel.upgradeText = $"+{bonusAmount:F1} {spanName}";
            }
            else
            {
                // Muestra el porcentaje, ej: "+5% Daño"
                newLevel.upgradeText = $"+{bonusAmount:P0} {spanName}";
            }

            levels.Add(newLevel);
        }
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
                return "Rango de Recolección de Experiencia";
            case PassiveType.ProjectileSize:
                return "Tamaño de Proyectil";
            case PassiveType.Armor:
                return "Armadura";
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
    Armor
}