using UnityEngine; 
using UnityEngine.UI; 
using System.Collections; 
using UnityEngine.SceneManagement; 

public class PauseMenu : MonoBehaviour 
{
	public GameObject menuPausa; // asignar el objeto de la interfaz de pausa 
	public bool juegoPausado = false; // estado actual del juego (true = pausado, false = jugando).

    void Start() 
    {
    	menuPausa.SetActive(false);
    }

    void Update() 
    {
    	// Comprueba si la tecla 'esc' ha sido presionada en este frame.
    	if(Input.GetKeyDown(KeyCode.Escape)){ 
    		// Comprueba el estado actual del juego.
    		if(juegoPausado){ 
    			// Si juegoPausado es true, llama a la función para reanudar el juego.
    			Reanudar(); 
    		} else { 
    			// Si juegoPausado es false, llama a la función para pausar el juego.
    			Pausar(); 
    		}
    	}
    }

    public void Reanudar(){ 
		SFXManager.instance.PlaySFX(SoundEffect.UIClick);
    	menuPausa.SetActive(false); // Oculta el objeto de la interfaz de pausa.
		juegoPausado = false; // Actualiza el estado del juego a 'no pausado'.
		// 1. Revisamos si  el panel de niveles está activo.
        if (UIController.instance.panelLvls.activeSelf)
        {
            //    Si el panel de niveles SÍ está abierto, no reanudamos el tiempo.
            //    Simplemente dejamos que siga en 0.
            return;
        }
		Time.timeScale = 1.0f; // Establece la escala de tiempo a 1.0 (el tiempo fluye normalmente).
    }

    public void Pausar(){ 
		SFXManager.instance.PlaySFX(SoundEffect.UIClick);
    	menuPausa.SetActive(true); // Muestra el objeto de la interfaz de pausa.
    	Time.timeScale = 0.0f; // detiene el movimiento, físicas, animaciones basadas en tiempo
    	juegoPausado = true; // Actualiza el estado del juego a 'pausado'.
    }

    public void ReiniciarNivel(){ 
		SFXManager.instance.PlaySFX(SoundEffect.UIClick);
    	Time.timeScale = 1.0f; // el tiempo corre normal de nuevo
    	juegoPausado = false; // Restablece el estado de pausa 
    	// Obtiene el índice (número de orden en Build Settings) de la escena activa actualmente.
    	int escenaActual = SceneManager.GetActiveScene().buildIndex; 
    	// Carga la escena cuyo índice se acaba de obtener, reiniciando el nivel.
    	SceneManager.LoadScene(escenaActual); 
    }

	public void MenuPrincipal()
	{
		SFXManager.instance.PlaySFX(SoundEffect.UIClick);
		Time.timeScale = 1.0f; // Asegura que el tiempo corra antes de cargar la nueva escena.
		juegoPausado = false; // Restablece el estado de pausa.

		// Carga la escena con el nombre "Menu"
		SceneManager.LoadScene("Menu");
	}
	
	public void Configuracion(){
        SFXManager.instance.PlaySFX(SoundEffect.UIClick);
    }

}