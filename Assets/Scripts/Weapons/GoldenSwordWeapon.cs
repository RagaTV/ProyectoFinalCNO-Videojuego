using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldenSwordWeapon : Weapon
{
    public float rotateSpeed;
    public Transform holder, fireballToSpawn;
    public float timeBetweenSpawn;
    private float spawnCounter;
    public EnemyDamager damager;

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
            Instantiate(fireballToSpawn, fireballToSpawn.position, fireballToSpawn.rotation, holder).gameObject.SetActive(true);
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
        transform.localScale = Vector3.one * stats[weaponLvl].size;
        timeBetweenSpawn = stats[weaponLvl].attackDelay;
        damager.lifeTime = stats[weaponLvl].duration;

        spawnCounter = 0f; 
    }
}
