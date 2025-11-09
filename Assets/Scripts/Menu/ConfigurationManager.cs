using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Necesario para Button y Color
using TMPro; // NECESARIO para usar TextMeshProUGUI

public class ConfigurationManager : MonoBehaviour
{

	// --- REFERENCIAS DE PANELES DE REGRESO (ASIGNAR EN AMBAS ESCENAS) ---
    // En EscenaMenu, asignar MenuPrincipalPanel y dejar MenuPausaPanel en NULL.
    // En EscenaBryan, asignar MenuPausaPanel y dejar MenuPrincipalPanel en NULL.
    public GameObject MenuPausaPanel;      
    public GameObject MenuPrincipalPanel; 
    
    // --- REFERENCIAS DE LA INTERFAZ DE CONFIGURACIÓN (ASIGNAR EN AMBAS ESCENAS) ---
    public GameObject menuConfiguracionPanel; 
    
    // Paneles de Contenido (Las pestañas)
    public GameObject panelAjustes;      
    public GameObject panelControles;    
    public GameObject panelEnciclopedia;

    // --- REFERENCIAS DE BOTONES (PESTAÑAS) ---
    public Button ajustesButton;
    public Button controlesButton;
    public Button enciclopediaButton;

    // --- REFERENCIAS DEL TEXTO (¡TMPro!) ---
    // Usa TMPro.TextMeshProUGUI en lugar de Text
    public TextMeshProUGUI ajustesText;
    public TextMeshProUGUI controlesText;
    public TextMeshProUGUI enciclopediaText;

    // --- PROPIEDADES DE COLOR ---
    // Color de la IMAGEN del botón
    public Color colorActivo = new Color32(29, 23, 23, 255); // Gris Oscuro (Opaco)
    public Color colorInactivo = new Color(0.7f, 0.7f, 0.7f, 1f); // Gris Claro
    
    // Color del TEXTO del botón
    public Color colorTextoActivo = Color.black; // Texto negro para el botón activo
    public Color colorTextoInactivo = Color.white; // Texto blanco para los inactivos

    // --- FUNCIÓN PRIVADA PARA RESALTAR EL BOTÓN ACTIVO ---
    private void ResaltarBoton(Button botonActivo, TextMeshProUGUI textoActivo)
    {
        // 1. Resetear IMAGEN y TEXTO a color inactivo
        // Reseteo de IMAGEN
        if (ajustesButton != null && ajustesButton.image != null) ajustesButton.image.color = colorInactivo;
        if (controlesButton != null && controlesButton.image != null) controlesButton.image.color = colorInactivo;
        if (enciclopediaButton != null && enciclopediaButton.image != null) enciclopediaButton.image.color = colorInactivo;

        // Reseteo de TEXTO
        if (ajustesText != null) ajustesText.color = colorTextoInactivo;
        if (controlesText != null) controlesText.color = colorTextoInactivo;
        if (enciclopediaText != null) enciclopediaText.color = colorTextoInactivo;


        // 2. Aplicar color ACTIVO al botón y al texto seleccionado
        if (botonActivo != null && botonActivo.image != null)
        {
            botonActivo.image.color = colorActivo; // Cambia la IMAGEN a gris oscuro
        }
        
        if (textoActivo != null)
        {
            textoActivo.color = colorTextoActivo; // Cambia el TEXTO a negro
        }
    }


    // --- FUNCIONES DE APERTURA ---

    public void AbrirConfiguracion(){
        if (menuConfiguracionPanel != null)
        {
            menuConfiguracionPanel.SetActive(true);
        }
        // Muestra por defecto la primera sección y resalta su botón
        AbrirPanelAjustes();
    }

    // --- FUNCIÓN DE REGRESO ---

    public void Regresar()
    {
        // 1. Oculta la interfaz de configuración.
        if (menuConfiguracionPanel != null)
        {
            menuConfiguracionPanel.SetActive(false);
        }
        
        // 2. Decide a dónde volver basándose en la bandera estática (AbiertoDesdePausa).
        if (PauseMenu.AbiertoDesdePausa) 
        {
            // Volver al menú de pausa (Escena de Juego - EscenaBryan)
            if (MenuPausaPanel != null)
            {
                MenuPausaPanel.SetActive(true);
                
                // Asegura que el tiempo se detenga si volvemos al menú de Pausa
                PauseMenu pauseMenu = FindObjectOfType<PauseMenu>();
                if (pauseMenu != null)
                {
                    pauseMenu.Pausar(); 
                }
            }
        }
        else 
        {
            // Volver al menú principal (Escena de Menú)
            if (MenuPrincipalPanel != null)
            {
                MenuPrincipalPanel.SetActive(true);
            }
        }
    }
    
    // --- LÓGICA DE NAVEGACIÓN ENTRE PESTAÑAS Y RESALTE ---

    public void AbrirPanelAjustes(){
        // Activar/Desactivar Paneles de Contenido
        if (panelAjustes != null) panelAjustes.SetActive(true);
        if (panelControles != null) panelControles.SetActive(false);
        if (panelEnciclopedia != null) panelEnciclopedia.SetActive(false);

        // Resaltar el botón activo
        ResaltarBoton(ajustesButton, ajustesText); 
    }

    public void AbrirPanelControles(){
        // Activar/Desactivar Paneles de Contenido
        if (panelAjustes != null) panelAjustes.SetActive(false);
        if (panelControles != null) panelControles.SetActive(true);
        if (panelEnciclopedia != null) panelEnciclopedia.SetActive(false);

        // Resaltar el botón activo
        ResaltarBoton(controlesButton, controlesText); 
    }

    public void AbrirPanelEnciclopedia(){
        // Activar/Desactivar Paneles de Contenido
        if (panelAjustes != null) panelAjustes.SetActive(false);
        if (panelControles != null) panelControles.SetActive(false);
        if (panelEnciclopedia != null) panelEnciclopedia.SetActive(true);

        // Resaltar el botón activo
        ResaltarBoton(enciclopediaButton, enciclopediaText); 
    }
}