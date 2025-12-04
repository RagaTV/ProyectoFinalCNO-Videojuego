using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour, IDamageable
{
    public Rigidbody2D rB;
    private Animator anim;
    private SpriteRenderer spriteEnemy;
    private Color originalColor;
    
    // --- Estructura de Stats ---
    [Header("Stats Base")]
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

    // Stats Actuales
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
            float healthRate = 0.25f; // 25% por minuto (balanceado)
            float damageRate = 0.2f; // 20% por minuto

            // Calcula multiplicadores
            float healthMultiplier = 1f + (currentMinute * healthRate);
            float damageMultiplier = 1f + (currentMinute * damageRate);
            
            float oldMaxHealth = currentMaxHealth; // Guardamos la vida máxima anterior

            currentMaxHealth = baseMaxHealth * healthMultiplier;
            currentDamageAmount = baseDamageAmount * damageMultiplier;

            // --- Escalar Vida Actual ---
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
            // --- Detección de Obstáculos ---
            Vector2 dirToPlayer = (target.position - transform.position).normalized;
            
            // Chequea si hay algo adelante
            Vector2 checkPos = (Vector2)transform.position + dirToPlayer * 0.5f;
            Collider2D obstacle = Physics2D.OverlapCircle(checkPos, 0.3f);
            
            // Si hay obstáculo, verifica si debe esquivarlo
            bool shouldAvoid = false;
            if (obstacle != null && obstacle.gameObject != this.gameObject) // Ignora su propio collider
            {
                // NO esquivar si es:
                // - El jugador
                // - Otro enemigo
                // - El boss
                // - Un arma (tiene componente EnemyDamager)
                // - Un pickup (monedas o experiencia)
                bool isPlayer = obstacle.gameObject.tag == "Player";
                bool isEnemy = obstacle.gameObject.tag == "Enemy";
                bool isBoss = obstacle.gameObject.tag == "Boss";
                bool isWeapon = obstacle.GetComponent<EnemyDamager>() != null;
                bool isPickup = obstacle.GetComponent<CoinPickup>() != null || obstacle.GetComponent<ExpPickup>() != null;
                
                shouldAvoid = !isPlayer && !isEnemy && !isBoss && !isWeapon && !isPickup;
            }
            
            // Si debe esquivar, mueve perpendicular
            if (shouldAvoid)
            {
                Vector2 perp = Vector2.Perpendicular(dirToPlayer);
                rB.velocity = perp * currentMoveSpeed * 0.7f; // Más lento al rodear
            }
            else
            {
                rB.velocity = dirToPlayer * currentMoveSpeed;
            }

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
                    // --- Usar Daño Escalado ---
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
        DamageNumberController.instance.SpawnDamage(damageToTake, transform.position); 
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
