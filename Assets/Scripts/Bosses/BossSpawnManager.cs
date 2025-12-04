using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BossSpawnEvent
{
    public float timeToSpawn; 
    public GameObject bossPrefab;
    public string bossName;
    [HideInInspector] public bool hasSpawned = false;
}

public class BossSpawnManager : MonoBehaviour
{
    public List<BossSpawnEvent> bossEvents;
    
    [Header("Configuración Spawn")]
    public float spawnDistance = 10f; // Distancia del jugador a la que aparece

    // Referencias
    
    void Update()
    {
        float currentGameTime =  UIController.instance.gameTimer;

        foreach (BossSpawnEvent bossEvent in bossEvents)
        {
            if (!bossEvent.hasSpawned && currentGameTime >= bossEvent.timeToSpawn)
            {
                bossEvent.hasSpawned = true;
                StartCoroutine(SpawnBossSequence(bossEvent));
            }
        }
    }

    private IEnumerator SpawnBossSequence(BossSpawnEvent bossEvent)
    {
        
        Debug.LogWarning("¡⚠️ ALERTA DE JEFE: " + bossEvent.bossName + " ⚠️!");
        
        yield return new WaitForSeconds(2f); 
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        Vector3 spawnPos = PlayerController.instance.transform.position + (Vector3)(randomDirection * spawnDistance);

        // Invocar al Jefe
        GameObject bossObject = Instantiate(bossEvent.bossPrefab, spawnPos, Quaternion.identity);

        // CONECTAR LA BARRA DE VIDA
        BossBase bossScript = bossObject.GetComponent<BossBase>();
        
        if (bossScript != null && BossHealthBar.instance != null)
        {
            BossHealthBar.instance.ActivateBossHealth(bossScript, bossEvent.bossName);
        }
    }
}