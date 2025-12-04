using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public static VirtualJoystick instance;

    [Header("Configuración Visual")]
    public Image joystickBg; // La base del joystick
    public Image joystickHandle; // La palanca que se mueve
    
    [Header("Ajustes")]
    public float joystickVisualDistance = 50f; // Qué tan lejos se mueve visualmente la palanca

    private Vector2 inputVector; // El vector de dirección (-1 a 1)

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Asegurarse de que el handle esté centrado al inicio
        if(joystickHandle != null && joystickBg != null)
        {
            joystickHandle.rectTransform.anchoredPosition = Vector2.zero;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickBg.rectTransform, eventData.position, eventData.pressEventCamera, out pos))
        {
            // Convertir la posición a un valor entre -1 y 1 relativo al tamaño de la base
            pos.x = (pos.x / joystickBg.rectTransform.sizeDelta.x);
            pos.y = (pos.y / joystickBg.rectTransform.sizeDelta.y);

            // Calcular el vector de entrada (normalizado si es necesario para que sea circular)
            inputVector = new Vector2(pos.x * 2, pos.y * 2); // Multiplicamos por 2 porque el pivote está en el centro (0.5)
            
            // Limitar el vector a una magnitud de 1 (círculo)
            inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;

            // Mover visualmente la palanca
            joystickHandle.rectTransform.anchoredPosition = new Vector2(inputVector.x * joystickVisualDistance, inputVector.y * joystickVisualDistance);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        inputVector = Vector2.zero;
        joystickHandle.rectTransform.anchoredPosition = Vector2.zero;
    }

    // Función pública para obtener la entrada desde otros scripts (PlayerController)
    public Vector2 GetInput()
    {
        return inputVector;
    }
}
