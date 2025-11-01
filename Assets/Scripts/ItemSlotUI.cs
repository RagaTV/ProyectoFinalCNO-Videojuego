using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlotUI : MonoBehaviour
{
    public Image iconImage;
    public TMP_Text levelText;
    public GameObject slotPanel; 

    public void SetWeapon(Weapon weapon)
    {
        slotPanel.SetActive(true);
        iconImage.sprite = weapon.icon;
        
        int levelToShow = weapon.weaponLvl + 1;
        levelText.text = "Nvl " + levelToShow;
    }

    public void SetPassive(PassiveItem passive)
    {
        slotPanel.SetActive(true);
        iconImage.sprite = passive.icon;
        
        int levelToShow = 1;
        if(PlayerController.instance.passiveLevels.ContainsKey(passive))
        {
            levelToShow = PlayerController.instance.passiveLevels[passive] + 1;
        }
        
        levelText.text = "Nvl " + levelToShow;
    }

    public void ClearSlot()
    {
        slotPanel.SetActive(false);
        iconImage.sprite = null;
        levelText.text = "";
    }
}
