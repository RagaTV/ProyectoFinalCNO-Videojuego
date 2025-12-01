using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnChest : MonoBehaviour
{
    public GameObject[] chestPrefabs;
    public float distancia = 1f;
    private Vector3 spawnPosition; 
    
    void Start()
    {
        StartCoroutine(SpawnC(20.0f));
    }

    void Update()
    {
    
    }

    public void SpawnChestCamara()
    {
        int chestIndex = Random.Range(0, chestPrefabs.Length);
        Camera mainCamera = Camera.main;
        bool aux = false;
        //area de aparición
        Vector3 pos1 = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane)); 
        Vector3 pos2 = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane));
        while (aux!=true)// encontrar una posicion libre
        {
            float spawnX = Random.Range(pos1.x, pos2.x);
            float spawnY = Random.Range(pos1.y, pos2.y);
            spawnPosition = new Vector3(spawnX, spawnY, 0f);
            // si no hay colisión en 2D 
            // OverlapCircle para un radio alrededor de la posicion
            if(Physics2D.OverlapCircle(spawnPosition, distancia)==null)
            {
                Instantiate(chestPrefabs[chestIndex], spawnPosition, chestPrefabs[chestIndex].transform.rotation);
                aux=true;
            }
        }
    }

    private IEnumerator SpawnC(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            SpawnChestCamara();
        }
    }

}
