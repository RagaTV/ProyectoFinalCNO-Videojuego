using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillChoice : MonoBehaviour, IInteractable
{
    public bool isRedPill; // True = Boss Fight, False = Game Over

    // Variables para flotar
    public float floatSpeed = 2f;
    public float floatHeight = 0.2f;
    public float rotateSpeed = 2f;
    public float rotateAngle = 15f; // Grados de inclinación
    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Efecto de flotación
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(startPos.x, newY, startPos.z);

        // Efecto de rotación (balanceo)
        float zRotation = Mathf.Sin(Time.time * rotateSpeed) * rotateAngle;
        transform.rotation = Quaternion.Euler(0f, 0f, zRotation);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController.instance.SetInteractable(this);
            Debug.Log("Presiona Botón de Acción para seleccionar la pastilla " + (isRedPill ? "ROJA" : "AZUL"));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController.instance.ClearInteractable(this);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerController.instance.SetInteractable(this);
            Debug.Log("Presiona Botón de Acción para seleccionar la pastilla " + (isRedPill ? "ROJA" : "AZUL"));
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerController.instance.ClearInteractable(this);
        }
    }

    public void Interact()
    {
        if (isRedPill)
        {
            // True Ending: Boss Fight
            if (EnemySpawner.instance != null)
            {
                EnemySpawner.instance.ResumeGameForTrueEnding();
            }
            
            // Destruir ambas pastillas (buscándolas por tag o tipo)
            PillChoice[] pills = FindObjectsOfType<PillChoice>();
            foreach (PillChoice pill in pills)
            {
                Destroy(pill.gameObject);
            }
        }
        else
        {
            // Game Over (Blue Pill)
            StartCoroutine(ScreenGameOver());
        }
    }

    private IEnumerator ScreenGameOver()
    {
        // Efecto visual de "muerte" (zoom y fade)
        if (CameraControl.instance != null)
        {
            CameraControl.instance.StartDeathSequence();
        }

        // Espera dramática
        yield return new WaitForSeconds(2f);

        // Pantalla de Game Over
        if (PlayerStats.instance != null)
        {
            PlayerStats.instance.GameOver();
        }
        // Tocar música final
        if (MusicController.instance != null)
        {   
            MusicController.instance.PlayTrack(4);
        }
    }
}
