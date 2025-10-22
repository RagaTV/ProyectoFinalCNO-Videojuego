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
            nameLvl.text = theWeapon.name + "\nNivel " + nextLevel; // Muestra el nivel al que subirá
            assignedWeapon = theWeapon;
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
        // 1. Revisa si el arma seleccionada está en la lista de 'unassigned'
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
