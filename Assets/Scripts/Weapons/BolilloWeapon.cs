using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BolilloWeapon : Weapon
{
    [Header("Configuración Proyectil")]
    public Projectile projectile; 
    [Header("Configuración Arma Bolillo")]
    public Transform holder;
    public EnemyDamager baseDamager; 
    public float timeBetweenShot;
    private float shotCounter;  
    private int currentAmount; 

    void Start()
    {
        SetStats(); 
    }

    void Update()
    {
        if(statsUpdated == true)
        {
            statsUpdated = false;
            SetStats(); 
        }

        shotCounter -= Time.deltaTime;
        
        if (shotCounter <= 0)
        {
            shotCounter = stats[weaponLvl].attackDelay;

            for (int i = 0; i < currentAmount; i++)
            {
                float angle = Random.Range(0f, 360f);

                Quaternion spawnRotation = Quaternion.AngleAxis(angle, Vector3.forward);
                
                Projectile newProjectileScript = Instantiate(
                    projectile, 
                    holder.position, 
                    spawnRotation
                );
                
                GameObject newProjectile = newProjectileScript.gameObject;
                newProjectile.SetActive(true);

                EnemyDamager damagerScript = newProjectile.GetComponent<EnemyDamager>();
                
                if (damagerScript != null)
                {
                    damagerScript.weaponID = this;
                    damagerScript.damageAmount = stats[weaponLvl].damage;
                    damagerScript.lifeTime = stats[weaponLvl].duration;
                    
                    newProjectile.transform.localScale = Vector3.one * stats[weaponLvl].size * PlayerStats.instance.projectileSizeMultiplier;
                }
                
                newProjectileScript.moveSpeed = stats[weaponLvl].speed;
            }
        }
    }

    public void SetStats()
    {
        currentAmount = stats[weaponLvl].amount;
        shotCounter = 0f; 
        if (baseDamager != null)
        {
             baseDamager.damageAmount = stats[weaponLvl].damage;
             baseDamager.lifeTime = stats[weaponLvl].duration;
        }
        
    }
}