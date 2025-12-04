using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAppleCatController : BossBase
{
    [Header("Movement")]
    public float moveSpeed = 4f;
    public float runAnimationSpeed = 1f;

    [Header("Spin Attack")]
    public float spinDuration = 3f;
    public float spinSpeedMultiplier = 1.5f; // Se mueve más rápido al girar
    public float spinDamage = 15f;
    public float spinDamageInterval = 0.5f; // Intervalo de daño
    public float spinCooldown = 8f;
    private float spinCounter;
    private float spinDamageTimer;
    private bool isSpinning = false;

    [Header("Shoot Attack")]
    public GameObject gunProjectilePrefab;
    public Transform firePoint; // Asigna un hijo vacío donde sale la bala
    public float shootCooldown = 5f;
    public float shootDuration = 1.5f; // Tiempo que se queda quieto disparando
    public float shootAnimDelay = 0.1f; // TIEMPO EXACTO del disparo (Frame / SampleRate)
    public SoundEffect shootSound; // Sonido de disparo
    private float shootCounter;
    private Coroutine activeSpinCoroutine;

    [Header("Special Attack")]
    public float specialAttackChance = 0.3f; // 30%
    public float specialAttackCheckInterval = 10f;
    public float spiralDuration = 4f;
    public float spiralShootRate = 0.1f;
    public float spiralRotationSpeed = 30f; // Velocidad de rotación del patrón de disparo
    private float specialAttackTimer;

    [Header("Components")]
    private Animator anim;
    private Rigidbody2D rb;
    private Transform target;
    private SpriteRenderer spriteRenderer;

    private enum BossState { Running, Spinning, Shooting, SpecialSpinning }
    private BossState currentState;

    private Color originalSpriteColor = Color.white;

    [Header("Muerte y Loot")]
    public float deathFadeDuration = 1.5f; 
    
    [Header("Partículas de Muerte")]
    public GameObject deathParticlePrefab; 
    public bool useProceduralParticles = true; 
    public Color particleColor1 = new Color(1f, 0.3f, 0f); 
    public Color particleColor2 = new Color(0.8f, 0f, 0f); 
    public int particleCount = 30; 

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
        
        spinCounter = spinCooldown;
        shootCounter = shootCooldown;
        specialAttackTimer = specialAttackCheckInterval;

        ChangeState(BossState.Running);
    }

    void Update()
    {
        if (target == null)
        {
            if (PlayerController.instance != null) target = PlayerController.instance.transform;
            else return;
        }

        // Cooldowns
        if (spinCounter > 0) spinCounter -= Time.deltaTime;
        if (shootCounter > 0) shootCounter -= Time.deltaTime;
        if (spinDamageTimer > 0) spinDamageTimer -= Time.deltaTime;
        
        // Special Attack Timer
        if (specialAttackTimer > 0) specialAttackTimer -= Time.deltaTime;

        // Voltear Sprite
        if (currentHealth > 0 && !isSpinning && currentState != BossState.SpecialSpinning)
        {
            if (target.position.x > transform.position.x) spriteRenderer.flipX = true;
            else spriteRenderer.flipX = false;
        }

        switch (currentState)
        {
            case BossState.Running:
                HandleRunningState();
                break;

            case BossState.Spinning:
                HandleSpinningState();
                break;

            case BossState.Shooting:
                // Quieto mientras dispara
                rb.velocity = Vector2.zero;
                break;

            case BossState.SpecialSpinning:
                // Quieto o movimiento lento mientras hace el especial
                rb.velocity = Vector2.zero; 
                break;
        }
    }

    void HandleRunningState()
    {
        // Chequeo de ataque especial
        if (specialAttackTimer <= 0)
        {
            specialAttackTimer = specialAttackCheckInterval;
            if (Random.value <= specialAttackChance)
            {
                StartCoroutine(SpecialSpinRoutine());
                return;
            }
        }

        // Siempre corre hacia el jugador
        Vector2 dir = (target.position - transform.position).normalized;
        
        // Obstacle Avoidance
        Vector2 checkPos = (Vector2)transform.position + dir * 0.5f;
        Collider2D obstacle = Physics2D.OverlapCircle(checkPos, 0.5f);
        
        bool shouldAvoid = false;
        if (obstacle != null && obstacle.gameObject != this.gameObject)
        {
            bool isPlayer = obstacle.gameObject.CompareTag("Player");
            bool isEnemy = obstacle.gameObject.CompareTag("Enemy");
            bool isBoss = obstacle.gameObject.CompareTag("Boss");
            bool isWeapon = obstacle.GetComponent<EnemyDamager>() != null;
            bool isPickup = obstacle.GetComponent<CoinPickup>() != null || obstacle.GetComponent<ExpPickup>() != null;
            
            shouldAvoid = !isPlayer && !isEnemy && !isBoss && !isWeapon && !isPickup;
        }

        if (shouldAvoid)
        {
            Vector2 perp = Vector2.Perpendicular(dir);
            rb.velocity = perp * moveSpeed * 0.8f;
        }
        else
        {
            rb.velocity = dir * moveSpeed;
        }
        
        // Animación de correr siempre activa
        anim.Play("Running"); 

        // Decisiones de ataque
        float dist = Vector2.Distance(transform.position, target.position);

        // Prioridad: Spin (Cerca/Medio) -> Shoot (Lejos)
        if (spinCounter <= 0 && dist < 8f)
        {
            activeSpinCoroutine = StartCoroutine(SpinRoutine());
        }
        else if (shootCounter <= 0)
        {
            StartCoroutine(ShootRoutine());
        }
    }

    void HandleSpinningState()
    {
        // Perseguir con inercia
        Vector2 dirToTarget = (target.position - transform.position).normalized;
        
        // Lerp de la velocidad actual hacia la dirección deseada
        // El factor 2f determina qué tan rápido gira. Menor valor = más inercia.
        Vector2 newVelocity = Vector2.Lerp(rb.velocity, dirToTarget * (moveSpeed * spinSpeedMultiplier), 2f * Time.deltaTime);
        
        rb.velocity = newVelocity;
    }

    IEnumerator SpinRoutine()
    {
        ChangeState(BossState.Spinning);
        isSpinning = true;
        anim.Play("Spinning"); // Asegúrate de tener esta animación

        float timer = 0;
        while (timer < spinDuration)
        {
            timer += Time.deltaTime;
            // Rotación visual
            // transform.Rotate(Vector3.forward * 720 * Time.deltaTime); 
            yield return null;
        }

        // transform.rotation = Quaternion.identity; // Reset rotación
        isSpinning = false;
        spinCounter = spinCooldown;
        ChangeState(BossState.Running);
    }

    IEnumerator ShootRoutine()
    {
        ChangeState(BossState.Shooting);
        anim.Play("Shooting"); // Asegúrate de tener esta animación

        yield return new WaitForSeconds(shootAnimDelay); // Tiempo exacto para el frame del disparo

        if (gunProjectilePrefab != null)
        {
            // Reproducir sonido
            if (SFXManager.instance != null)
            {
                SFXManager.instance.PlaySFX(shootSound);
            }

            // Usar firePoint si existe, si no, usar la posición del boss
            Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
            
            GameObject proj = Instantiate(gunProjectilePrefab, spawnPos, Quaternion.identity);
            BossGunProjectile bgp = proj.GetComponent<BossGunProjectile>();
            if (bgp != null)
            {
                bgp.SetDirection(target.position - transform.position);
            }
        }

        yield return new WaitForSeconds(shootDuration - 0.5f); // Resto de la animación

        shootCounter = Random.Range(1f, 3f); // Random cooldown entre 1 y 3 segundos
        ChangeState(BossState.Running);
    }

    IEnumerator SpecialSpinRoutine()
    {
        ChangeState(BossState.SpecialSpinning);
        
        // Esperar 1 segundo
        yield return new WaitForSeconds(1f);

        isSpinning = true; // Activar daño por contacto
        anim.Play("Spinning"); // Reusamos la animación de giro

        float timer = 0f;
        float currentAngle = 0f;
        float nextShootTime = 0f;

        while (timer < spiralDuration)
        {
            timer += Time.deltaTime;
            
            // Rotar el ángulo de disparo
            currentAngle += spiralRotationSpeed * Time.deltaTime; // Gira el patrón

            if (Time.time >= nextShootTime)
            {
                nextShootTime = Time.time + spiralShootRate;

                if (gunProjectilePrefab != null)
                {
                    // Reproducir sonido
                    // if (SFXManager.instance != null) SFXManager.instance.PlaySFX(shootSound);

                    Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
                    
                    // Disparar en la dirección del ángulo actual
                    Vector2 dir = Quaternion.Euler(0, 0, currentAngle) * Vector2.right;

                    GameObject proj = Instantiate(gunProjectilePrefab, spawnPos, Quaternion.identity);
                    BossGunProjectile bgp = proj.GetComponent<BossGunProjectile>();
                    if (bgp != null)
                    {
                        bgp.SetDirection(dir);
                    }
                }
            }

            yield return null;
        }

        isSpinning = false;
        ChangeState(BossState.Running);
    }

    // Daño por contacto (Solo durante Spin o siempre si quieres)
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Si está girando, hace daño
            if (isSpinning)
            {
                if (spinDamageTimer <= 0)
                {
                    if (PlayerHealthController.instance != null)
                    {
                        PlayerHealthController.instance.TakeDamage(spinDamage);
                        spinDamageTimer = spinDamageInterval;
                        
                        // Detener el giro al golpear
                        if (activeSpinCoroutine != null) StopCoroutine(activeSpinCoroutine);
                        isSpinning = false;
                        spinCounter = spinCooldown;
                        ChangeState(BossState.Running);
                        anim.Play("Running");
                    }
                }
            }
            // Opcional: Daño por contacto normal también?
            // else { ... }
        }
    }

    void ChangeState(BossState newState)
    {
        currentState = newState;
    }

    public override void TakeDamage(float damageToTake)
    {
        base.TakeDamage(damageToTake);
        if (currentHealth > 0) StartCoroutine(FlashDamage());
        DamageNumberController.instance.SpawnDamage(damageToTake, transform.position);
    }

    private IEnumerator FlashDamage()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalSpriteColor;
    }
    protected override void Die()
    {
        GetComponent<Collider2D>().enabled = false;
        rb.velocity = Vector2.zero;
        this.enabled = false; 

        Debug.Log("¡El Boss ha sido derrotado!");

        if (StoryManager.instance != null)
        {
            StoryManager.instance.OnBossDefeated(gameObject.name);
        }

        SpawnDeathParticles();
        StartCoroutine(DeathFadeOut());
    }
    
    void SpawnDeathParticles()
    {
        if (deathParticlePrefab != null)
        {
            GameObject particles = Instantiate(deathParticlePrefab, transform.position, Quaternion.identity);
            Destroy(particles, 3f); 
            return;
        }
        
        if (useProceduralParticles)
        {
            CreateProceduralParticles();
        }
    }
    
    void CreateProceduralParticles()
    {
        GameObject particleObj = new GameObject("BossDeathParticles");
        particleObj.transform.position = transform.position;
        
        ParticleSystem ps = particleObj.AddComponent<ParticleSystem>();
        
        var main = ps.main;
        main.startLifetime = 1.5f; 
        main.startSpeed = 5f; 
        main.startSize = 0.3f; 
        main.maxParticles = particleCount;
        main.simulationSpace = ParticleSystemSimulationSpace.World; 
        
        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { 
                new GradientColorKey(particleColor1, 0.0f),  
                new GradientColorKey(particleColor2, 0.5f),  
                new GradientColorKey(Color.black, 1.0f)      
            },
            new GradientAlphaKey[] { 
                new GradientAlphaKey(1.0f, 0.0f),   
                new GradientAlphaKey(0.8f, 0.5f),   
                new GradientAlphaKey(0.0f, 1.0f)    
            }
        );
        colorOverLifetime.color = gradient;
        
        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(0.0f, 1.0f);  
        curve.AddKey(1.0f, 0.0f);  
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, curve);
        
        var emission = ps.emission;
        emission.enabled = true;
        emission.rateOverTime = 0; 
        
        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.5f; 
        
        var forceOverLifetime = ps.forceOverLifetime;
        forceOverLifetime.enabled = true;
        forceOverLifetime.y = -2f; 
        
        ps.Emit(particleCount);
        
        Destroy(particleObj, main.startLifetime.constant + 0.5f);
    }
    
    private IEnumerator DeathFadeOut()
    {
        float elapsed = 0f;
        Color startColor = spriteRenderer.color;
        
        while (elapsed < deathFadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / deathFadeDuration;
            
            Color currentColor = Color.Lerp(startColor, Color.black, t);
            currentColor.a = Mathf.Lerp(1f, 0f, t);
            
            spriteRenderer.color = currentColor;
            
            yield return null;
        }
        
        spriteRenderer.color = new Color(0, 0, 0, 0);
        Destroy(gameObject);
    }
}
