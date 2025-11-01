using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIController : MonoBehaviour
{
    public static UIController instance;
    private void Awake()
    {
        instance = this;
    }

    public Slider expLvlSlider;
    public TMP_Text expLvlText;
    public TMP_Text timer;
    public LvlUpSelectionButton[] lvlUpButtons;
    public GameObject panelLvls;

    public ItemSlotUI[] weaponSlots;
    public ItemSlotUI[] passiveSlots;

   public float gameTimer = 0f;
    // Start is called before the first frame update
    void Start()
    {
        expLvlSlider.value = 0;
    }

    // Update is called once per frame
    void Update()
    {
        gameTimer += Time.deltaTime;
        TimeSpan time = TimeSpan.FromSeconds(gameTimer);

        timer.text = time.ToString(@"mm\:ss");
    }

    public void UpdateExperience(int currentExperience, int levelExp, int currentLevel)
    {
        expLvlSlider.maxValue = levelExp;
        expLvlSlider.value = currentExperience;
        expLvlText.text = "Nivel: " + currentLevel;
    }

    public void UpdateInventoryUI()
    {
        List<Weapon> assignedWeapons = PlayerController.instance.assignedWeapons;
        for(int i = 0; i < weaponSlots.Length; i++)
        {
            if(i < assignedWeapons.Count)
            {
                weaponSlots[i].SetWeapon(assignedWeapons[i]);
            }
            else
            {
                weaponSlots[i].ClearSlot();
            }
        }

        List<PassiveItem> assignedPassives = PlayerController.instance.assignedPassives;
        for(int i = 0; i < passiveSlots.Length; i++)
        {
            if(i < assignedPassives.Count)
            {
                passiveSlots[i].SetPassive(assignedPassives[i]);
            }
            else
            {
                passiveSlots[i].ClearSlot();
            }
        }
    }
    
    // --- REEMPLAZA ESTE MÉTODO COMPLETO ---
    public void ShowLevelUpOptions()
    {
        panelLvls.SetActive(true);
        Time.timeScale = 0f;

        List<Weapon> upgradableWeapons = new List<Weapon>();
        List<object> generalPool = new List<object>();
        
        if (PlayerController.instance.assignedWeapons.Count > 0)
        {
            foreach (Weapon weapon in PlayerController.instance.assignedWeapons)
            {
                if (weapon.weaponLvl < weapon.stats.Count - 1)
                {
                    upgradableWeapons.Add(weapon);
                }
            }
        }

        if (PlayerController.instance.assignedWeapons.Count < 4) 
        {
            if (PlayerController.instance.unassignedWeapons.Count > 0)
            {
                generalPool.AddRange(PlayerController.instance.unassignedWeapons);
            }
        }

        if (PlayerController.instance.assignedPassives.Count > 0)
        {
            foreach (PassiveItem passive in PlayerController.instance.assignedPassives)
            {
                int currentLevelIndex = PlayerController.instance.passiveLevels[passive];
                if (currentLevelIndex < passive.levels.Count - 1)
                {
                    generalPool.Add(passive);
                }
            }
        }
        
        if (PlayerController.instance.assignedPassives.Count < 4) 
        {
            if (PlayerController.instance.unassignedPassives.Count > 0)
            {
                generalPool.AddRange(PlayerController.instance.unassignedPassives);
            }
        }

        List<object> optionsToShow = new List<object>();

        if (upgradableWeapons.Count > 0)
        {
            int selectedIndex = UnityEngine.Random.Range(0, upgradableWeapons.Count);
            optionsToShow.Add(upgradableWeapons[selectedIndex]);
            upgradableWeapons.RemoveAt(selectedIndex); 
        }

        generalPool.AddRange(upgradableWeapons); // Añadir las mejoras de armas restantes

        int slotsToFill = lvlUpButtons.Length - optionsToShow.Count;
        for(int i = 0; i < slotsToFill; i++)
        {
            if (generalPool.Count == 0) break; 

            int selectedIndex = UnityEngine.Random.Range(0, generalPool.Count);
            optionsToShow.Add(generalPool[selectedIndex]);
            generalPool.RemoveAt(selectedIndex);
        }

        ShuffleList(optionsToShow);

        for(int i = 0; i < lvlUpButtons.Length; i++)
        {
            lvlUpButtons[i].gameObject.SetActive(true); 

            if (i < optionsToShow.Count)
            {
                lvlUpButtons[i].UpdateButtonDisplay(optionsToShow[i]);
            }
            else
            {
                lvlUpButtons[i].gameObject.SetActive(false); 
            }
        }
    }

    private void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = UnityEngine.Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}
