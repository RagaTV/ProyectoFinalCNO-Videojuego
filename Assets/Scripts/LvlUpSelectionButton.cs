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
    private UpgradeOption assignedOption;

    private void Awake()
    {
        buttonImage = GetComponent<Image>();
    }

    public void UpdateButtonDisplay(UpgradeOption option)
    {
        assignedOption = option;
        GetComponent<Button>().interactable = true;
        
        if (option.item is Weapon)
        {
            Weapon weapon = (Weapon)option.item;
            WeaponStats stats = (WeaponStats)option.generatedStats;
            
            upgradeDescription.text = stats.upgradeText;
            weaponIcon.sprite = weapon.icon;
            nameLvl.text = weapon.name + "\nNivel " + option.levelNum;
            SetRarityColor(stats.rarity);
        }
        else if (option.item is PassiveItem)
        {
            PassiveItem passive = (PassiveItem)option.item;
            PassiveStatLevel stats = (PassiveStatLevel)option.generatedStats;

            upgradeDescription.text = stats.upgradeText;
            weaponIcon.sprite = passive.icon;
            nameLvl.text = passive.passiveName + "\nNivel " + option.levelNum;
            SetRarityColor(stats.rarity);
        }
    }
    
    public void SelectUpgrade()
    {
        if(assignedOption == null) return;

        SFXManager.instance.PlaySFX(SoundEffect.UIClick); // (Lo muevo aquí)

        if (assignedOption.item is Weapon)
        {
            Weapon weapon = (Weapon)assignedOption.item;
            WeaponStats stats = (WeaponStats)assignedOption.generatedStats;

            if (PlayerController.instance.unassignedWeapons.Contains(weapon))
            {
                // Es un arma nueva, la añade y LUEGO aplica el Nivel 1
                // (AddWeapon ya añade el Nivel 1 (baseStats))
                PlayerController.instance.AddWeapon(weapon);
            }
            else
            {
                // Es un arma existente, solo aplica el nivel generado
                weapon.ApplyLevel(stats);
            }
        }
        else if (assignedOption.item is PassiveItem)
        {
            PassiveItem passive = (PassiveItem)assignedOption.item;
            PassiveStatLevel stats = (PassiveStatLevel)assignedOption.generatedStats;
            
            // Llama a la función de Upgrade, que se encarga de todo
            PlayerController.instance.UpgradePassive(passive, stats);
        }
        
        // Cierra el panel y reanuda el juego
        UIController.instance.panelLvls.SetActive(false);
        UIController.instance.panelActive = false;
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
