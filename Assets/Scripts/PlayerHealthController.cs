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
    private SpriteRenderer sprite; 
    private Color originalColor;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;

        playerController = GetComponent<PlayerController>();
        healthSlider.maxValue = maxHealth;

        sprite = GetComponent<SpriteRenderer>();
        originalColor = sprite.color;

        UpdateHealthUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damageReceived)
    {
        if (deathPlayer) return;

        currentHealth -= damageReceived;
        StartCoroutine(FlashDamage());

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
        
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            deathPlayer = true;
            anim.SetBool("isDeath", deathPlayer);

            StopAllCoroutines(); 
            sprite.color = originalColor;
            playerController.Die();
            CameraControl.instance.StartDeathSequence();
        }
    }

    private void UpdateHealthUI()
    {
        healthSlider.value = currentHealth;

        int currentHPForText = Mathf.CeilToInt(currentHealth);
        int maxHPForText = Mathf.CeilToInt(maxHealth);

        healthText.text = currentHPForText + " / " + maxHPForText;
    }

    private IEnumerator FlashDamage()
    {
        sprite.color = Color.red;

        yield return new WaitForSeconds(0.1f); 

        if (!deathPlayer)
        {
            sprite.color = originalColor;
        }
    }
}
