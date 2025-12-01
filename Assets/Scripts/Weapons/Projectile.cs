using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{   
    [Header("Movimiento")]
    public float moveSpeed;

    [Header("Rotación Visual")]
    public float rotateSpeed = 360f; // Velocidad de giro (grados por segundo)
    public Transform modelToRotate;  // Aquí arrastrarás al hijo "Sprite"

    // Update is called once per frame
    void Update()
    {
        // Mueve el PADRE (Hacha Projectile) en línea recta
        transform.position += transform.up * moveSpeed * Time.deltaTime;

        // Rota el HIJO (Sprite) sobre su propio eje
        if (modelToRotate != null)
        {
            modelToRotate.Rotate(Vector3.back * rotateSpeed * Time.deltaTime);
        }
    }
}