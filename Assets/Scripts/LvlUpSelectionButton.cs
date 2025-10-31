using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LvlUpSelectionButton : MonoBehaviour
{
    public TMP_Text upgradeDescription, nameLvl;
    public Image weaponIcon;
    private Weapon assignedWeapon;
    public Image buttonImage; 
    public Color comunColor = new Color(0.8f, 0.8f, 0.8f); // Gris
    public Color raraColor = new Color(0.2f, 0.5f, 1f);   // Azul
    public Color epicaColor = new Color(0.7f, 0.2f, 1f);   // Morado
    public Color legendariaColor = new Color(1f, 0.8f, 0f); // Dorado
    private object assignedUpgrade;

    private void Awake()
    {
        buttonImage = GetComponent<Image>();
    }

    public void UpdateButtonDisplay(object upgradeObject)
    {
        assignedUpgrade = upgradeObject;
        GetComponent<Button>().interactable = true;

        // REVISA SI ES UN ARMA
        if (upgradeObject is Weapon)
        {
            Weapon theWeapon = (Weapon)upgradeObject;
            
            bool isNewWeapon = PlayerController.instance.unassignedWeapons.Contains(theWeapon);
            int statIndexToShow; 
            int levelNumToShow;  

            if (isNewWeapon)
            {
                statIndexToShow = 0; // Muestra info de Nivel 1 (índice 0)
                levelNumToShow = 1;
            }
            else
            {
                statIndexToShow = theWeapon.weaponLvl + 1; // Muestra info del sig. nivel
                levelNumToShow = theWeapon.weaponLvl + 2; 
            }

            if (statIndexToShow >= theWeapon.stats.Count)
            {
                upgradeDescription.text = "MAX LEVEL";
                weaponIcon.sprite = theWeapon.icon;
                nameLvl.text = theWeapon.name + "\nNivel " + (theWeapon.weaponLvl + 1); 
                GetComponent<Button>().interactable = false; // Desactiva el botón
            }
            else
            {
                upgradeDescription.text = theWeapon.stats[statIndexToShow].upgradeText;
                weaponIcon.sprite = theWeapon.icon;
                nameLvl.text = theWeapon.name + "\nNivel " + levelNumToShow;
                SetRarityColor(theWeapon.stats[statIndexToShow].rarity);
            }
        }
        // REVISA SI ES UN PASIVO
        else if (upgradeObject is PassiveItem)
        {
            PassiveItem thePassive = (PassiveItem)upgradeObject;
            
            int currentLevelIndex = -1;
            // Revisa si ya tenemos este pasivo
            if (PlayerController.instance.passiveLevels.ContainsKey(thePassive))
            {
                currentLevelIndex = PlayerController.instance.passiveLevels[thePassive];
            }

            int nextLevelIndex = currentLevelIndex + 1;
            int nextLevelNum = nextLevelIndex + 1; // Nivel 1, 2, 3...

            if (nextLevelIndex >= thePassive.levels.Count)
            {
                upgradeDescription.text = "MAX LEVEL";
                weaponIcon.sprite = thePassive.icon;
                nameLvl.text = thePassive.passiveName + "\nNivel " + (currentLevelIndex + 1); 
                GetComponent<Button>().interactable = false; // Desactiva el botón
            }
            else
            {
                upgradeDescription.text = thePassive.levels[nextLevelIndex].upgradeText;
                weaponIcon.sprite = thePassive.icon;
                nameLvl.text = thePassive.passiveName + "\nNivel " + nextLevelNum;
                SetRarityColor(thePassive.levels[nextLevelIndex].rarity);
            }
        }
    }
    
    public void SelectUpgrade()
    {
        if(assignedUpgrade == null) return;

        if (assignedUpgrade is Weapon)
        {
            Weapon weapon = (Weapon)assignedUpgrade;
            if (PlayerController.instance.unassignedWeapons.Contains(weapon))
            {
                PlayerController.instance.AddWeapon(weapon);
            }
            else
            {
                weapon.LevelUp();
            }
        }
        // REVISA SI ES UN PASIVO
        else if (assignedUpgrade is PassiveItem)
        {
            PassiveItem passive = (PassiveItem)assignedUpgrade;
            // Llama a la nueva función que creamos en PlayerController
            PlayerController.instance.UpgradePassive(passive);
        }
        
        UIController.instance.panelLvls.SetActive(false);
        Time.timeScale = 1f;
    }
    
    private void SetRarityColor(UpgradeRarity rarity)
    {
        switch (rarity)
        {
            case UpgradeRarity.Comun:
                buttonImage.color = comunColor;
                break;
            case UpgradeRarity.Rara:
                buttonImage.color = raraColor;
                break;
            case UpgradeRarity.Epica: 
                buttonImage.color = epicaColor;
                break;
            case UpgradeRarity.Legendaria:
                buttonImage.color = legendariaColor;
                break;
        }
    }
}
