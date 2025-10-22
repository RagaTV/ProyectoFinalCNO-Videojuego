using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public WeaponStats baseStats;
    public List<WeaponStats> stats;
    [HideInInspector]
    public bool statsUpdated;
    public Sprite icon;
    public int maxLevels = 10;
    public int weaponLvl = 0;

    public void LevelUp()
    {
        if (weaponLvl < stats.Count - 1)
        {
            weaponLvl++;
            statsUpdated = true;
        }
    }

    public void GenerateLevelPath()
    {
        if (stats != null && stats.Count > 0) return;

        stats = new List<WeaponStats>();
        stats.Add(baseStats); // El Nivel 0 es lo que pusiste en el Inspector

        for (int i = 1; i < maxLevels; i++)
        {
            // 1. Copia el nivel anterior
            WeaponStats prevStats = stats[i - 1];
            WeaponStats newStats = new WeaponStats(prevStats); // ¡Usamos el constructor de copia!

            // 2. Elige UNA estadística para mejorar
            // (0=Daño, 1=Velocidad, 2=Tamaño, 3=Cooldown, 4=Cantidad, 5=Duración)
            int statToUpgrade = Random.Range(0, 6); 

            // 3. Aplica la mejora y crea el texto
            switch (statToUpgrade)
            {
                case 0: // Daño
                    float damageIncrease = 0.2f; // 20%
                    newStats.damage *= (1f + damageIncrease);
                    newStats.upgradeText = $"+{damageIncrease:P0} Damage"; // P0 = formato porcentaje
                    break;
                
                case 1: // Velocidad (de rotación/proyectil)
                    float speedIncrease = 0.15f; // 15%
                    newStats.speed *= (1f + speedIncrease);
                    newStats.upgradeText = $"+{speedIncrease:P0} Speed";
                    break;

                case 2: // Tamaño
                    float sizeIncrease = 0.1f; // 10%
                    newStats.size *= (1f + sizeIncrease);
                    newStats.upgradeText = $"+{sizeIncrease:P0} Area";
                    break;

                case 3: // Cooldown (Attack Delay)
                    float cooldownReduction = 0.1f; // 10%
                    newStats.attackDelay *= (1f - cooldownReduction); // ¡El Cooldown se REDUCE!
                    newStats.upgradeText = $"-{cooldownReduction:P0} Cooldown";
                    break;
                
                case 4: // Cantidad (Amount)
                    int amountIncrease = 1;
                    newStats.amount += amountIncrease;
                    newStats.upgradeText = $"+{amountIncrease} Projectile";
                    break;

                case 5: // Duración
                    float durationIncrease = 0.2f; // 20%
                    newStats.duration *= (1f + durationIncrease);
                    newStats.upgradeText = $"+{durationIncrease:P0} Duration";
                    break;
            }

            // 4. Añade el nivel recién creado a la lista
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
    public WeaponStats() { }

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