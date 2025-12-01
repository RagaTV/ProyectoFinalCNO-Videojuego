// PassiveStatLevel.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PassiveStatLevel
{
    public float multiplier; 
    public string upgradeText; 
    public UpgradeRarity rarity; 
    
    public PassiveStatLevel() {}

    public PassiveStatLevel(PassiveStatLevel other)
    {
        this.multiplier = other.multiplier;
        this.upgradeText = other.upgradeText;
        this.rarity = other.rarity;
    }
}