using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGolluxController : MonoBehaviour, IDamageable
{
    [Header("Stats Base")]
    public float maxHealth = 1000f;
    [HideInInspector]
    public float currentHealth;
    public float moveSpeed = 2f;
    public float damage = 10f;
    
    [Header("Ataque Básico (Hit)")]
    public float hitWaitTime = 1.5f; 
    private float hitCounter;
    
    // --- ¡NUEVAS VARIABLES PARA LA CARRERA! ---
    [Header("Ataque de Carrera (Dash)")]
    public float dashOvershoot = 3f;
    public float dashSpeed = 8f; // ¡Más rápido que moveSpeed!
    public float dashAnimationSpeed = 2.5f; // Multiplicador de la anim "Move"
    public float dashDamageMultiplier = 2f; // Doble daño
    public float dashCooldown = 10f; // Tiempo entre carreras
    public float dashMinDistance = 4f; // Distancia mín. para usarla
    private float dashCooldownCounter;
    private Vector2 dashTargetPosition; // Dónde estaba el jugador
    private bool isDashing = false; // ¡El booleano que pediste!
    // ------------------------------------------
    [Header("Ajustes de Ataque")]
    public float attackDuration = 1.0f;
    [Header("Componentes")]
    private Animator anim;
    private Rigidbody2D rb;
    private Transform target;
    
    // FSM (solo 4 estados, Dashing se maneja con el booleano)
    private enum BossState { Idle, Moving, Attacking, Hurt }
    private BossState currentState;
    private SpriteRenderer spriteRenderer;
    [Header("Muerte y Loot")]
    //public GameObject lootPrefab; 
    public float deathDelay = 1.5f;
    
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        target = PlayerController.instance.transform;
        spriteRenderer = GetComponent<SpriteRenderer>();

        currentHealth = maxHealth;
        
        hitCounter = 0;
        dashCooldownCounter = dashCooldown; // Empieza en cooldown
        
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

        if (!isDashing && currentHealth > 0)
        {
            if (target.position.x > transform.position.x)
            {
                spriteRenderer.flipX = false; // Asumiendo que tu sprite original mira a la DERECHA
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
                    // Comprueba si ya llegó al destino
                    if (Vector2.Distance(transform.position, dashTargetPosition) < 0.5f)
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
                    rb.velocity = (target.position - transform.position).normalized * moveSpeed;
                }

                anim.SetBool("IsMoving", rb.velocity.magnitude > 0.1f);
                break;

            case BossState.Attacking:
                // Pausado, esperando evento de animación 'Hit'
                rb.velocity = Vector2.zero;
                anim.SetBool("IsMoving", false);
                break;
            
            case BossState.Hurt:
                // Pausado, esperando la corrutina de recuperación
                rb.velocity = Vector2.zero;
                anim.SetBool("IsMoving", false);
                break;
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
            // EN LUGAR de solo cambiar estado, iniciamos la rutina segura
            StartCoroutine(AttackRoutine());
        }
    }

    // --- ESTA ES LA SOLUCIÓN MÁGICA ---
    IEnumerator AttackRoutine()
    {
        ChangeState(BossState.Attacking);
        anim.SetTrigger("Hit"); // Inicia el golpe

        yield return new WaitForSeconds(attackDuration);

        hitCounter = hitWaitTime;
        ChangeState(BossState.Moving);
        
        // FUERZA BRUTA: Le decimos al Animator "Ponte a caminar AHORA MISMO"
        // (Asegúrate de que "Move" sea el nombre exacto de tu estado en la caja gris del Animator)
        anim.Play("Move");
    }

        
    void StartDash()
    {
        isDashing = true;
        
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
        isDashing = false; // ¡Desactivamos el booleano!
        
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

    void Die()
    {
        // 1. EVITAR QUE SIGA MOLESTANDO
        // Desactivamos el colisionador para que el jugador ya no pueda chocar ni recibir daño del cadáver.
        GetComponent<Collider2D>().enabled = false;
        
        // Detenemos el movimiento en seco.
        rb.velocity = Vector2.zero;
        
        // Desactivamos este script para que el Update() deje de correr (ya no buscará al jugador).
        this.enabled = false; 

        Debug.Log("¡El Boss ha sido derrotado!");

        /*if (lootPrefab != null)
        {
            // Instancia el cofre o gema en la posición del boss
            Instantiate(lootPrefab, transform.position, Quaternion.identity);
        }*/

        Destroy(gameObject, deathDelay);
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

    public void TakeDamage(float damageToTake)
    {
        if (currentHealth <= 0)
        {
            return;
        }

        currentHealth -= damageToTake;
        ChangeState(BossState.Hurt);
        
        // (Opcional: Llama a tu barra de vida de jefe aquí)
        // BossHealthBar.instance.UpdateHealth(currentHealth);
        
        // Activa el estado de "Hurt" que diseñamos
        ChangeState(BossState.Hurt);
        // (Asegúrate de tener un Trigger "Hurt" en tu Animator)
        // anim.SetTrigger("Hurt"); 
        
        if (currentHealth <= 0)
        {
            Die(); // Llama a la función Die() que ya escribimos
        } 
        else 
        {
            StartCoroutine(FlashDamage());
            StartCoroutine(RecoverFromHurt());
        }
        
        // Muestra el número de daño
        DamageNumberController.instance.SpawnDamage(damageToTake, transform.position);
    }

    // El jefe no puede ser empujado, así que esta función
    // simplemente llama a la otra.
    public void TakeDamage(float damageToTake, bool shouldKnockBack)
    {
        TakeDamage(damageToTake);
        
        // (No aplicamos knockback al jefe)
        // if (shouldKnockBack) { ... }
    }

    private IEnumerator FlashDamage()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f); 
        spriteRenderer.color = Color.white; // O el color original
    }

    private IEnumerator RecoverFromHurt()
    {
        // Define cuánto tiempo se queda "aturdido"
        yield return new WaitForSeconds(0.001f); 
        
        // Después de 0.5s, vuelve a moverse
        ChangeState(BossState.Moving);
    }
}