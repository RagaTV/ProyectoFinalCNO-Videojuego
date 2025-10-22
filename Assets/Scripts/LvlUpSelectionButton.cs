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
        upgradeDescription.text = theWeapon.stats[theWeapon.weaponLvl].upgradeText;
        weaponIcon.sprite = theWeapon.icon;
        nameLvl.text = theWeapon.name + "\nNivel " + theWeapon.weaponLvl;

        assignedWeapon = theWeapon;
    }
     
    public void SelectUpgrade()
    {
        if(assignedWeapon != null)
        {
            assignedWeapon.LevelUp();

            UIController.instance.panelLvls.SetActive(false);
            Time.timeScale = 1f;
        }
    }
}
