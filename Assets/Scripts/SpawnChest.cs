using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnChest : MonoBehaviour
{
    public GameObject[] chestPrefabs;
    
    void Start()
    {
        StartCoroutine(SpawnC(20.0f));
    }

    void Update()
    {
    
    }

    public void SpawnChestCamara()
    {
        int chestIndex=Random.Range(0, chestPrefabs.Length);
        //aparezca dentro de la camara y el jugador lo vea
        Camera mainCamera = Camera.main;
        Vector3 pos1=mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane)); 
        Vector3 pos2 = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane));
        float spawnX = Random.Range(pos1.x  , pos2.x );
        float spawnY = Random.Range(pos1.y , pos2.y );
        Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0f);
        Instantiate(chestPrefabs[chestIndex], spawnPosition,chestPrefabs[chestIndex].transform.rotation);
    }
    private IEnumerator SpawnC(float delay)
    {
        while (true)
        {
            SpawnChestCamara();
            yield return new WaitForSeconds(delay);
        }
    }

}
