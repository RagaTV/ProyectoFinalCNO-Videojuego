using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomWeapon : Weapon
{
    [Header("Configuración Proyectil")]
    public EnemyDamager damager;
    public Projectile projectile;
    [Header("Configuración Arma")]
    private float shotCounter;
    public float weaponRange;
    public LayerMask whatIsEnemy;
    // Start is called before the first frame update
    void Start()
    {
        SetStats();
    }

    // Update is called once per frame
    void Update()
{
    if(statsUpdated == true)
    {
        statsUpdated = false;
        SetStats(); 
    }

    shotCounter -= Time.deltaTime;
    
    if(shotCounter <= 0)
    {
        shotCounter = stats[weaponLvl].attackDelay;

        for(int i = 0; i < stats[weaponLvl].amount; i++)
        {
            float randomAngle = Random.Range(0f, 360f);

            Quaternion randomRotation = Quaternion.Euler(0f, 0f, randomAngle);
            
            projectile.transform.rotation = randomRotation;

            GameObject newProjectile = Instantiate(projectile, transform.position, randomRotation).gameObject;
            newProjectile.SetActive(true);
            
            EnemyDamager damagerScript = newProjectile.GetComponent<EnemyDamager>();
        
            if (damagerScript != null)
            {
                damagerScript.weaponID = this;
            }
        }
        
        
    }
}

    void SetStats()
    {
        damager.damageAmount = stats[weaponLvl].damage;
        transform.localScale = Vector3.one * stats[weaponLvl].size * PlayerStats.instance.projectileSizeMultiplier;
        damager.lifeTime = stats[weaponLvl].duration;
        projectile.moveSpeed = stats[weaponLvl].speed;
        shotCounter = 0f;
    }
}
