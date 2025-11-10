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
    public int maxLevel = 100;
    public int baseExperience = 5;
    private PlayerHealthController healthController;
    private void Awake()
    {
        instance = this;
    }
    
    void Start()
    {
        healthController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthController>();
        while (expLevels.Count < maxLevel)
        {
            expLevels.Add(Mathf.CeilToInt(expLevels[expLevels.Count - 1] * 1.05f));
        }
    }

    void Update()
    {
        
    }

    public void GetExp(int amountToGet)
    {
        float finalExp = amountToGet * PlayerStats.instance.xpMultiplier;
        currentExperience += Mathf.CeilToInt(finalExp);

        if (currentExperience >= expLevels[currentLevel])
        {
            LevelUp();
        }
        if (healthController != null && !healthController.deathPlayer)
        {
            UIController.instance.UpdateExperience(currentExperience, expLevels[currentLevel], currentLevel);
        }
        
    }

    public void SpawnExp(Vector3 position, int expValue)
    {
        Instantiate(pickup, position, Quaternion.identity).expValue = expValue;
    }
    
    void LevelUp()
    {
        SFXManager.instance.PlaySFX(SoundEffect.LevelUp);
        
        currentExperience -= expLevels[currentLevel];
        currentLevel++;

        if (currentLevel >= expLevels.Count)
        {
            currentLevel = expLevels.Count - 1;
        }
        UIController.instance.ShowLevelUpOptions();
    }
    }
