using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealthController : MonoBehaviour
{
    public Animator anim;
    public bool deathPlayer;

    public float currentHealth, maxHealth;
    public Slider healthSlider;
    public TextMeshProUGUI healthText;

    private PlayerController playerController;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;

        playerController = GetComponent<PlayerController>();
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
        healthText.text = currentHealth + " / " + maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damageReceived)
    {
        if (deathPlayer) return;

        currentHealth -= damageReceived;
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
        healthSlider.value = currentHealth;
        healthText.text = currentHealth + " / " + maxHealth;

        if (currentHealth <= 0)
        {
            deathPlayer = true;
            anim.SetBool("isDeath", deathPlayer);
            playerController.Die();
        }
    }
}
