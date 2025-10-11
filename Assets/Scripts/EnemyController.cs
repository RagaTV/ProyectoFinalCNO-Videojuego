using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Rigidbody2D rB;
    public float moveSpeed;
    private Transform target;

    // Start is called before the first frame update
    void Start()
    {
        rB=GetComponent<Rigidbody2D>();
        target = FindObjectOfType<PlayerController>().transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rB.velocity = (target.position - transform.position).normalized * moveSpeed;

        Vector3 spriteOrientation = transform.localScale;

        if (target.position.x > transform.position.x)
            spriteOrientation.x = Mathf.Abs(spriteOrientation.x);
        else if (target.position.x < transform.position.x)
            spriteOrientation.x = -Mathf.Abs(spriteOrientation.x);

        transform.localScale = spriteOrientation;
    }
}
