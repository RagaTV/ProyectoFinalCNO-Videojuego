using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Rigidbody2D rB;
    private Animator anim;
    private SpriteRenderer spriteEnemy;
    private Color originalColor;
    public float moveSpeed;
    public float damageAmount;
    public float hitWaitTime = 1f;
    private float hitCounter;
    private float knockbackForce = 5f;
    private Transform target;
    private PlayerHealthController healthController;
    private GameObject playerObject;
    public float maxHealth = 10f;
    private float currentHealth;
    private float knockBackTime = 0.25f;
    private float knockBackCounter;
    public int expToGive = 1;
    private float auxExtra=1.2f;
    private int levelup=1;


    void OnEnable()
    {
        currentHealth = maxHealth;

        if (anim != null)
        {
            anim.speed = 1;
        }
        rB = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteEnemy = GetComponent<SpriteRenderer>();
        originalColor = spriteEnemy.color;
        playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            target = playerObject.transform;
            healthController = playerObject.GetComponent<PlayerHealthController>();
        }
    }


    void Update()
    {
        float minutes = UIController.instance.gameTimer / 60f;

        for (int i = levelup; i <= minutes; i++)
        {
            float healthPercent = currentHealth / maxHealth;

            maxHealth *= auxExtra;
            damageAmount *= auxExtra;

            // Ajusta la vida actual al mismo porcentaje de la nueva vida máxima
            currentHealth = maxHealth * healthPercent;

            levelup++;
        }

        if (healthController != null && healthController.deathPlayer)
        {
            gameObject.SetActive(false);
        }
    }
    
    void FixedUpdate()
    { 
        if(knockBackCounter > 0)
        {
            knockBackCounter -= Time.fixedDeltaTime;
            if (moveSpeed > 0)
            {
                moveSpeed = -moveSpeed * 1.5f;
            }
            if(knockBackCounter <= 0)
            {
                moveSpeed = Mathf.Abs(moveSpeed * .5f);
            }
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
                hitCounter-=Time.deltaTime;
            }
        }
        else if (target != null) // Si el jugador murió pero el enemigo sigue vivo
        {
            rB.velocity = Vector2.zero;
            anim.speed = 0; // Congela la animación
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
        gameObject.SetActive(false);
        ExperienceLevelController.instance.SpawnExp(transform.position, expToGive);
    }
    else
    {
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

        // new Color(1.5f, 1.5f, 1.5f)

        yield return new WaitForSeconds(0.1f); 

        spriteEnemy.color = originalColor;
    }
}