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
    private void Awake()
    {
        instance = this;
    }
    
    void Start()
    {
        while (expLevels.Count < maxLevel)
        {
            expLevels.Add(Mathf.CeilToInt(expLevels[expLevels.Count - 1] * 1.1f));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetExp(int amountToGet)
    {
        currentExperience += amountToGet;

        if (currentExperience >= expLevels[currentLevel])
        {
            LevelUp();
        }
        UIController.instance.UpdateExperience(currentExperience, expLevels[currentLevel], currentLevel);
    }

    public void SpawnExp(Vector3 position, int expValue)
    {
        Instantiate(pickup, position, Quaternion.identity).expValue = expValue;
    }
    
    void LevelUp()
{
    currentExperience -= expLevels[currentLevel];
    currentLevel++;

    if (currentLevel >= expLevels.Count)
    {
        currentLevel = expLevels.Count - 1;
    }

    // UIController.instance.panelLvls.SetActive(true);
    // Time.timeScale = 0;
    // UIController.instance.lvlUpButtons[0].UpdateButtonDisplay(PlayerController.instance.assignedWeapons[0]); 
    // UIController.instance.lvlUpButtons[1].UpdateButtonDisplay(PlayerController.instance.unassignedWeapons[0]); 
    
    
    // --- AÑADE SOLO ESTA LÍNEA ---
    UIController.instance.ShowLevelUpOptions();
}
}
