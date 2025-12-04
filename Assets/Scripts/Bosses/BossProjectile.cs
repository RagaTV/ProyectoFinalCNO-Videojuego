using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    public float speed = 5f;
    public float damage = 10f;
    public float lifetime = 5f;

    private Vector2 direction;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
        
        // Rotate to face direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
        
        // Rotar sobre su propio eje (efecto visual)
        transform.Rotate(Vector3.forward * 360f * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            if (PlayerHealthController.instance != null)
            {
                PlayerHealthController.instance.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Obstacles")) 
        {
            Destroy(gameObject);
        }
    }
}
