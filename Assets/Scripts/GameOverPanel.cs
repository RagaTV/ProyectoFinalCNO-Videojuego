using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameOverPanel : MonoBehaviour
{
    public static GameOverPanel instance;
    private void Awake() { instance = this; }
    public GameObject panelGameO;

    [Header("Colores")]
    public Color upgradedColor = Color.white; // Color original (blanco/dorado)
    public Color baseColor = new Color(0.5f, 0.5f, 0.5f, 1f); // Gris apagado

    [Header("Textos Generales")]
    public TMP_Text timeSurvivedText;
    public TMP_Text levelObtainedText;
    public TMP_Text killsText;
    public TMP_Text totalDamageText;
    public TMP_Text dpsText;

    [Header("Textos de Stats del Jugador")]
    public TMP_Text maxHealthText;
    public TMP_Text armorText;
    public TMP_Text regenText;
    public TMP_Text damageMultText;
    public TMP_Text projectileSizeText;
    public TMP_Text moveSpeedText;
    public TMP_Text pickupRangeText;
    public TMP_Text luckText;
    public TMP_Text xpMultText;
    public TMP_Text coinMultText;

    [Header("Lista de Armas")]
    public TMP_Text weaponListLeft;
    public TMP_Text weaponListRight;

    public void DisplayStats()
    {
        panelGameO.SetActive(true);
        UIController.instance.ToggleHUD(false);
        PlayerHealthController.instance.ToggleHealth(false);
        PlayerStats stats = PlayerStats.instance;

        // --- 1. DATOS GENERALES ---
        float time = UIController.instance.gameTimer;
        timeSurvivedText.text = "Tiempo: " + System.TimeSpan.FromSeconds(time).ToString(@"mm\:ss");
        levelObtainedText.text = "Nivel: " + ExperienceLevelController.instance.currentLevel.ToString();
        killsText.text = "Kills: " + stats.enemiesKilled.ToString();
        totalDamageText.text = "Daño Total: " + stats.totalDamageDone.ToString("F0");
        float dps = time > 0 ? stats.totalDamageDone / time : 0;
        dpsText.text = "DPS: " + dps.ToString("F1");

        // --- 2. MEJORAS DEL JUGADOR (CON COLORES) ---
        // Usamos la función mágica que creamos abajo:
        
        // Vida
        SetStatText(maxHealthText, "Vida: ", stats.maxHealth, stats.BaseMaxHealth, "F0");
        
        // Armadura (Comparar si es mayor a 0, ya que la base suele ser 0)
        SetStatText(armorText, "Armadura: ", stats.armor, stats.BaseArmor, "F1");
        
        // Regen
        SetStatText(regenText, "Regen: ", stats.healthRegen, stats.BaseHealthRegen, "F1", "/s");

        // Daño (Porcentaje)
        SetStatText(damageMultText, "Daño: ", stats.damageMultiplier, stats.BaseDamage, "F0", "%", true);

        // Área (Porcentaje)
        SetStatText(projectileSizeText, "Área: ", stats.projectileSizeMultiplier, stats.BaseProjectileSize, "F0", "%", true);

        // Velocidad
        SetStatText(moveSpeedText, "Velocidad: ", stats.moveSpeed, stats.BaseMoveSpeed, "F1");

        // Rango
        SetStatText(pickupRangeText, "Rango: ", stats.pickupRange, stats.BasePickupRange, "F1");

        // Suerte (Porcentaje)
        SetStatText(luckText, "Suerte: ", stats.luck, stats.BaseLuck, "F0", "%", true);

        // XP y Oro (Opcionales)
        if(xpMultText) SetStatText(xpMultText, "XP: ", stats.xpMultiplier, stats.BaseXP, "F0", "%", true);
        if(coinMultText) SetStatText(coinMultText, "Oro: ", stats.coinMultiplier, stats.BaseCoins, "F0", "%", true);


        // --- 3. LISTA DE ARMAS ---
        string leftColumnBuilder = "";
        string rightColumnBuilder = "";
        int counter = 0;

        foreach (KeyValuePair<Weapon, float> pair in stats.weaponDamageStats)
        {
            string line = $"{pair.Key.name}: {pair.Value.ToString("F0")}\n";
            if (counter % 2 == 0) leftColumnBuilder += line;
            else rightColumnBuilder += line;
            counter++;
        }
        if (counter == 0) leftColumnBuilder = "Sin daño de armas";
        
        weaponListLeft.text = leftColumnBuilder;
        weaponListRight.text = rightColumnBuilder;

        Time.timeScale = 0f;
    }

    // --- FUNCIÓN AUXILIAR MÁGICA ---
    // Esta función revisa si el stat cambió y asigna el color correcto
    void SetStatText(TMP_Text textComp, string prefix, float currentVal, float baseVal, string format, string suffix = "", bool isPercent = false)
    {
        // 1. Construir el texto numérico
        string valueStr = "";
        
        if (isPercent)
        {
            // Si es porcentaje, multiplicamos por 100 antes de convertir a string
            valueStr = (currentVal * 100).ToString(format);
        }
        else
        {
            valueStr = currentVal.ToString(format);
        }

        // 2. Asignar el texto al componente
        textComp.text = prefix + valueStr + suffix;

        // 3. LOGICA DE COLOR
        // Usamos una pequeña tolerancia (0.01) porque comparar floats exactos a veces falla
        if (Mathf.Abs(currentVal - baseVal) > 0.01f)
        {
            // ¡Es diferente! (Mejorado) -> Color Brillante
            textComp.color = upgradedColor;
            // Opcional: Ponerlo en negrita
            textComp.fontStyle = FontStyles.Bold; 
        }
        else
        {
            // Es igual al base -> Color Gris
            textComp.color = baseColor;
            // Opcional: Quitar negrita
            textComp.fontStyle = FontStyles.Normal;
        }
    }
}