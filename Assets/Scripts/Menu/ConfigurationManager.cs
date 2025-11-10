using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 
using UnityEngine.Audio; // NECESARIO para AudioMixer
using TMPro; // NECESARIO para usar TextMeshProUGUI

public class ConfigurationManager : MonoBehaviour
{
    // REFERENCIAS DE PANELES DE REGRESO
    public GameObject MenuPausaPanel;      
    public GameObject MenuPrincipalPanel; 
    
    // REFERENCIAS DE LA INTERFAZ DE CONFIGURACIÓN
    public GameObject menuConfiguracionPanel; 
    
    // Paneles de Contenido
    public GameObject panelAjustes;      
    public GameObject panelControles;    
    public GameObject panelEnciclopedia;

    // BOTONES
    public Button ajustesButton;
    public Button controlesButton;
    public Button enciclopediaButton;

    // REFERENCIAS DEL TEXTO (TMPro)
    public TextMeshProUGUI ajustesText;
    public TextMeshProUGUI controlesText;
    public TextMeshProUGUI enciclopediaText;

    // --- COMPONENTES DE AUDIO Y SLIDERS (Solo Música y SFX) ---
    public AudioMixer mainMixer;       // Tu Mixer principal
    public Slider musicSlider;         // Slider para la música
    public Slider sfxSlider;           // Slider para los efectos de sonido

    // PROPIEDADES DE COLOR
    public Color colorActivo = new Color32(29, 23, 23, 255); // Gris Oscuro (Opaco)
    public Color colorInactivo = new Color(0.7f, 0.7f, 0.7f, 1f); // Gris Claro
    public Color colorTextoActivo = Color.black; 
    public Color colorTextoInactivo = Color.white; 

    private const string MUSIC_PARAM = "MusicVolume";     // Nombre del parámetro expuesto en el Mixer
    private const string SFX_PARAM = "SFXVolume";         // Nombre del parámetro expuesto en el Mixer


    void Start()
    {
        // Inicializa solo los sliders de Música y SFX al cargar la escena
        if (mainMixer != null)
        {
            InitializeSlider(musicSlider, MUSIC_PARAM);
            InitializeSlider(sfxSlider, SFX_PARAM);
        }
    }

    // Función auxiliar para inicializar sliders
    private void InitializeSlider(Slider slider, string paramName)
    {
        if (slider != null && mainMixer.GetFloat(paramName, out float volume))
        {
            // Convierte de Decibelios a valor lineal (0 a 1)
            slider.value = Mathf.Pow(10, volume / 20);
        }
    }

    // --- FUNCIONES DE CONTROL DE VOLUMEN (Asignar a cada Slider) ---

    public void SetMusicVolume(float volume)
    {
        SetMixerVolume(MUSIC_PARAM, volume);
    }

    public void SetSFXVolume(float volume)
    {
        SetMixerVolume(SFX_PARAM, volume);
    }

    // Función central que realiza la conversión logarítmica (con FIX de volumen mínimo)
    private void SetMixerVolume(string parameterName, float volume)
    {
        if (mainMixer == null) return;

        // FIX: Asegura que el volumen nunca es 0 para evitar errores logarítmicos, usando 0.0001f como mínimo.
        float clampedVolume = Mathf.Max(0.0001f, volume); 
        
        // Convierte el valor lineal del slider (0.0001 a 1) a Decibelios (-80dB a 0dB)
        float dB = Mathf.Log10(clampedVolume) * 20;

        // Establece el parámetro en el AudioMixer
        mainMixer.SetFloat(parameterName, dB);
    }
    
    // --- FUNCIÓN PRIVADA PARA RESALTAR EL BOTÓN ACTIVO ---
    private void ResaltarBoton(Button botonActivo, TextMeshProUGUI textoActivo)
    {
        // Reseteo de IMAGEN
        if (ajustesButton != null && ajustesButton.image != null) ajustesButton.image.color = colorInactivo;
        if (controlesButton != null && controlesButton.image != null) controlesButton.image.color = colorInactivo;
        if (enciclopediaButton != null && enciclopediaButton.image != null) enciclopediaButton.image.color = colorInactivo;

        // Reseteo de TEXTO
        if (ajustesText != null) ajustesText.color = colorTextoInactivo;
        if (controlesText != null) controlesText.color = colorTextoInactivo;
        if (enciclopediaText != null) enciclopediaText.color = colorTextoInactivo;

        // Aplicar color ACTIVO
        if (botonActivo != null && botonActivo.image != null)
        {
            botonActivo.image.color = colorActivo; 
        }
        
        if (textoActivo != null)
        {
            textoActivo.color = colorTextoActivo; 
        }
    }


    public void AbrirConfiguracion(){
        if (menuConfiguracionPanel != null)
        {
            menuConfiguracionPanel.SetActive(true);
        }
        AbrirPanelAjustes();
    }

    public void Regresar()
    {
        if (menuConfiguracionPanel != null)
        {
            menuConfiguracionPanel.SetActive(false);
        }
        
        if (PauseMenu.AbiertoDesdePausa) 
        {
            if (MenuPausaPanel != null)
            {
                MenuPausaPanel.SetActive(true);
                
                PauseMenu pauseMenu = FindObjectOfType<PauseMenu>();
                if (pauseMenu != null)
                {
                    pauseMenu.Pausar(); 
                }
            }
        }
        else 
        {
            if (MenuPrincipalPanel != null)
            {
                MenuPrincipalPanel.SetActive(true);
            }
        }
    }
    

    public void AbrirPanelAjustes(){
        if (panelAjustes != null) panelAjustes.SetActive(true);
        if (panelControles != null) panelControles.SetActive(false);
        if (panelEnciclopedia != null) panelEnciclopedia.SetActive(false);
        ResaltarBoton(ajustesButton, ajustesText); 
    }

    public void AbrirPanelControles(){
        if (panelAjustes != null) panelAjustes.SetActive(false);
        if (panelControles != null) panelControles.SetActive(true);
        if (panelEnciclopedia != null) panelEnciclopedia.SetActive(false);
        ResaltarBoton(controlesButton, controlesText); 
    }

    public void AbrirPanelEnciclopedia(){
        if (panelAjustes != null) panelAjustes.SetActive(false);
        if (panelControles != null) panelControles.SetActive(false);
        if (panelEnciclopedia != null) panelEnciclopedia.SetActive(true);
        ResaltarBoton(enciclopediaButton, enciclopediaText); 
    }
}