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
    
    public void ShowLevelUpOptions()
    {
        //Pausa el juego y muestra el panel
        panelLvls.SetActive(true);
        Time.timeScale = 0f;

        //Crea una lista de opciones posibles
        List<Weapon> availableUpgrades = new List<Weapon>();

        //Revisa las armas que YA TIENES (assignedWeapons)
        if (PlayerController.instance.assignedWeapons.Count > 0)
        {
            foreach (Weapon weapon in PlayerController.instance.assignedWeapons)
            {
                // Solo la añade si NO está al nivel máximo
                if (weapon.weaponLvl < weapon.stats.Count - 1)
                {
                    availableUpgrades.Add(weapon);
                }
            }
        }

        //Revisa las armas que NO TIENES (unassignedWeapons)
        if (PlayerController.instance.unassignedWeapons.Count > 0)
        {
            availableUpgrades.AddRange(PlayerController.instance.unassignedWeapons);
        }

        //Asigna las opciones a los botones
        foreach (LvlUpSelectionButton button in lvlUpButtons)
        {
            if (availableUpgrades.Count > 0)
            {
                //Elige una opción aleatoria de la lista
                int selectedOption = UnityEngine.Random.Range(0, availableUpgrades.Count);
                
                //Asigna esa arma al botón
                button.UpdateButtonDisplay(availableUpgrades[selectedOption]);
                
                //Quita esa arma de la lista para que no se repita
                availableUpgrades.RemoveAt(selectedOption);
            }
            else
            {
                //Si no hay más opciones, esconde el botón
                button.gameObject.SetActive(false);
            }
        }
    }
}
