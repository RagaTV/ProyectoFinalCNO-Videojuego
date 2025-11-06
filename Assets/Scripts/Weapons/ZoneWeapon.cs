using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneWeapon : Weapon
{
    public EnemyDamager damager; 
    
    public SoundEffect spawnSound;

    private float spawnTime, spawnCounter;
    private int currentAmount;

    void Start()
    {
        SetStats(); 
    }

    void Update()
    {
        if (statsUpdated == true)
        {
            statsUpdated = false;
            SetStats();
        }
        if (stats.Count == 0)
        {
            return; // Espera a que los stats se carguen
        }

        spawnCounter -= Time.deltaTime;
        if(spawnCounter <= 0f)
        {
            spawnCounter = spawnTime;

            if (spawnSound != SoundEffect.None)
            {
                SFXManager.instance.PlaySFXPitched(spawnSound);
            }
            
            Instantiate(damager, transform.position, Quaternion.identity, transform).gameObject.SetActive(true);
        }
    }

    void SetStats()
    {
        damager.damageAmount = stats[weaponLvl].damage;
        damager.lifeTime = stats[weaponLvl].duration;
        damager.timeBetweenDamage = 1f / stats[weaponLvl].speed;
        damager.damageAmountMultiplier = stats[weaponLvl].amount;
        damager.transform.localScale = Vector3.one * stats[weaponLvl].size * PlayerStats.instance.projectileSizeMultiplier;

        spawnTime = stats[weaponLvl].attackDelay;
                    
        spawnCounter = 0f;
    }
}