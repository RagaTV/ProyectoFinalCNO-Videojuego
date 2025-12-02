using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour, IDamageable
{
    public Rigidbody2D rB;
    private Animator anim;
    private SpriteRenderer spriteEnemy;
    private Color originalColor;
    
    // --- NUEVA ESTRUCTURA DE STATS ---
    [Header("Stats Base (del Inspector)")]
    public float baseMoveSpeed = 3f;
    public float baseDamageAmount = 1f;
    public float baseMaxHealth = 2f;
    public float coinDropChance = 0.25f;
    public int coinValue = 1;
    public int expToGive = 1;

    [Header("Stats de Combate")]
    public float hitWaitTime = 1f;
    private float hitCounter;
    private float knockbackForce = 5f;
    private float knockBackTime = 0.25f;
    private float knockBackCounter;

    // "Stats Actuales" que se escalan y se usan en combate
    private float currentMoveSpeed;
    private float currentDamageAmount;
    private float currentMaxHealth;
    private float currentHealth; // Vida actual (de 0 a currentMaxHealth)
    // ------------------------------------

    private Transform target;
    private PlayerHealthController healthController;
    private GameObject playerObject;
    
    // Escalado
    private int levelup;

    void Awake()
    {
        rB = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteEnemy = GetComponent<SpriteRenderer>();
        originalColor = spriteEnemy.color;

        // Encontrar al jugador solo una vez si es posible
        playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            target = playerObject.transform;
            healthController = playerObject.GetComponent<PlayerHealthController>();
        }
    }

    // OnEnable se llama CADA VEZ que sale del Object Pool
    void OnEnable()
    {
        // Las stats actuales se igualan a las stats base
        currentMoveSpeed = baseMoveSpeed;
        currentDamageAmount = baseDamageAmount;
        currentMaxHealth = baseMaxHealth;
        currentHealth = currentMaxHealth; // Llenar la barra de vida
        // ------------------------------------------------

        levelup = 1; // Para que el escalado se recalcule
        knockBackCounter = 0;
        hitCounter = 0; 

        if (rB != null)
        {
            rB.velocity = Vector2.zero;
        }
        if (anim != null)
        {
            anim.speed = 1;
        }
        
        // Es más seguro verificar al jugador aquí también, por si acaso
        if (target == null && PlayerController.instance != null)
        {
            target = PlayerController.instance.transform;
            healthController = PlayerController.instance.GetComponent<PlayerHealthController>();
        }
    }

    void Update()
    {
        if (healthController == null || healthController.deathPlayer)
        {
            gameObject.SetActive(false); // Devuelve al pool si el jugador muere
            return;
        }

        int currentMinute = Mathf.FloorToInt(UIController.instance.gameTimer / 60f);
        if (currentMinute >= levelup)
        {
            // Tasas de escalado separadas 
            float healthRate = 0.6f; // 60% por minuto (para que sea noticeable)
            float damageRate = 0.2f; // 20% por minuto

            // Calcula multiplicadores
            float healthMultiplier = 1f + (currentMinute * healthRate);
            float damageMultiplier = 1f + (currentMinute * damageRate);
            
            float oldMaxHealth = currentMaxHealth; // Guardamos la vida máxima anterior

            currentMaxHealth = baseMaxHealth * healthMultiplier;
            currentDamageAmount = baseDamageAmount * damageMultiplier;

            // --- CORRECCIÓN CLAVE: ESCALAR LA VIDA ACTUAL ---
            // Asegura que la vida actual crezca en proporción a la vida máxima para que no muera de un golpe
            float maxHealthIncrease = currentMaxHealth - oldMaxHealth;
            currentHealth += maxHealthIncrease;
            // ------------------------------------------------

            levelup = currentMinute + 1;
        }
    }
    
    void FixedUpdate()
    {
        if (healthController == null || healthController.deathPlayer) return;
        
        if (knockBackCounter > 0)
        {
            knockBackCounter -= Time.fixedDeltaTime;
            rB.velocity = -rB.velocity.normalized * knockbackForce;
            return;
        }

        if (target != null)
        {
            // --- Mantenemos el uso de STATS ACTUALES ---
            rB.velocity = (target.position - transform.position).normalized * currentMoveSpeed;

            // ... (lógica de voltear el sprite) ...
            Vector3 spriteOrientation = transform.localScale;
            if (target.position.x > transform.position.x)
                spriteOrientation.x = Mathf.Abs(spriteOrientation.x);
            else if (target.position.x < transform.position.x)
                spriteOrientation.x = -Mathf.Abs(spriteOrientation.x);
            transform.localScale = spriteOrientation;


            if(hitCounter > 0){
                hitCounter-=Time.fixedDeltaTime;
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (hitCounter <= 0)
            {
                if (healthController != null && !healthController.deathPlayer)
                {
                    // --- USA EL DAÑO ESCALADO ---
                    healthController.TakeDamage(currentDamageAmount);
                    
                    // ... (lógica de knockback) ...
                    Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                    if (playerRb != null)
                    {
                        Vector2 knockbackDirection = (playerRb.transform.position - transform.position).normalized;
                        playerRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
                    }

                    hitCounter = hitWaitTime;
                }
            }
        }
    }

    // Esta función ya estaba bien, porque usa "currentHealth"
    public void TakeDamage(float damageToTake)
    {
        if (currentHealth <= 0)
        {
            return;
        }
        currentHealth -= damageToTake;
        if (currentHealth <= 0)
        {
            spriteEnemy.color = originalColor;
            if (ExperienceLevelController.instance != null)
            {
                ExperienceLevelController.instance.SpawnExp(transform.position, expToGive);
            }

            float finalCoinDropChance = coinDropChance * PlayerStats.instance.luck;
            if (Random.value <= Mathf.Min(finalCoinDropChance, 1f))
            {
                CoinController.instance.SpawnCoin(transform.position, coinValue);
            }

            PlayerStats.instance.AddKill();
            gameObject.SetActive(false); // Devuelve al pool
        } else {
            StartCoroutine(FlashDamage());
        }
        // DamageNumberController.instance.SpawnDamage(damageToTake, transform.position); // Asumo que esto existe
    }

    public void TakeDamage(float damageToTake, bool shouldKnockBack)
    {
        TakeDamage(damageToTake);
        if (shouldKnockBack)
        {
            knockBackCounter = knockBackTime;
        }
    }
    
    private IEnumerator FlashDamage()
    {
        spriteEnemy.color = Color.red;
        yield return new WaitForSeconds(0.1f); 
        spriteEnemy.color = originalColor;
    }
}