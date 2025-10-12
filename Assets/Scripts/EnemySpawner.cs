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

            float minutes = Time.timeSinceLevelLoad / 60f;
            if (minutes >= 7f)
            {
                GameObject extra = pool.GetExtraEasy();
                extra.transform.position = SelectSpawnPoint();
                extra.SetActive(true);
            }
        }

        transform.position = target.position;
    }

    public Vector3 SelectSpawnPoint(){
        Vector3 spawnPoint = Vector3.zero;

        bool spawnVerticalEdge = Random.Range(0f,1f) > 0.5f;
        if(spawnVerticalEdge)
        {
            spawnPoint.y = Random.Range(minSpawn.position.y, maxSpawn.position.y);
            if(Random.Range(0f,1f) > 0.5f){
                spawnPoint.x = maxSpawn.position.x;
            } else {
                spawnPoint.x = minSpawn.position.x;
            }
        } else {
            spawnPoint.x = Random.Range(minSpawn.position.x, maxSpawn.position.x);
            if(Random.Range(0f,1f) > 0.5f){
                spawnPoint.y = maxSpawn.position.y;
            } else {
                spawnPoint.y = minSpawn.position.y;
            }
        }
        return spawnPoint;
    }
}
