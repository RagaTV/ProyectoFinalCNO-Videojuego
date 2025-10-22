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
        int nextLevel = theWeapon.weaponLvl + 1;

        if (nextLevel >= theWeapon.stats.Count)
        {
            upgradeDescription.text = "MAX LEVEL";
            weaponIcon.sprite = theWeapon.icon;
            nameLvl.text = theWeapon.name + "\nNivel " + theWeapon.weaponLvl; // Muestra el nivel actual
            assignedWeapon = null; 
        }
        else
        {
            upgradeDescription.text = theWeapon.stats[nextLevel].upgradeText;
            weaponIcon.sprite = theWeapon.icon;
            nameLvl.text = theWeapon.name + "\nNivel " + nextLevel;
            assignedWeapon = theWeapon;

            UpgradeRarity rarity = theWeapon.stats[nextLevel].rarity;
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

        /*upgradeDescription.text = theWeapon.stats[theWeapon.weaponLvl].upgradeText;
        weaponIcon.sprite = theWeapon.icon;
        nameLvl.text = theWeapon.name + "\nNivel " + theWeapon.weaponLvl;

        assignedWeapon = theWeapon;*/
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
