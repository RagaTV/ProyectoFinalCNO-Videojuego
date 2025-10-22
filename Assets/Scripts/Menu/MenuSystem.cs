using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSystem : MonoBehaviour
{
    public void Jugar(){
    	//SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    	SceneManager.LoadScene("EscenaBryan");
    }

    public void Salir(){
    	Debug.Log("Saliendo del juego...");
    	Application.Quit();
    }
}
