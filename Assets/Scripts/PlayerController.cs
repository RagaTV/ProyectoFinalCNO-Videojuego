using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float dashForce = 10f;       
    public float dashDuration = 0.5f;     
    public float dashCooldown = 0.5f;

    public Animator anim;
    private Rigidbody2D rb;

    private bool isDashing = false;
    private bool canDash = true;
    private Vector3 moveInput;
    private Vector3 lastDirection;

    private PlayerHealthController healthController;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        healthController = GetComponent<PlayerHealthController>();
    }

    void Update()
    {
        if (isDashing){
            return;
        } 

        // Movimiento normal
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput.Normalize();

        

        if (moveInput != Vector3.zero)
        {
            anim.SetBool("isMoving", true);
            lastDirection = moveInput;
        }
        else
        {
            anim.SetBool("isMoving", false);
        }

        Vector3 spriteOrientation = transform.localScale;
        if (moveInput.x > 0)
            spriteOrientation.x = Mathf.Abs(spriteOrientation.x);
        else if (moveInput.x < 0)
            spriteOrientation.x = -Mathf.Abs(spriteOrientation.x);
        transform.localScale = spriteOrientation;

        // Dash con Space
        if (Input.GetKeyDown(KeyCode.Space) && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    void FixedUpdate(){
        if (!isDashing) {
            rb.velocity = moveInput * speed;
        }
    }
    

    private IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;
        anim.SetTrigger("roll");

        // Dirección del dash
        Vector3 dashDirection = moveInput;
        if (dashDirection == Vector3.zero)
            dashDirection = lastDirection;

        // Aplicar fuerza del dash
        rb.velocity = dashDirection * dashForce;

        // Esperar duración del dash
        yield return new WaitForSeconds(dashDuration);

        // Terminar dash
        rb.velocity = Vector2.zero;
        isDashing = false;

        // Cooldown
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public void Die()
    {
        // Detiene TODAS las coroutines activas(el Dash)
        StopAllCoroutines();
        // Detiene inmediatamente todo el movimiento físico
        rb.velocity = Vector2.zero;
        // Desactiva este script para que Update() y FixedUpdate() dejen de ejecutarse
        this.enabled = false;
    }
}
