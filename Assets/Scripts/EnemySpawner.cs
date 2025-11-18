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

    // Start is called before the first frame update
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

            GameObject spawnedObject = pool.GetEnemyByDifficulty();
            spawnedObject.transform.position = SelectSpawnPoint();
            spawnedObject.SetActive(true);

            float minutes = UIController.instance.gameTimer / 60f;
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
}
