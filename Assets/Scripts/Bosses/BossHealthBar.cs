using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHealthBar : MonoBehaviour
{
    public static BossHealthBar instance;

    [Header("Referencias UI (Arrastra los objetos aquí)")]
    public GameObject healthBarObject; 
    public Slider healthSlider;        
    public TMP_Text bossNameText;      
    public TMP_Text hpText;            

    private BossBase activeBoss;

    void Awake()
    {
        instance = this;
        // Al inicio ocultamos toda la barra
        if (healthBarObject != null) healthBarObject.SetActive(false);
    }

    void Update()
    {
        if (activeBoss != null)
        {
            // 1. Actualizar la barra visual
            healthSlider.value = activeBoss.currentHealth;

            // 2. Actualizar el texto de números (Ej: "450 / 1000")
            if (hpText != null)
            {
                hpText.text = activeBoss.currentHealth.ToString("0") + " / " + activeBoss.maxHealth.ToString("0");
            }
        }
        // Si el boss muere o desaparece, ocultamos la barra
        else if (healthBarObject.activeSelf)
        {
            healthBarObject.SetActive(false);
        }
    }

    public void ActivateBossHealth(BossBase boss, string name)
    {
        activeBoss = boss;
        
        // Configuramos los valores iniciales
        bossNameText.text = name;
        healthSlider.maxValue = boss.maxHealth;
        healthSlider.value = boss.currentHealth;
        
        // Mostramos la barra
        healthBarObject.SetActive(true);
    }
}