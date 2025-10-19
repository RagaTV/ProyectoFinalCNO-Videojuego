using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamager : MonoBehaviour //weaponds
{
    public float damageAmount;
    public float lifeTime, growSpeed = 4f;
    private Vector3 targetSize; 

    // Start is called before the first frame update
    void Start()
    {
        //Destroy(gameObject, lifeTime);
        targetSize = transform.localScale;
        transform.localScale = Vector3.zero;

    }

    void Update()
    {
        transform.localScale = Vector3.MoveTowards(transform.localScale, targetSize, growSpeed * Time.deltaTime);
        lifeTime -= Time.deltaTime;
        if(lifeTime <= 0)
        {
            targetSize = Vector3.zero;
            if(transform.localScale.x == 0f)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D (Collider2D collision)
    {
        if(collision.tag == "Enemy")
        {
            collision.GetComponent<EnemyController>().TakeDamage(damageAmount);
            //collision.GetComponent<EnemyMovement>().TakeDamage(damageAmount); //en el video se llama diferente el script
        }
    }
}
