using System.Collections;
using UnityEngine;

public class BossFroggerController : BossBase
{
    [Header("Movement")]
    public float moveSpeed = 3f;
    public float stopDistance = 3f; // Distance to stop and attack

    [Header("Attacks")]
    public float damage = 15f;
    
    [Header("Spit Attack")]
    public GameObject spitProjectilePrefab;
    public float spitCooldown = 5f;
    public float spitRange = 15f; // Aumentado para que dispare desde lejos
    public int spitCount = 8; // 8 proyectiles
    public float spitSpread = 90f; // Ángulo más abierto para 8 balas
    private float spitCounter;
    
    [Header("Special Attack")]
    public int specialWaves = 3;
    public int specialProjectilesPerWave = 20; // 360 grados / 16 = ~22 grados
    private float specialAttackCheckTimer = 10f;
    private bool wantsSpecialAttack = false;

    [Header("Tongue Attack")]
    public float tongueCooldown = 3f;
    public float tongueRange = 1.8f; // ¡Muy cerca! (Antes 3f)
    public Vector2 tongueAttackSize = new Vector2(2.5f, 1.5f); // Tamaño del área de golpe
    public float tongueAttackOffset = 1.5f; // Qué tan adelante golpea
    public float tongueAttackOffsetY = 0.5f; // Qué tan arriba golpea
    public float tongueDamageMultiplier = 1.5f;
    private float tongueCounter;

    [Header("Heal Ability")]
    public float healCooldown = 20f;
    public float healAmount = 100f;
    public float healThreshold = 0.5f; // Heal when HP < 50%
    private float healCounter;

    [Header("Components")]
    private Animator anim;
    private Rigidbody2D rb;
    private Transform target;
    private SpriteRenderer spriteRenderer;

    private enum BossState { Idle, Moving, Attacking, Healing }
    private BossState currentState;

    private Color originalSpriteColor = Color.white;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
            originalSpriteColor = Color.white;
        }
    }

    protected override void Start()
    {
        base.Start();
        target = PlayerController.instance.transform;
        
        spitCounter = spitCooldown;
        tongueCounter = tongueCooldown;
        healCounter = healCooldown;

        ChangeState(BossState.Idle);
    }

    void Update()
    {
        if (target == null)
        {
            if (PlayerController.instance != null) target = PlayerController.instance.transform;
            else return;
        }

        // Cooldowns
        if (spitCounter > 0) spitCounter -= Time.deltaTime;
        if (tongueCounter > 0) tongueCounter -= Time.deltaTime;
        if (healCounter > 0) healCounter -= Time.deltaTime;

        // Special Attack Timer (Cada 10s chequea 10% de probabilidad)
        specialAttackCheckTimer -= Time.deltaTime;
        if (specialAttackCheckTimer <= 0)
        {
            specialAttackCheckTimer = 10f;
            if (Random.value <= 0.3f) // 30% de probabilidad
            {
                wantsSpecialAttack = true;
                Debug.Log("¡Boss Frogger prepara ataque especial!");
            }
        }

        // Sprite Flip
        // Sprite Flip (SOLO si no está atacando)
        if (currentHealth > 0 && (currentState == BossState.Moving || currentState == BossState.Idle))
        {
            // INVERTIDO: Si el sprite original mira a la IZQUIERDA, usa esto.
            // Si mira a la DERECHA, invierte los true/false.
            if (target.position.x > transform.position.x) spriteRenderer.flipX = false;
            else spriteRenderer.flipX = true;
        }

        switch (currentState)
        {
            case BossState.Idle:
                // Decide next action
                float distance = Vector2.Distance(transform.position, target.position);
                
                // Priority: Special -> Heal -> Tongue (Very Close) -> Spit (Far) -> Move
                if (wantsSpecialAttack)
                {
                    StartCoroutine(SpecialSpitRoutine());
                }
                else if (currentHealth < maxHealth * healThreshold && healCounter <= 0)
                {
                    StartCoroutine(HealRoutine());
                }
                else if (distance <= tongueRange && tongueCounter <= 0)
                {
                    StartCoroutine(TongueRoutine());
                }
                else if (distance <= spitRange && spitCounter <= 0)
                {
                    StartCoroutine(SpitRoutine());
                }
                else
                {
                    ChangeState(BossState.Moving);
                }
                break;

            case BossState.Moving:
                float distToPlayer = Vector2.Distance(transform.position, target.position);
                
                // Check for attacks while moving
                if (wantsSpecialAttack)
                {
                    StartCoroutine(SpecialSpitRoutine());
                    break;
                }
                if (currentHealth < maxHealth * healThreshold && healCounter <= 0)
                {
                    StartCoroutine(HealRoutine());
                    break;
                }
                if (distToPlayer <= tongueRange && tongueCounter <= 0)
                {
                    StartCoroutine(TongueRoutine());
                    break;
                }
                if (distToPlayer <= spitRange && spitCounter <= 0 && distToPlayer > tongueRange) // Keep distance for spit
                {
                    StartCoroutine(SpitRoutine());
                    break;
                }

                MoveTowardsPlayer();
                break;

            case BossState.Attacking:
            case BossState.Healing:
                rb.velocity = Vector2.zero; // Stop while acting
                break;
        }
    }

    void MoveTowardsPlayer()
    {
        Vector2 dirToPlayer = (target.position - transform.position).normalized;
        
        // Obstacle Avoidance (Simplified from Gollux)
        Vector2 checkPos = (Vector2)transform.position + dirToPlayer * 0.5f;
        Collider2D obstacle = Physics2D.OverlapCircle(checkPos, 0.5f);
        
        bool shouldAvoid = false;
        if (obstacle != null && obstacle.gameObject != this.gameObject)
        {
            bool isPlayer = obstacle.gameObject.CompareTag("Player");
            bool isEnemy = obstacle.gameObject.CompareTag("Enemy");
            bool isBoss = obstacle.gameObject.CompareTag("Boss");
            bool isPickup = obstacle.GetComponent<CoinPickup>() != null || obstacle.GetComponent<ExpPickup>() != null;
            
            shouldAvoid = !isPlayer && !isEnemy && !isBoss && !isPickup;
        }

        if (shouldAvoid)
        {
            Vector2 perp = Vector2.Perpendicular(dirToPlayer);
            rb.velocity = perp * moveSpeed * 0.8f;
        }
        else
        {
            rb.velocity = dirToPlayer * moveSpeed;
        }

        anim.SetBool("IsMoving", rb.velocity.magnitude > 0.1f);
    }

    IEnumerator SpitRoutine()
    {
        ChangeState(BossState.Attacking);
        rb.velocity = Vector2.zero; // ¡Frenar en seco!
        anim.SetBool("IsMoving", false);
        anim.SetTrigger("Spit");

        yield return new WaitForSeconds(0.5f); // Wait for animation point

        if (spitProjectilePrefab != null)
        {
            // OLA 1
            FireSpitWave();
            
            yield return new WaitForSeconds(0.3f); // Pequeña pausa entre olas
            
            // OLA 2
            FireSpitWave();
        }

        yield return new WaitForSeconds(1f); // Recover
        spitCounter = Random.Range(2f, 10f); // Delay random entre 2 y 10 segundos
        ChangeState(BossState.Idle);
        anim.Play("Idle"); // Force animation reset
    }

    void FireSpitWave()
    {
        // Dirección base hacia el jugador
        Vector2 baseDir = (target.position - transform.position).normalized;
        
        // Calcular ángulo inicial (mitad del spread hacia un lado)
        float angleStep = spitCount > 1 ? spitSpread / (spitCount - 1) : 0f;
        float startAngle = -spitSpread / 2f;

        for (int i = 0; i < spitCount; i++)
        {
            // Calcular dirección rotada
            float currentAngle = startAngle + (angleStep * i);
            Vector2 projectileDir = Quaternion.Euler(0, 0, currentAngle) * baseDir;

            GameObject proj = Instantiate(spitProjectilePrefab, transform.position, Quaternion.identity);
            BossProjectile bp = proj.GetComponent<BossProjectile>();
            if (bp != null)
            {
                bp.damage = damage;
                bp.SetDirection(projectileDir);
            }
        }
    }

    IEnumerator SpecialSpitRoutine()
    {
        wantsSpecialAttack = false; // Consumimos el flag
        ChangeState(BossState.Attacking);
        rb.velocity = Vector2.zero;
        anim.SetBool("IsMoving", false);
        
        // Usamos la animación de Idle como pediste
        anim.Play("Idle"); 

        yield return new WaitForSeconds(0.5f); // Preparación

        for (int w = 0; w < specialWaves; w++)
        {
            // Disparar en 360 grados
            float angleStep = 360f / specialProjectilesPerWave;
            
            // Offset de ángulo para que cada ola sea un poco diferente (opcional)
            float angleOffset = w * (angleStep / 2f); 

            for (int i = 0; i < specialProjectilesPerWave; i++)
            {
                float currentAngle = angleOffset + (angleStep * i);
                Vector2 dir = Quaternion.Euler(0, 0, currentAngle) * Vector2.right; // Vector base derecha rotado

                if (spitProjectilePrefab != null)
                {
                    GameObject proj = Instantiate(spitProjectilePrefab, transform.position, Quaternion.identity);
                    BossProjectile bp = proj.GetComponent<BossProjectile>();
                    if (bp != null)
                    {
                        bp.damage = damage;
                        bp.SetDirection(dir);
                    }
                }
            }
            
            // Pausa entre olas
            yield return new WaitForSeconds(0.8f);
        }

        yield return new WaitForSeconds(1f); // Recover
        ChangeState(BossState.Idle);
        anim.Play("Idle");
    }

    IEnumerator TongueRoutine()
    {
        ChangeState(BossState.Attacking);
        rb.velocity = Vector2.zero; // ¡Frenar en seco!
        anim.SetBool("IsMoving", false);
        anim.SetTrigger("Tongue");

        yield return new WaitForSeconds(0.4f); // Wait for hit frame

        // Check distance again
        // USAMOS UNA CAJA (Hitbox) en lugar de distancia simple
        // Esto permite ver el área de golpe en el editor
        
        Vector2 center = transform.position;
        // Si mira a la derecha (flipX false), el offset es positivo. Si mira a la izquierda, negativo.
        float directionMultiplier = spriteRenderer.flipX ? -1f : 1f;
        Vector2 hitCenter = center + new Vector2(tongueAttackOffset * directionMultiplier, tongueAttackOffsetY);
        
        Collider2D hitPlayer = Physics2D.OverlapBox(hitCenter, tongueAttackSize, 0f, LayerMask.GetMask("Player")); // Asegúrate que el Player esté en la capa "Player" o usa LayerMask por defecto
        
        // Si no usas capas, usa esto:
        if (hitPlayer == null)
        {
             Collider2D[] hits = Physics2D.OverlapBoxAll(hitCenter, tongueAttackSize, 0f);
             foreach(var hit in hits)
             {
                 if(hit.CompareTag("Player"))
                 {
                     hitPlayer = hit;
                     break;
                 }
             }
        }

        if (hitPlayer != null)
        {
            if (PlayerHealthController.instance != null)
            {
                PlayerHealthController.instance.TakeDamage(damage * tongueDamageMultiplier);
            }
        }

        yield return new WaitForSeconds(0.6f); // Recover
        tongueCounter = tongueCooldown;
        ChangeState(BossState.Idle);
        anim.Play("Idle"); // Force animation reset
    }

    IEnumerator HealRoutine()
    {
        ChangeState(BossState.Healing);
        rb.velocity = Vector2.zero; // ¡Frenar en seco!
        anim.SetBool("IsMoving", false);
        anim.SetTrigger("Heal");

        yield return new WaitForSeconds(0.5f);

        currentHealth += healAmount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        
        // Visual feedback for heal could go here (particles, green flash)
        DamageNumberController.instance.SpawnDamage(-healAmount, transform.position); // Negative damage = heal number? Or just spawn green text if supported.

        yield return new WaitForSeconds(1f);
        healCounter = healCooldown;
        ChangeState(BossState.Idle);
        anim.Play("Idle"); // Force animation reset
    }

    void ChangeState(BossState newState)
    {
        currentState = newState;
    }

    public override void TakeDamage(float damageToTake)
    {
        base.TakeDamage(damageToTake);
        
        if (currentHealth > 0)
        {
            StartCoroutine(FlashDamage());
        }
        
        DamageNumberController.instance.SpawnDamage(damageToTake, transform.position);
    }

    private IEnumerator FlashDamage()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f); 
        spriteRenderer.color = originalSpriteColor;
    }

    // DIBUJAR EL GIZMO EN EL EDITOR PARA VER EL RANGO
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stopDistance);

        Gizmos.color = Color.red;
        // Calcular el centro del hitbox basado en hacia dónde miraría por defecto (derecha)
        // Nota: En el editor estático no sabemos el flip, así que dibujamos ambos o asumimos derecha
        Vector2 center = transform.position;
        Vector2 rightCenter = center + new Vector2(tongueAttackOffset, tongueAttackOffsetY);
        Gizmos.DrawWireCube(rightCenter, tongueAttackSize);
    }
}
