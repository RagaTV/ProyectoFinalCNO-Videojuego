using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGunProjectile : MonoBehaviour
{
    public float speed = 10f;
    public float damage = 10f;
    public float lifetime = 5f;

    private Vector2 direction;
    private bool isReady = false;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (!isReady) return;

        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection.normalized;
        
        // Rotar el proyectil para que mire hacia donde va
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        
        isReady = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (PlayerHealthController.instance != null)
            {
                PlayerHealthController.instance.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Obstacles")) // Asegúrate de tener esta capa o usa tags
        {
            Destroy(gameObject);
        }
    }
}
