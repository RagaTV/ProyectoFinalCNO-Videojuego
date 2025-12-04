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

    // Referencias (Arrastralas en el inspector o usa FindObjectOfType)
    
    void Update()
    {
        // Asegurate de tener UIController.instance accesible, si no usa Time.time
        float currentGameTime =  UIController.instance.gameTimer; // O UIController.instance.gameTimer;

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
        
        // 2. Advertencia visual (Logs por ahora)
        Debug.LogWarning("¡⚠️ ALERTA DE JEFE: " + bossEvent.bossName + " ⚠️!");
        
        // Espera dramática (puedes poner un sonido aquí)
        yield return new WaitForSeconds(2f); 

        // 3. Calcular posición: En un círculo alrededor del jugador
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        Vector3 spawnPos = PlayerController.instance.transform.position + (Vector3)(randomDirection * spawnDistance);

        // 4. Invocar al Jefe
        GameObject bossObject = Instantiate(bossEvent.bossPrefab, spawnPos, Quaternion.identity);

        // 5. CONECTAR LA BARRA DE VIDA
        // Obtenemos TU script del objeto instanciado
        BossBase bossScript = bossObject.GetComponent<BossBase>();
        
        if (bossScript != null && BossHealthBar.instance != null)
        {
            BossHealthBar.instance.ActivateBossHealth(bossScript, bossEvent.bossName);
        }
        
        // (Opcional) Si quieres que la cámara haga algo, hazlo aquí
    }
    
}