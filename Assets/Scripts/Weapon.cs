using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UpgradeRarity
{
    Comun,    // 1 Mejora (Gris)
    Rara,      // 2 Mejoras (Azul)
    Epica,      // 3 Mejoras (Morado)
    Legendaria  // 4 Mejoras (Amarillo)
}

public class Weapon : MonoBehaviour
{
    public WeaponStats baseStats;
    public List<WeaponStats> stats;
    [HideInInspector]
    public bool statsUpdated;
    public Sprite icon;
    public int maxLevels = 10;
    public int weaponLvl = 0;

    private float rareChance = 0.32f;      // 30%
    private float epicChance = 0.15f;      // 15%
    private float legendaryChance = 0.03f;  // 3%

    public void LevelUp()
    {
        if (weaponLvl < stats.Count - 1)
        {
            /*weaponLvl++;
            statsUpdated = true;
            UIController.instance.UpdateInventoryUI();*/

            WeaponStats statsToApply = GenerateNextLevelStats();
            ApplyLevel(statsToApply);
        }
    }

    public WeaponStats GenerateNextLevelStats()
    {
        //  Obtiene los stats actuales
        WeaponStats prevStats = stats[weaponLvl];
        WeaponStats newStats = new WeaponStats(prevStats); 

        //  Obtiene la suerte actual (con seguridad)
        float playerLuck = 1f;
        try { playerLuck = PlayerStats.instance.luck; }
        catch { playerLuck = 1f; }
        
        //  Calcula las probabilidades (¡tu lógica de Suerte!)
        float finalLegendaryChance = legendaryChance * playerLuck;
        float finalEpicChance = epicChance * playerLuck;
        float finalRareChance = rareChance * playerLuck;
        
        int upgradesToApply;
        float roll = Random.value; 

        if (roll < finalLegendaryChance)
        {
            newStats.rarity = UpgradeRarity.Legendaria;
            upgradesToApply = 4;
        }
        else if (roll < finalLegendaryChance + finalEpicChance)
        {
            newStats.rarity = UpgradeRarity.Epica; 
            upgradesToApply = 3;
        }
        else if (roll < finalLegendaryChance + finalEpicChance + finalRareChance)
        {
            newStats.rarity = UpgradeRarity.Rara;
            upgradesToApply = 2;
        }
        else
        {
            newStats.rarity = UpgradeRarity.Comun;
            upgradesToApply = 1;
        }

        List<string> upgradeTexts = new List<string>();
        List<int> statsAlreadyUpgraded = new List<int>(); // Evita repetir mejoras
        int totalStatTypes = 6; // (0=Daño, 1=Vel, 2=Tamaño, 3=Cooldown, 4=Cant, 5=Dur)

        for (int j = 0; j < upgradesToApply; j++)
        {
            // Si ya mejoramos las 6 stats, no podemos hacer más
            if (statsAlreadyUpgraded.Count >= totalStatTypes) break; 
            
            int statToUpgrade;
            
            // Elige un stat que no haya sido mejorado en ESTE nivel
            do {
                statToUpgrade = Random.Range(0, totalStatTypes);
            } while (statsAlreadyUpgraded.Contains(statToUpgrade));
            
            statsAlreadyUpgraded.Add(statToUpgrade); // Marca la stat como usada

            // Aplica la mejora y guarda el texto
            switch (statToUpgrade)
            {
                case 0: // Daño
                    float damageIncrease = 0.20f;
                    newStats.damage *= (1f + damageIncrease);
                    upgradeTexts.Add($"+{damageIncrease:P0} Daño");
                    break;
                case 1: // Velocidad
                    float speedIncrease = 0.15f;
                    newStats.speed *= (1f + speedIncrease);
                    upgradeTexts.Add($"+{speedIncrease:P0} Vel.");
                    break;
                case 2: // Tamaño
                    float sizeIncrease = 0.1f;
                    newStats.size *= (1f + sizeIncrease);
                    upgradeTexts.Add($"+{sizeIncrease:P0} Área");
                    break;
                case 3: // Cooldown
                    float cooldownReduction = 0.1f;
                    newStats.attackDelay *= (1f - cooldownReduction);
                    upgradeTexts.Add($"-{cooldownReduction:P0} Cooldown");
                    break;
                case 4: // Cantidad
                    int amountIncrease = 1;
                    newStats.amount += amountIncrease;
                    upgradeTexts.Add($"+{amountIncrease} Proyectil");
                    break;
                case 5: // Duración
                    float durationIncrease = 0.2f;
                    newStats.duration *= (1f + durationIncrease);
                    upgradeTexts.Add($"+{durationIncrease:P0} Duración");
                    break;
            }
        }
        
        newStats.upgradeText = string.Join(", ", upgradeTexts);
        
        //  DEVUELVE el nivel generado (¡no lo añade a la lista!)
        return newStats;
    }
    
    public void ApplyLevel(WeaponStats statsToApply)
    {
        weaponLvl++;
        stats.Add(statsToApply);
        statsUpdated = true;

        UIController.instance.UpdateInventoryUI();
    }
}



[System.Serializable]

public class WeaponStats
{
    public float speed, damage, size, attackDelay, duration;
    public int amount;
    public string upgradeText;
    public UpgradeRarity rarity;
    public WeaponStats()
    {
        this.rarity = UpgradeRarity.Comun;
    }

    public WeaponStats(WeaponStats other)
    {
        this.speed = other.speed;
        this.damage = other.damage;
        this.size = other.size;
        this.attackDelay = other.attackDelay;
        this.amount = other.amount;
        this.duration = other.duration;
        // No copiamos el upgradeText, se generará uno nuevo
    }
}