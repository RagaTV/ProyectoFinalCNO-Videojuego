using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamager : MonoBehaviour
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
    private List<IDamageable> enemiesInrange = new List<IDamageable>();
    public int damageAmountMultiplier = 1;
    public bool destroyOnImpact;
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

                for(int i = enemiesInrange.Count - 1; i >= 0; i--)
                {
                    // Convertimos la interfaz a un MonoBehaviour
                    MonoBehaviour enemyMono = enemiesInrange[i] as MonoBehaviour;

                    // Comprobamos si el MonoBehaviour es válido Y su gameObject está activo
                    if(enemyMono != null && enemyMono.gameObject.activeInHierarchy)
                    {
                        float finalDamage = (damageAmount * damageAmountMultiplier) * PlayerStats.instance.damageMultiplier;
                        enemiesInrange[i].TakeDamage(finalDamage, shouldKnockBack);
                        PlayerStats.instance.AddDamageForWeapon(weaponID, finalDamage);
                    } 
                    else
                    {
                        // Si es nulo O está inactivo, lo quita de la lista
                        enemiesInrange.RemoveAt(i);
                    }
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable damageable = collision.GetComponent<IDamageable>();
        if (damageable != null)
        {
            if (damageOverTime == false)
            {
                // --- Lógica de daño instantáneo ---
                if (hitSound != SoundEffect.None)
                {
                    SFXManager.instance.PlaySFXPitched(hitSound);
                    if (destroyOnImpact == true)
                    {
                        Destroy(gameObject);
                    }
                }

                float finalDamage = (damageAmount * damageAmountMultiplier) * PlayerStats.instance.damageMultiplier;
                damageable.TakeDamage(finalDamage, shouldKnockBack); // ¡Llama a la interfaz!
                PlayerStats.instance.AddDamageForWeapon(weaponID, finalDamage);
            }
            else
            {
                // --- Lógica de daño en el tiempo ---
                if (!enemiesInrange.Contains(damageable))
                {
                    enemiesInrange.Add(damageable); // ¡Añade la interfaz!
                }
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(damageOverTime == true)
        {
            IDamageable damageable = collision.GetComponent<IDamageable>();
            if (damageable != null)
            {
                if (enemiesInrange.Contains(damageable))
                {
                    enemiesInrange.Remove(damageable); // ¡Quita la interfaz!
                }
            }
        }
    }
}
