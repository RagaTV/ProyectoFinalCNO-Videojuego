using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeapon : Weapon
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

            Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, weaponRange * stats[weaponLvl].size, whatIsEnemy);
            if(enemies.Length > 0)
            {
                for(int i=0; i < stats[weaponLvl].amount; i++)
                {
                    Vector3 targetPosition = enemies[Random.Range(0, enemies.Length)].transform.position;

                    Vector3 direction = targetPosition - transform.position;
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    angle -= 90;
                    projectile.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

                    GameObject newProjectile = Instantiate(projectile, projectile.transform.position, projectile.transform.rotation).gameObject;
                    newProjectile.SetActive(true);
                    EnemyDamager damagerScript = newProjectile.GetComponent<EnemyDamager>();
                
                    if (damagerScript != null)
                    {
                        damagerScript.weaponID = this;
                    }
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
