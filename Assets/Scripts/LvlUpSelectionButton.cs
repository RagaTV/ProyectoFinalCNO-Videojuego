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

    private void Awake()
    {
        // Obtiene el componente Image de ESTE objeto (el botón)
        buttonImage = GetComponent<Image>(); 
    }

    public void UpdateButtonDisplay(Weapon theWeapon)
    {
        assignedWeapon = theWeapon;
        bool isNewWeapon = PlayerController.instance.unassignedWeapons.Contains(theWeapon);

        int statIndexToShow; 
        int levelNumToShow;  

        if (isNewWeapon)
        {
            statIndexToShow = 0;
            levelNumToShow = 1;
        }
        else
        {
            statIndexToShow = theWeapon.weaponLvl + 1;
            levelNumToShow = theWeapon.weaponLvl + 2; 
        }

        if (statIndexToShow >= theWeapon.stats.Count)
        {
            upgradeDescription.text = "MAX LEVEL";
            weaponIcon.sprite = theWeapon.icon;
            nameLvl.text = theWeapon.name + "\nNivel " + (theWeapon.weaponLvl + 1); 
            
            GetComponent<Button>().interactable = false;
        }
        else
        {
            GetComponent<Button>().interactable = true;

            upgradeDescription.text = theWeapon.stats[statIndexToShow].upgradeText;
            weaponIcon.sprite = theWeapon.icon;
            nameLvl.text = theWeapon.name + "\nNivel " + levelNumToShow;

            UpgradeRarity rarity = theWeapon.stats[statIndexToShow].rarity;
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
     
    public void SelectUpgrade()
    {
        if(assignedWeapon != null)
        {
            if (PlayerController.instance.unassignedWeapons.Contains(assignedWeapon))
            {
                // ¡Es un arma NUEVA! Llama a tu nueva función
                PlayerController.instance.AddWeapon(assignedWeapon);
            }
            else
            {
                // Es un arma que ya teníamos. Solo súbela de nivel.
                assignedWeapon.LevelUp();
            }
            
            // Cierra el panel y reanuda el juego
            UIController.instance.panelLvls.SetActive(false);
            Time.timeScale = 1f;
        }
    }
}
