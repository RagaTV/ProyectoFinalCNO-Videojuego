using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuEffects : MonoBehaviour
{
    public Sprite[] spritesDePersonajes;
    private Image imagenUI;
    private RectTransform rectTransform; // manejar posición y escala

    public float tiempoEntreCambios = 5.0f;
    public float tiempoDeTransicion = 1.0f;
    public float alphaDeseado = 0.5f;

    // 🆕 Configuración de movimiento y escala
    public Vector2 rangoMovimientoHorizontal = new Vector2(-150f, 150f); // Rango de desplazamiento
    public Vector2 rangoMovimientoVertical = new Vector2(-75f, 75f);     // Rango de desplazamiento
    public Vector2 rangoEscala = new Vector2(1.0f, 1.2f);             // Rango de escala (ej. 100% a 120%)
    public Vector2 rangoRotacion = new Vector2(-10f, 10f);             // Rango de rotación en Z

    void Start()
    {
        imagenUI = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>(); // Obtener el RectTransform
        
        // Inicializar la imagen como transparente (Alpha = 0)
        Color colorInicial = imagenUI.color;
        colorInicial.a = 0f;
        imagenUI.color = colorInicial;

        StartCoroutine(RotarImagenes());
    }

    IEnumerator RotarImagenes()
    {
        while (true)
        {
            // Desvanecer la imagen actual a transparente (Fade Out)
            yield return StartCoroutine(Fade(0f, tiempoDeTransicion)); 

            // Elegir nueva imagen y configurar animación
            int indiceAleatorio = Random.Range(0, spritesDePersonajes.Length);
            imagenUI.sprite = spritesDePersonajes[indiceAleatorio];

            // Definir un punto de inicio y un punto final aleatorios
            Vector2 posInicial = new Vector2(
                Random.Range(rangoMovimientoHorizontal.x, rangoMovimientoHorizontal.y),
                Random.Range(rangoMovimientoVertical.x, rangoMovimientoVertical.y)
            );
            Vector2 posFinal = new Vector2(
                Random.Range(rangoMovimientoHorizontal.x, rangoMovimientoHorizontal.y),
                Random.Range(rangoMovimientoVertical.x, rangoMovimientoVertical.y)
            );
            float escalaInicial = Random.Range(rangoEscala.x, rangoEscala.y);
            float escalaFinal = Random.Range(rangoEscala.x, rangoEscala.y);
            float rotInicial = Random.Range(rangoRotacion.x, rangoRotacion.y);
            float rotFinal = Random.Range(rangoRotacion.x, rangoRotacion.y);

            // Establecer posición y escala iniciales
            rectTransform.anchoredPosition = posInicial;
            rectTransform.localScale = Vector3.one * escalaInicial;
            rectTransform.localEulerAngles = new Vector3(0f, 0f, rotInicial);

            // Desvanecer la nueva imagen a la opacidad deseada (Fade In)
            yield return StartCoroutine(Fade(alphaDeseado, tiempoDeTransicion)); 

            // Mover y Escalar el personaje mientras está visible
            float tiempoMovimiento = tiempoEntreCambios;
            float tiempoTranscurrido = 0;
            
            while (tiempoTranscurrido < tiempoMovimiento)
            {
                tiempoTranscurrido += Time.deltaTime;
                float factor = tiempoTranscurrido / tiempoMovimiento;
                
                // Aplicar interpolación (movimiento suave)
                rectTransform.anchoredPosition = Vector2.Lerp(posInicial, posFinal, factor);
                rectTransform.localScale = Vector3.one * Mathf.Lerp(escalaInicial, escalaFinal, factor);
                rectTransform.localEulerAngles = new Vector3(0f, 0f, Mathf.Lerp(rotInicial, rotFinal, factor));
                
                yield return null;
            }
            
            // Asegurar posición y escala final
            rectTransform.anchoredPosition = posFinal;
            rectTransform.localScale = Vector3.one * escalaFinal;
            rectTransform.localEulerAngles = new Vector3(0f, 0f, rotFinal);
            
            // El bucle se reinicia para el Fade Out
        }
    }

    // Fade recibe la duración como parámetro
    IEnumerator Fade(float alphaFinal, float duracion)
    {
        float tiempoTranscurrido = 0;
        Color colorInicial = imagenUI.color;
        Color colorFinal = colorInicial;
        colorFinal.a = alphaFinal;

        while (tiempoTranscurrido < duracion)
        {
            tiempoTranscurrido += Time.deltaTime;
            float factor = tiempoTranscurrido / duracion;

            imagenUI.color = Color.Lerp(colorInicial, colorFinal, factor);
            
            yield return null;
        }
        
        imagenUI.color = colorFinal;
    }
}