using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthController : MonoBehaviour
{
    public Animator anim;
    public bool deathPlayer;

    public float currentHealth, maxHealth;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damageReceived){
        currentHealth-=damageReceived;

        if(currentHealth <= 0){
            deathPlayer = true;
            anim.SetBool("isDeath", deathPlayer);
        }
    }
}
