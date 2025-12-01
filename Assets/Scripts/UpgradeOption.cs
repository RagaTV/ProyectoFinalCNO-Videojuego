using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeOption
{
    // El item (ej. el ScriptableObject "Pluma Veloz")
    public object item; 
    
    // El NIVEL 2 que acabamos de generar (ej. Raro, +10% Vel)
    public object generatedStats; 
    
    // El nivel al que vamos a subir (ej. 2)
    public int levelNum; 
}
