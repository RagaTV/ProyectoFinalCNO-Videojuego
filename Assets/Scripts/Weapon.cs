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
            weaponLvl++;
            statsUpdated = true;
 
            UIController.instance.UpdateInventoryUI();
        }
    }

    public void GenerateLevelPath()
    {
        if (stats != null && stats.Count > 0) return;

        stats = new List<WeaponStats>();
        stats.Add(baseStats);

        for (int i = 1; i < maxLevels; i++)
        {
            WeaponStats prevStats = stats[i - 1];
            WeaponStats newStats = new WeaponStats(prevStats); 

            // --- Lógica de Rareza ---
            int upgradesToApply = 1;
            float roll = Random.value; // Un número aleatorio entre 0.0 y 1.0

            if (roll < legendaryChance) //0.03
            {
                newStats.rarity = UpgradeRarity.Legendaria;
                upgradesToApply = 4;
            }
            // roll (0.03) < (0.03) + (0.15) = 0.18
            else if (roll < legendaryChance + epicChance)
            {
                newStats.rarity = UpgradeRarity.Epica; 
                upgradesToApply = 3;
            }
            // roll (0.18) < (0.03) + (0.15) + (0.32) = 0.50
            else if (roll < legendaryChance + epicChance + rareChance)
            {
                newStats.rarity = UpgradeRarity.Rara;
                upgradesToApply = 2;
            }
            else
            {
                newStats.rarity = UpgradeRarity.Comun;
                upgradesToApply = 1;
            }


            // --- Lógica de Múltiples Mejoras ---
            List<string> upgradeTexts = new List<string>(); // Guarda los textos
            List<int> statsAlreadyUpgraded = new List<int>(); // Evita repetir mejoras
            int totalStatTypes = 6; // (0=Daño, 1=Vel, 2=Tamaño, 3=Cooldown, 4=Cant, 5=Dur)

            for (int j = 0; j < upgradesToApply; j++)
            {
                // Si ya mejoramos las 6 stats, no podemos hacer más
                if (statsAlreadyUpgraded.Count >= totalStatTypes) break; 
                
                int statToUpgrade;
                
                do {
                    statToUpgrade = Random.Range(0, totalStatTypes);
                } while (statsAlreadyUpgraded.Contains(statToUpgrade));
                
                statsAlreadyUpgraded.Add(statToUpgrade); // Marca la stat como usada

                // Aplica la mejora y guarda el texto
                switch (statToUpgrade)
                {
                    case 0: // Daño
                        float damageIncrease = 0.25f;
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

            // Añade el nivel recién creado a la lista 'stats'
            stats.Add(newStats);
        }
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