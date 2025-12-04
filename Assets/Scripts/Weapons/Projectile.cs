using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{   
    [Header("Movimiento")]
    public float moveSpeed;

    [Header("Rotación Visual")]
    public float rotateSpeed = 360f; // Velocidad de giro (grados por segundo)
    public Transform modelToRotate;  

    void Update()
    {
        transform.position += transform.up * moveSpeed * Time.deltaTime;

        if (modelToRotate != null)
        {
            modelToRotate.Rotate(Vector3.back * rotateSpeed * Time.deltaTime);
        }
    }
}