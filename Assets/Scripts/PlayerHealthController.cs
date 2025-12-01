using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealthController : MonoBehaviour
{
    public static PlayerHealthController instance;
    private void Awake()
    {
        instance = this;
    }
    public Animator anim;
    public bool deathPlayer;
    public float currentHealth;
    private float maxHealth;
    public Slider healthSlider;
    public TextMeshProUGUI healthText;
    private PlayerController playerController;
    private SpriteRenderer sprite; 
    private Color originalColor;
    
    public void ToggleHealth(bool state)
    {
        if(healthSlider != null)
        {
            healthSlider.gameObject.SetActive(state);
        }

        // Agrega esto para que también desaparezca el texto "100/100"
        if(healthText != null)
        {
            healthText.gameObject.SetActive(state);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        maxHealth = PlayerStats.instance.maxHealth;
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
        if (PlayerStats.instance.healthRegen > 0 && currentHealth < maxHealth && !deathPlayer)
        {
            currentHealth += PlayerStats.instance.healthRegen * Time.deltaTime;

            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }

            UpdateHealthUI();
        }
        
    }

    public void TakeDamage(float damageReceived)
    {
        if (deathPlayer) return;

        float actualDamageTaken = damageReceived - PlayerStats.instance.armor;
        if (actualDamageTaken < 1f)
        {
            actualDamageTaken = 1f;
        }

        currentHealth -= actualDamageTaken;
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
            SFXManager.instance.PlaySFX(SoundEffect.DeathSound);

            StopAllCoroutines(); 
            sprite.color = originalColor;
            playerController.Die();
            CameraControl.instance.StartDeathSequence();
            PlayerStats.instance.PrintGameReport();
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
    
    public void UpdateMaxHealth()
    {
        float healthPercent = currentHealth / maxHealth;
        
        maxHealth = PlayerStats.instance.maxHealth;
        
        currentHealth = maxHealth * healthPercent;
        
        healthSlider.maxValue = maxHealth;
        UpdateHealthUI();
    }
}
