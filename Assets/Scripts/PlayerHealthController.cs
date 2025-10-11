using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthController : MonoBehaviour
{
    public Animator anim;
    public bool deathPlayer;

    public float currentHealth, maxHealth;

    private PlayerController playerController;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;

        playerController = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damageReceived)
    {
        if (deathPlayer) return;

        currentHealth -= damageReceived;

        if (currentHealth <= 0)
        {
            deathPlayer = true;
            anim.SetBool("isDeath", deathPlayer);
            playerController.Die();
        }
    }
}
