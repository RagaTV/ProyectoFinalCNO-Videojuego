using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamager : MonoBehaviour //weaponds
{
    public float damageAmount;
    public float lifeTime, growSpeed = 4f;
    private Vector3 targetSize;
    public bool shouldKnockBack;
    public bool destroyParent;
    public SoundEffect hitSound;

    public bool damageOverTime;
    public float timeBetweenDamage;
    private float damageCounter;
    private List<EnemyController> enemiesInrange = new List<EnemyController>();
    public int damageAmountMultiplier = 1;
    public Weapon weaponID;


    // Start is called before the first frame update
    void Start()
    { 
        //Destroy(gameObject, lifeTime);
        targetSize = transform.localScale;
        transform.localScale = Vector3.zero;

        if (weaponID == null)
        {
            // Busca en mis padres hasta encontrar el script "Weapon"
            weaponID = GetComponentInParent<Weapon>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.MoveTowards(transform.localScale, targetSize, growSpeed * Time.deltaTime);
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            targetSize = Vector3.zero;
            if (transform.localScale.x == 0f)
            {
                Destroy(gameObject);
                if (destroyParent)
                {
                    Destroy(transform.parent.gameObject);
                }
            }
        }
        
        if(damageOverTime == true)
        {
            damageCounter -= Time.deltaTime;
            if(damageCounter <= 0)
            {
                damageCounter = timeBetweenDamage;

                for(int i=0; i<enemiesInrange.Count; i++)
                {
                    if(enemiesInrange[i] != null && enemiesInrange[i].gameObject.activeInHierarchy)
                    {
                        float finalDamage = (damageAmount * damageAmountMultiplier) * PlayerStats.instance.damageMultiplier;
                        enemiesInrange[i].TakeDamage(finalDamage, shouldKnockBack);
                        PlayerStats.instance.AddDamageForWeapon(weaponID, finalDamage);
                    } 
                    else
                    {
                        // Si es nulo O está inactivo, lo quita de la lista
                        enemiesInrange.RemoveAt(i);
                        i--;
                    }
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (damageOverTime == false)
        {
            if (collision.tag == "Enemy")
            {
                if (hitSound != SoundEffect.None)
                {
                    SFXManager.instance.PlaySFXPitched(hitSound);
                }

                float finalDamage = (damageAmount * damageAmountMultiplier) * PlayerStats.instance.damageMultiplier;
                collision.GetComponent<EnemyController>().TakeDamage(finalDamage, shouldKnockBack);
                PlayerStats.instance.AddDamageForWeapon(weaponID, finalDamage);
            }
        }
        else
        {
            if (collision.tag == "Enemy")
            {
                enemiesInrange.Add(collision.GetComponent<EnemyController>());
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(damageOverTime == true)
        {
            if (collision.tag == "Enemy")
            {
                enemiesInrange.Remove(collision.GetComponent<EnemyController>());
            }
        }
    }
}
