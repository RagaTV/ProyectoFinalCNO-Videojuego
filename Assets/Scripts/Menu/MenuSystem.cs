using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSystem : MonoBehaviour
{
    public void Jugar()
    {
        SFXManager.instance.PlaySFX(SoundEffect.UIClick);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        SceneManager.LoadScene("EscenaBryan");
    }
    
    public void Configuracion(){
        SFXManager.instance.PlaySFX(SoundEffect.UIClick);
    }

    public void Salir(){
        SFXManager.instance.PlaySFX(SoundEffect.UIClick);
    	Debug.Log("Saliendo del juego...");
    	Application.Quit();
    }
}
