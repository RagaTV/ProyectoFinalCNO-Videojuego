using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSystem : MonoBehaviour
{

	//public GameObject ConfigurationInterfaz; // Asigna el panel de configuración aquí

	public ConfigurationManager configManager;


    public void Jugar()
    {
        SFXManager.instance.PlaySFX(SoundEffect.UIClick);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        SceneManager.LoadScene("EscenaBryan");
    }
    
    public void Configuracion(){
        SFXManager.instance.PlaySFX(SoundEffect.UIClick);

        // 2. REGISTRA que NO se abrió desde la Pausa
        PauseMenu.AbiertoDesdePausa = false; 
        
        // 3. Pide al ConfigurationManager que muestre el panel y la primera sección.
        if (configManager != null)
        {
            configManager.AbrirConfiguracion();
        }
    }

    public void Salir(){
        SFXManager.instance.PlaySFX(SoundEffect.UIClick);
    	Debug.Log("Saliendo del juego...");
    	Application.Quit();
    }
}
