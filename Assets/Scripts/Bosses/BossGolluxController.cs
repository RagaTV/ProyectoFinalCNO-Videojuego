using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGolluxController : BossBase
{
    // [Header("Stats Base")] -> Heredado de BossBase
    // public float maxHealth = 1000f; -> Inherited
    // [HideInInspector]
    // public float currentHealth; -> Inherited
    
    public float moveSpeed = 2f;
    public float damage = 10f;
    
    [Header("Ataque Básico (Hit)")]
    public float hitWaitTime = 1.5f; 
    private float hitCounter;
    
    // --- VARIABLES PARA LA CARRERA ---
    [Header("Ataque de Carrera (Dash)")]
    public float dashOvershoot = 3f;
    public float dashSpeed = 8f;
    public float dashAnimationSpeed = 2.5f; // Multiplicador de la anim "Move"
    public float dashDamageMultiplier = 2f; // Doble daño
    public float dashCooldown = 10f; // Tiempo entre carreras
    public float dashMinDistance = 4f; // Distancia mín. para usarla
    public float dashTimeout = 3f; // Tiempo máximo de dash antes de auto-cancelar
    private float dashCooldownCounter;
    private float dashTimer; // Temporizador para detectar dash atascado
    private Vector2 dashTargetPosition; // Dónde estaba el jugador
    private bool isDashing = false;
    // ------------------------------------------
    [Header("Ajustes de Ataque")]
    public float attackDuration = 1.0f;
    [Header("Componentes")]
    private Animator anim;
    private Rigidbody2D rb;
    private Transform target;
    
    // FSM
    // Boss es imparable
    private enum BossState { Idle, Moving, Attacking }
    private BossState currentState;
    private Color originalSpriteColor = Color.white; // Guardamos el color original
    private SpriteRenderer spriteRenderer;
    [Header("Muerte y Loot")]
    //public GameObject lootPrefab; 
    public float deathFadeDuration = 1.5f; // Duración del fade-out al morir
    
    [Header("Partículas de Muerte")]
    [Tooltip("OPCIONAL: Arrastra un prefab de partículas aquí. Si está vacío, se crearán partículas por código.")]
    public GameObject deathParticlePrefab; // Prefab de partículas (opcional)
    public bool useProceduralParticles = true; // Si no hay prefab, crear partículas por código
    public Color particleColor1 = new Color(1f, 0.3f, 0f); // Naranja
    public Color particleColor2 = new Color(0.8f, 0f, 0f); // Rojo oscuro
    public int particleCount = 30; // Cantidad de partículas
    
    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Fuerza el color a blanco ANTES que cualquier otra cosa
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
        
        hitCounter = 0;
        dashCooldownCounter = dashCooldown; // Empieza en cooldown
        dashTimer = 0f;
        
        ChangeState(BossState.Idle); 
    }

    void Update()
    {
        if (target == null)
    {
        if (PlayerController.instance != null)
        {
            target = PlayerController.instance.transform;
        }
        else
        {
            // Aún no encuentra al jugador, no hagas nada.
            return; 
        }
    }
        
        // --- Manejo de Cooldowns ---
        if (hitCounter > 0) hitCounter -= Time.deltaTime;
        if (dashCooldownCounter > 0) dashCooldownCounter -= Time.deltaTime;

        // Voltea el sprite según la dirección (incluso durante el dash)
        if (currentHealth > 0)
        {
            if (target.position.x > transform.position.x)
            {
                spriteRenderer.flipX = false; // Sprite original mira a la DERECHA
            }
            else
            {
                spriteRenderer.flipX = true; // Lo volteamos
            }
        }
        if (currentState == BossState.Moving && 
            !isDashing &&
            dashCooldownCounter <= 0 &&
            Vector2.Distance(transform.position, target.position) >= dashMinDistance)
        {
            StartDash();
        }

        // --- Lógica de la Máquina de Estados (FSM) ---
        switch (currentState)
        {
            case BossState.Idle:
                ChangeState(BossState.Moving);
                break;

            case BossState.Moving:
                
                // 1. Lógica de Dash
                if (isDashing)
                {
                    // Incrementa el temporizador del dash
                    dashTimer += Time.deltaTime;
                    
                    // TIMEOUT: Si el dash tarda demasiado, cáncelalo
                    if (dashTimer >= dashTimeout)
                    {
                        Debug.Log("Dash timeout - Boss estaba atascado");
                        EndDash();
                    }
                    // Comprueba si ya llegó al destino
                    else if (Vector2.Distance(transform.position, dashTargetPosition) < 0.5f)
                    {
                        EndDash();
                    }
                }
                // 2. Lógica de Pausa (Cooldown)
                else if (hitCounter > 0) 
                {
                    rb.velocity = Vector2.zero; // Pausa post-ataque básico
                }
                // 3. Lógica de Movimiento Normal
                else
                {
                    // --- DETECCIÓN DE OBSTÁCULOS ---
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
                        rb.velocity = perp * moveSpeed * 0.7f; // Más lento al rodear
                    }
                    else
                    {
                        rb.velocity = dirToPlayer * moveSpeed;
                    }
                }

                anim.SetBool("IsMoving", rb.velocity.magnitude > 0.1f);
                break;

            case BossState.Attacking:
                // Pausado, esperando evento de animación 'Hit'
                rb.velocity = Vector2.zero;
                anim.SetBool("IsMoving", false);
                break;
        }
    }
    
    // Detecta colisiones durante el dash (para evitar quedarse atascado en paredes)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDashing)
        {
            // Ignorar colisiones con:
            // - Jugador (porque es el objetivo)
            // - Enemigos (para no detenerse si choca con ellos)
            // - Pickups (monedas/exp)
            // - Armas (si tienen collider físico)
            bool isPlayer = collision.gameObject.tag == "Player";
            bool isEnemy = collision.gameObject.tag == "Enemy";
            bool isPickup = collision.gameObject.GetComponent<CoinPickup>() != null || collision.gameObject.GetComponent<ExpPickup>() != null;
            bool isWeapon = collision.gameObject.GetComponent<EnemyDamager>() != null;

            // Si choca con algo que NO es ninguno de los anteriores (ej: pared), termina el dash
            if (!isPlayer && !isEnemy && !isPickup && !isWeapon)
            {
                Debug.Log("Dash interrumpido por colisión con: " + collision.gameObject.name);
                EndDash();
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag != "Player") return;

        // --- LÓGICA DE GOLPE DE CARRERA ---
        if (isDashing)
        {
            float finalDashDamage = damage * dashDamageMultiplier;
            // Asegúrate de que PlayerHealthController tenga instancia válida
            if(PlayerHealthController.instance != null) {
                PlayerHealthController.instance.TakeDamage(finalDashDamage);
            }
            EndDash(); 
        }
        // --- LÓGICA DE GOLPE BÁSICO ---
        // Verificamos que esté moviéndose Y que el cooldown haya terminado
        else if (currentState == BossState.Moving && hitCounter <= 0)
        {
            // Iniciamos la rutina segura
            StartCoroutine(AttackRoutine());
        }
    }


    IEnumerator AttackRoutine()
    {
        ChangeState(BossState.Attacking);
        anim.SetTrigger("Hit"); // Inicia el golpe

        yield return new WaitForSeconds(attackDuration);

        hitCounter = hitWaitTime;
        ChangeState(BossState.Moving);
        
        // Le decimos al Animator "Ponte a caminar"

        anim.Play("Move");
    }

        
    void StartDash()
    {
        isDashing = true;
        dashTimer = 0f; // Reinicia el temporizador
        
        dashCooldownCounter = dashCooldown;
        
        // 1. Calculamos la dirección hacia el jugador
        Vector2 direction = (target.position - transform.position).normalized;
        
        // 2. Calculamos la distancia actual
        float distanceToPlayer = Vector2.Distance(transform.position, target.position);
        
        // 3. Definimos el destino: Posición actual + (Dirección * (Distancia + EXTRA))
        // Esto crea un punto en la misma línea, pero más lejos.
        dashTargetPosition = (Vector2)transform.position + (direction * (distanceToPlayer + dashOvershoot));
        // -------------------

        // Configuración de animación y física
        anim.speed = dashAnimationSpeed;
        
        // Ahora nos movemos hacia ese punto lejano
        Vector2 dashDirection = (dashTargetPosition - (Vector2)transform.position).normalized;
        rb.velocity = dashDirection * dashSpeed;
        
        anim.SetBool("IsMoving", true);
    }

    void EndDash()
    {
        isDashing = false;
        
        // 1. Devuelve la animación a velocidad normal
        anim.speed = 1.0f;
        
        // 2. Detente
        rb.velocity = Vector2.zero;
        
        // 3. Regresa al estado de movimiento normal
        ChangeState(BossState.Moving);
    }
    
    // ------------------------------------

    // --- EVENTOS DE ANIMACIÓN (Ataque Básico) ---
    public void ANIM_EVENT_PerformMeleeAttack()
    {
        PlayerHealthController.instance.TakeDamage(damage);
        Debug.Log("Gollux ataca (Básico)!");
    }

    protected override void Die()
    {
        // 1. Desactivar collider
        GetComponent<Collider2D>().enabled = false;
        rb.velocity = Vector2.zero;
        this.enabled = false; 

        Debug.Log("¡El Boss ha sido derrotado!");

        /*if (lootPrefab != null)
        {
            Instantiate(lootPrefab, transform.position, Quaternion.identity);
        }*/

        // 2. PARTÍCULAS DE MUERTE
        SpawnDeathParticles();
        
        // 3. FADE-OUT SUAVE (oscurecer y desaparecer)
        StartCoroutine(DeathFadeOut());
    }
    
    void SpawnDeathParticles()
    {
        // OPCIÓN 1: Si tienes un prefab asignado, úsalo
        if (deathParticlePrefab != null)
        {
            GameObject particles = Instantiate(deathParticlePrefab, transform.position, Quaternion.identity);
            Destroy(particles, 3f); // Destruye después de 3 segundos
            return;
        }
        
        // OPCIÓN 2: Si no hay prefab Y useProceduralParticles está activo, crea partículas por código
        if (useProceduralParticles)
        {
            CreateProceduralParticles();
        }
    }
    
    void CreateProceduralParticles()
    {
        // Crea un GameObject temporal para las partículas
        GameObject particleObj = new GameObject("BossDeathParticles");
        particleObj.transform.position = transform.position;
        
        // Añade el componente ParticleSystem
        ParticleSystem ps = particleObj.AddComponent<ParticleSystem>();
        
        // --- CONFIGURACIÓN DEL SISTEMA DE PARTÍCULAS ---
        var main = ps.main;
        main.startLifetime = 1.5f; // Duración de cada partícula
        main.startSpeed = 5f; // Velocidad inicial
        main.startSize = 0.3f; // Tamaño
        main.maxParticles = particleCount;
        main.simulationSpace = ParticleSystemSimulationSpace.World; // No sigue al boss
        
        // Gradiente de color (naranja → rojo oscuro → transparente)
        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { 
                new GradientColorKey(particleColor1, 0.0f),  // Inicio: naranja
                new GradientColorKey(particleColor2, 0.5f),  // Medio: rojo
                new GradientColorKey(Color.black, 1.0f)      // Final: negro
            },
            new GradientAlphaKey[] { 
                new GradientAlphaKey(1.0f, 0.0f),   // Inicio: opaco
                new GradientAlphaKey(0.8f, 0.5f),   // Medio: semi-transparente
                new GradientAlphaKey(0.0f, 1.0f)    // Final: transparente
            }
        );
        colorOverLifetime.color = gradient;
        
        // Reducir tamaño con el tiempo
        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(0.0f, 1.0f);  // Inicio: tamaño normal
        curve.AddKey(1.0f, 0.0f);  // Final: tamaño 0
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, curve);
        
        // Emisión en ráfaga
        var emission = ps.emission;
        emission.enabled = true;
        emission.rateOverTime = 0; // No emitir continuamente
        
        // Forma de emisión (esfera)
        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.5f; // Radio de la explosión
        
        // Gravedad
        var forceOverLifetime = ps.forceOverLifetime;
        forceOverLifetime.enabled = true;
        forceOverLifetime.y = -2f; // Gravedad hacia abajo
        
        // Emite todas las partículas de golpe
        ps.Emit(particleCount);
        
        // Destruye el objeto después de que terminen las partículas
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
            
            // Oscurece el color (va hacia negro)
            Color currentColor = Color.Lerp(startColor, Color.black, t);
            // Reduce la opacidad (va hacia transparente)
            currentColor.a = Mathf.Lerp(1f, 0f, t);
            
            spriteRenderer.color = currentColor;
            
            yield return null;
        }
        
        // Asegura que esté completamente transparente
        spriteRenderer.color = new Color(0, 0, 0, 0);
        
        // Destruye el objeto
        Destroy(gameObject);
    }

    void ChangeState(BossState newState)
    {
        currentState = newState;
        
        // Vuelve a poner la velocidad normal si salimos de la carrera
        // por recibir daño o algo así.
        if (isDashing && newState != BossState.Moving)
        {
            EndDash();
        }
    }

    public override void TakeDamage(float damageToTake)
    {
        if (currentHealth <= 0) return;

        base.TakeDamage(damageToTake);
        DamageNumberController.instance.SpawnDamage(damageToTake, transform.position);
        
        // (Opcional: Llama a tu barra de vida de jefe aquí)
        // BossHealthBar.instance.UpdateHealth(currentHealth);
        
        if (currentHealth > 0) 
        {
            // Solo efecto visual, NO cambia de estado
            StartCoroutine(FlashDamage());
        }
    }
    private IEnumerator FlashDamage()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f); 
        spriteRenderer.color = Color.white; // Usa el color guardado en Awake
    }
}