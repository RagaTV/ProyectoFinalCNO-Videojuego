using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Rigidbody2D rB;
    private Animator anim;
    private SpriteRenderer spriteEnemy;
    private Color originalColor;
    
    // Stats Actuales
    public float moveSpeed;
    public float damageAmount;
    public float maxHealth;
    private float currentHealth;
    
    // Stats Base (Almacenados)
    private float baseSpeed;
    private float baseMaxHealth;
    private float baseDamageAmount;

    public float hitWaitTime = 1f;
    private float hitCounter;
    private float knockbackForce = 5f;
    private Transform target;
    private PlayerHealthController healthController;
    private GameObject playerObject;
    public float coinDropChance = 0.25f; // 25% de probabilidad
    public int coinValue = 1;
    private float knockBackTime = 0.25f;
    private float knockBackCounter;
    public int expToGive = 1;
    
    // Escalado
    private float auxExtra = 1.25f;
    private int levelup;

    void Awake()
    {
        rB = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteEnemy = GetComponent<SpriteRenderer>();
        originalColor = spriteEnemy.color;
    }

    void OnEnable()
    {
        baseMaxHealth = maxHealth;
        currentHealth = baseMaxHealth;
        baseSpeed = moveSpeed;
        baseDamageAmount = damageAmount;
        
        levelup = 1;
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
        if (PlayerController.instance != null)
        {
            target = PlayerController.instance.transform;
            healthController = PlayerController.instance.GetComponent<PlayerHealthController>();
        }
    }

    void Start()
    {
        baseSpeed = moveSpeed;
        baseMaxHealth = maxHealth;
        baseDamageAmount = damageAmount;
    }

    void Update()
    {
        if (healthController.deathPlayer)
        {
            gameObject.SetActive(false);
            return;
        }

        int currentMinute = Mathf.FloorToInt(UIController.instance.gameTimer / 60f);
        if (currentMinute >= levelup)
        {
            float multiplier = Mathf.Pow(auxExtra, currentMinute);
            maxHealth = baseMaxHealth * multiplier;
            damageAmount = baseDamageAmount * multiplier;
            levelup = currentMinute + 1;
        }
    }
    
    void FixedUpdate()
    {
        if (healthController.deathPlayer) return;
        
        if (knockBackCounter > 0)
        {
            knockBackCounter -= Time.fixedDeltaTime;
            rB.velocity = -rB.velocity.normalized * knockbackForce;
            return;
        }

        if (target != null && healthController != null && !healthController.deathPlayer)
        {
            rB.velocity = (target.position - transform.position).normalized * moveSpeed;

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
                    healthController.TakeDamage(damageAmount);
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

            if (Random.value <= coinDropChance)
            {
                CoinController.instance.SpawnCoin(transform.position, coinValue);
            }

            PlayerStats.instance.AddKill();
            gameObject.SetActive(false);
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