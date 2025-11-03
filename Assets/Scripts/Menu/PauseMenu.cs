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
    	menuPausa.SetActive(false); // Oculta el objeto de la interfaz de pausa.
    	Time.timeScale = 1.0f; // Establece la escala de tiempo a 1.0 (el tiempo fluye normalmente).
    	juegoPausado = false; // Actualiza el estado del juego a 'no pausado'.
    }

    public void Pausar(){ 
    	menuPausa.SetActive(true); // Muestra el objeto de la interfaz de pausa.
    	Time.timeScale = 0.0f; // detiene el movimiento, físicas, animaciones basadas en tiempo
    	juegoPausado = true; // Actualiza el estado del juego a 'pausado'.
    }

    public void ReiniciarNivel(){ 
    	Time.timeScale = 1.0f; // el tiempo corre normal de nuevo
    	juegoPausado = false; // Restablece el estado de pausa 
    	// Obtiene el índice (número de orden en Build Settings) de la escena activa actualmente.
    	int escenaActual = SceneManager.GetActiveScene().buildIndex; 
    	// Carga la escena cuyo índice se acaba de obtener, reiniciando el nivel.
    	SceneManager.LoadScene(escenaActual); 
    }

	public void MenuPrincipal(){ 
    	Time.timeScale = 1.0f; // Asegura que el tiempo corra antes de cargar la nueva escena.
    	juegoPausado = false; // Restablece el estado de pausa.

    	// Carga la escena con el nombre "Menu"
    	SceneManager.LoadScene("Menu"); 
    }

}