using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundSpawner : MonoBehaviour
{
    // Arrastra aquí los prefabs que quieres usar como decoración
    public GameObject[] decorPrefabs; 

    // Cuántos objetos de decoración quieres crear
    public int amountToSpawn = 30;

    // El centro del área donde aparecerán
    public Vector3 spawnAreaCenter = new Vector3(0, 0, 10);

    // El tamaño del "cubo" imaginario donde pueden aparecer
    public Vector3 spawnAreaSize = new Vector3(30, 20, 5);

    // Opcional: Un objeto padre para mantener la jerarquía limpia
    public Transform prefabParent;

    void Start()
    {
        SpawnDecorations();
    }

    void SpawnDecorations()
    {
        if (decorPrefabs.Length == 0)
        {
            Debug.LogWarning("No hay prefabs de decoración asignados en el Spawner.");
            return;
        }

        for (int i = 0; i < amountToSpawn; i++)
        {
            // 1. Elige un prefab al azar de la lista
            GameObject prefabToSpawn = decorPrefabs[Random.Range(0, decorPrefabs.Length)];

            // 2. Calcula una posición aleatoria dentro del área definida
            float spawnX = Random.Range(spawnAreaCenter.x - spawnAreaSize.x / 2, spawnAreaCenter.x + spawnAreaSize.x / 2);
            float spawnY = Random.Range(spawnAreaCenter.y - spawnAreaSize.y / 2, spawnAreaCenter.y + spawnAreaSize.y / 2);
            float spawnZ = Random.Range(spawnAreaCenter.z - spawnAreaSize.z / 2, spawnAreaCenter.z + spawnAreaSize.z / 2);

            Vector3 spawnPosition = new Vector3(spawnX, spawnY, spawnZ);

            // 3. Elige una rotación aleatoria
            Quaternion spawnRotation = Random.rotation;

            // 4. Crea el prefab en esa posición y rotación
            Instantiate(prefabToSpawn, spawnPosition, spawnRotation, prefabParent);
        }
    }
}