using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperienceLevelController : MonoBehaviour
{
    public static ExperienceLevelController instance;
    public int currentExperience;
    public ExpPickup pickup;
    public List<int> expLevels;
    public int currentLevel = 1; 
    public int levelCapForCurve = 100;
    public int baseExperience = 5;
    private PlayerHealthController healthController;
    private void Awake()
    {
        instance = this;
    }
    
    void Start()
    {
        healthController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthController>();
        
        // Asumiendo que 'expLevels' tiene al menos un valor inicial desde el Inspector
        while (expLevels.Count < levelCapForCurve) 
        {
            expLevels.Add(Mathf.CeilToInt(expLevels[expLevels.Count - 1] * 1.05f));
        }
    }

    void Update()
    {

    }
    
    public int GetRequiredExp(int level)
    {
        if (level >= expLevels.Count)
        {
            // Si el nivel es 100 o más (índice 99), siempre usa el requisito de XP para el nivel 100
            return expLevels[expLevels.Count - 1]; 
        }
        
        return expLevels[level];
    }

    public void GetExp(int amountToGet)
    {
        float finalExp = amountToGet * PlayerStats.instance.xpMultiplier;
        currentExperience += Mathf.CeilToInt(finalExp);

        // Usamos un 'while' por si el jugador gana suficiente XP para subir varios niveles
        int expForNextLevel = GetRequiredExp(currentLevel); // <-- Usa la nueva función
        
        while (currentExperience >= expForNextLevel)
        {
            LevelUp(expForNextLevel); // <-- Pasa el valor para restar
            expForNextLevel = GetRequiredExp(currentLevel); // <-- Obtiene el requisito para el *nuevo* nivel
        }

        if (healthController != null && !healthController.deathPlayer)
        {
            // Actualiza la UI con el requisito de XP correcto
            UIController.instance.UpdateExperience(currentExperience, expForNextLevel, currentLevel);
        }
    }

    public void SpawnExp(Vector3 position, int expValue)
    {
        Instantiate(pickup, position, Quaternion.identity).expValue = expValue;
    }
    
    void LevelUp(int expToNext) // <-- Modificado para aceptar la XP
    {
        SFXManager.instance.PlaySFX(SoundEffect.LevelUp);
        
        currentExperience -= expToNext; // <-- Resta la cantidad exacta
        currentLevel++;

        UIController.instance.ShowLevelUpOptions();
    }
}
