using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldenSwordWeapon : Weapon
{
    public float rotateSpeed;
    public Transform holder, weaponToSpawn;
    public float timeBetweenSpawn;
    private float spawnCounter;
    public EnemyDamager damager;
    private int currentAmount;

    // Start is called before the first frame update
    void Start()
    {
        SetStats();
    }

    // Update is called once per frame
    void Update()
    {
        //holder.rotation = Quaternion.Euler(0f, 0f, holder.rotation.eulerAngles.z + (rotateSpeed * Time.deltaTime));
        holder.rotation = Quaternion.Euler(0f, 0f, holder.rotation.eulerAngles.z + (rotateSpeed * Time.deltaTime * stats[weaponLvl].speed));

        spawnCounter -= Time.deltaTime;
        if (spawnCounter <= 0)
        {
            spawnCounter = timeBetweenSpawn;
            for (int i = 0; i < currentAmount; i++)
            {
                float angle = (360f / currentAmount) * i;

                Quaternion spawnRotation = holder.rotation * Quaternion.Euler(0, 0, angle);

                Instantiate(weaponToSpawn, holder.position, spawnRotation, holder).gameObject.SetActive(true);
            }
        }

        if(statsUpdated == true)
        {
            statsUpdated = false;
            SetStats(); 
        }
    }
    
    public void SetStats()
    {
        damager.damageAmount = stats[weaponLvl].damage;
        transform.localScale = Vector3.one * stats[weaponLvl].size * PlayerStats.instance.projectileSizeMultiplier;
        damager.lifeTime = stats[weaponLvl].duration;
        timeBetweenSpawn = damager.lifeTime + 1f;
        currentAmount = stats[weaponLvl].amount;
        spawnCounter = 0f; 
    }
}
