using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public float timeToSpawn;
    private float spawnCounter;
    public Transform minSpawn, maxSpawn;
    private Transform target;
    private GameObject playerObject;
    public Vector2 mapMinBounds;
    public Vector2 mapMaxBounds;

    [SerializeField] private ObjectPooler pool;
    private PlayerHealthController playerHealth;

    public static EnemySpawner instance;
    public GameObject redPillPrefab;
    public GameObject bluePillPrefab;
    public GameObject bossAppleCatPrefab;
    private bool eventTriggered = false;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        spawnCounter = timeToSpawn;
        playerObject = GameObject.FindGameObjectWithTag("Player");
        target = playerObject.transform;
        playerHealth = playerObject.GetComponent<PlayerHealthController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerHealth != null && playerHealth.deathPlayer)
            return;
            
        spawnCounter -= Time.deltaTime;
        if (spawnCounter <= 0)
        {
            spawnCounter = timeToSpawn;

            float minutes = UIController.instance.gameTimer / 60f;

            // EVENTO MINUTO 15: FINAL DEL JUEGO / DECISIÓN
            if (minutes >= 15f) // VALOR ORIGINAL
            // if (minutes >= 0.16f) // VALOR DE PRUEBA: ~10 segundos
            {
                if (!eventTriggered)
                {
                    StartCoroutine(StartEndGameEvent());
                }
                return; // Ya no spawneamos nada más
            }

            GameObject spawnedObject = pool.GetEnemyByDifficulty();
            spawnedObject.transform.position = SelectSpawnPoint();
            spawnedObject.SetActive(true);

            // float minutes = UIController.instance.gameTimer / 60f; // Ya calculado arriba
            if (minutes >= 5f)
            {
                GameObject extra = pool.GetExtraEasy();
                extra.transform.position = SelectSpawnPoint();
                extra.SetActive(true);
            }
            if (minutes >= 12)
            {
                GameObject enemyExtra = pool.GetEnemyByDifficulty();
                enemyExtra.transform.position = SelectSpawnPoint();
                enemyExtra.SetActive(true);
            }
        }

        transform.position = target.position;
    }

    public Vector3 SelectSpawnPoint(){
        Vector3 spawnPoint = Vector3.zero;

        bool spawnVerticalEdge = Random.Range(0f,1f) > 0.5f;
        if (spawnVerticalEdge)
        {
            spawnPoint.y = Random.Range(minSpawn.position.y, maxSpawn.position.y);
            if (Random.Range(0f, 1f) > 0.5f)
            {
                spawnPoint.x = maxSpawn.position.x;
            }
            else
            {
                spawnPoint.x = minSpawn.position.x;
            }
        }
        else
        {
            spawnPoint.x = Random.Range(minSpawn.position.x, maxSpawn.position.x);
            if (Random.Range(0f, 1f) > 0.5f)
            {
                spawnPoint.y = maxSpawn.position.y;
            }
            else
            {
                spawnPoint.y = minSpawn.position.y;
            }
        }
        
        spawnPoint.x = Mathf.Clamp(spawnPoint.x, mapMinBounds.x, mapMaxBounds.x);
        spawnPoint.y = Mathf.Clamp(spawnPoint.y, mapMinBounds.y, mapMaxBounds.y);
    
        return spawnPoint;
    }

    IEnumerator StartEndGameEvent()
    {
        eventTriggered = true;

        // 1. Despawnear todos los enemigos
        DespawnAllEnemies();
        
        // Detener música
        if (MusicController.instance != null)
        {
            MusicController.instance.StopAllTracks();
        }

        // 2. FADE OUT (Oscurecer pantalla)
        if (CameraControl.instance != null)
        {
            yield return StartCoroutine(CameraControl.instance.FadeOut(1f));
        }
        else
        {
            yield return new WaitForSeconds(1f);
        }

        // 3. Resetear jugador y congelarlo (mientras está oscuro)
        if (PlayerController.instance != null)
        {
            PlayerController.instance.ResetPosition();
            PlayerController.instance.SetWeaponsActive(false);
            PlayerController.instance.enabled = false;
        }

        // 4. Limpiar Pickups y Cofres
        CleanUpPickupsAndChests();
        SpawnChest chestSpawner = FindObjectOfType<SpawnChest>();
        if (chestSpawner != null) chestSpawner.enabled = false;

        // 5. FADE IN (Aclarar pantalla para ver los textos)
        if (CameraControl.instance != null)
        {
            yield return StartCoroutine(CameraControl.instance.FadeIn(1f));
        }

        // 6. MENSAJES FLOTANTES
        if (DamageNumberController.instance != null)
        {
            Vector3 msgPos = PlayerController.instance.transform.position + Vector3.up * 2f;
            float textSpeed = 0.5f;

            DamageNumberController.instance.SpawnFloatingText("Has logrado sobrevivir\nel tiempo necesario", msgPos, textSpeed);
            yield return new WaitForSeconds(3f);
            
            DamageNumberController.instance.SpawnFloatingText("Puedes terminar con todo\ntomando la pastilla azul", msgPos, textSpeed);
            yield return new WaitForSeconds(3f);

            DamageNumberController.instance.SpawnFloatingText("O enfrentar tus miedos\ntomando la pastilla roja", msgPos, textSpeed);
            yield return new WaitForSeconds(3f);
        }
        else
        {
            yield return new WaitForSeconds(2f);
        }

        // 7. Reactivar movimiento
        if (PlayerController.instance != null)
        {
            PlayerController.instance.enabled = true;
        }

        // 4. Spawnear Pastillas
        Vector3 playerPos = PlayerController.instance.transform.position;
        if (redPillPrefab != null) Instantiate(redPillPrefab, playerPos + new Vector3(3f, 0f, 0f), Quaternion.identity);
        if (bluePillPrefab != null) Instantiate(bluePillPrefab, playerPos + new Vector3(-3f, 0f, 0f), Quaternion.identity);
    }

    public void CleanUpPickupsAndChests()
    {
        // Destruir Pickups (Monedas, XP, Comida)
        // Asumiendo que tienen tags o scripts específicos. Si no tienen tag, buscar por tipo.
        // Ajusta los Tags según tu proyecto.
        string[] tagsToClean = { "Coin", "Exp", "Chest" }; 
        
        foreach (string tag in tagsToClean)
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject obj in objects)
            {
                Destroy(obj);
            }
        }
        
        // Si usas scripts específicos y no tags, puedes usar FindObjectsOfType
        // CoinPickup[] coins = FindObjectsOfType<CoinPickup>(); ...
    }

    public void DespawnAllEnemies()
    {
        // Buscar todos los objetos con tag "Enemy" y desactivarlos/destruirlos
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            enemy.SetActive(false); // O Destroy(enemy) si no usas pool para todo
        }
        
        // También los Bosses si hay alguno vivo
        GameObject[] bosses = GameObject.FindGameObjectsWithTag("Boss");
        foreach (GameObject boss in bosses)
        {
            boss.SetActive(false);
        }
    }

    public void ResumeGameForTrueEnding()
    {
        // 1. Reactivar armas
        if (PlayerController.instance != null)
        {
            PlayerController.instance.SetWeaponsActive(true);
        }

        // 2. Spawnear Boss AppleCat
        if (bossAppleCatPrefab != null)
        {
            GameObject bossObj = Instantiate(bossAppleCatPrefab, new Vector3(0f, 5f, 0f), Quaternion.identity);
            BossBase bossScript = bossObj.GetComponent<BossBase>();
            if (bossScript != null && BossHealthBar.instance != null)
            {
                BossHealthBar.instance.ActivateBossHealth(bossScript, "AppleCat");
            }
        }

        // 3. Música Final
        if (MusicController.instance != null) // Asumiendo que existe instancia estática o búscala
        {
            // MusicController no tiene static instance en tu código original, vamos a buscarlo
            MusicController mc = FindObjectOfType<MusicController>();
            if (mc != null) mc.PlayTrack(3); // EndTheme
        }
        
        // Nota: Los enemigos normales NO vuelven a spawnear porque el timer sigue > 15
        // Si quieres que vuelvan, tendrías que cambiar la lógica del Update, 
        // pero normalmente en la pelea final es solo el Boss.
        // Si quieres enemigos + Boss, comenta el "return" en el Update o usa un flag "bossFightActive".
    }
}
