using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Rigidbody2D rB;
    private Animator anim;

    public float moveSpeed;
    public float damageAmount;
    public float hitWaitTime = 1f;
    private float hitCounter;
    private float knockbackForce = 5f;

    private Transform target;
    private PlayerHealthController healthController;

    

    // Start is called before the first frame update
    void Start()
    {
        rB=GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            target = playerObject.transform;
            healthController = playerObject.GetComponent<PlayerHealthController>();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    { 
        if (target != null && healthController != null && !healthController.deathPlayer)
        {
            // Si el jugador está vivo, lo seguimos.
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
        else
        {
            // Si el jugador está muerto o no existe, detenemos al enemigo.
            rB.velocity = Vector2.zero;
            anim.speed = 0;
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
                    // 1. Infligimos daño y aplicamos el knockback
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
}
