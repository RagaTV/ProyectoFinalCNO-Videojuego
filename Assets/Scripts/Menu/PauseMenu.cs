using UnityEngine; 
using UnityEngine.UI; 
using System.Collections; 
using UnityEngine.SceneManagement; 

public class PauseMenu : MonoBehaviour 
{
	public GameObject menuPausa; // asignar el objeto de la interfaz de pausa 
	public bool juegoPausado = false; // estado actual del juego (true = pausado, false = jugando).

	public static bool AbiertoDesdePausa = false;

	public ConfigurationManager configManager;

    void Start() 
    {
    	menuPausa.SetActive(false);
    }

    void Update() 
    {
    	// Comprueba si la tecla 'esc' ha sido presionada en este frame.
    	if(Input.GetKeyDown(KeyCode.Escape)){ 
    		
            // PRIMERO, revisa si el panel de Configuración está abierto
            if (configManager != null && configManager.menuConfiguracionPanel.activeSelf)
            {
                // Si SÍ está abierto, "Escape" actúa como el botón "Regresar"
                configManager.Regresar();
                
                // Y nos aseguramos de que el juego SEPA que sigue pausado
                juegoPausado = true; 
                Time.timeScale = 0.0f;
            }

            // Si NINGÚN panel especial está abierto, haz la lógica normal
    		else if(juegoPausado)
            { 
    			Reanudar(); 
    		} 
            else 
            { 
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
		if (configManager != null && configManager.menuConfiguracionPanel.activeSelf)
        {
            //    Si cualquiera de los dos está abierto, NO HAGAS NADA.
            //    Simplemente ignora el clic en el botón de pausa.
            return;
        }
		
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
		SceneManager.LoadScene("Menu");
	}
	
	public void Configuracion(){
        SFXManager.instance.PlaySFX(SoundEffect.UIClick);

	    // 1. OCULTA el menú principal de pausa
	    menuPausa.SetActive(false); 
	    
	    // 2. REGISTRA que se abrió desde la Pausa
	    AbiertoDesdePausa = true; 
	    
		if (configManager != null){
	    	configManager.AbrirConfiguracion(); 
	    }
    }

}