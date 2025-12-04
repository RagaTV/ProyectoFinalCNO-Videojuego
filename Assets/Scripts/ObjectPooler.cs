using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Creamos una clase para que sea más fácil organizar los prefabs en el Inspector.
[System.Serializable]
public class EnemyTier {
    public string tierName;
    public List<GameObject> prefabs;
    public int poolSizePerPrefab;
}

public class ObjectPooler : MonoBehaviour
{
    public List<EnemyTier> enemyTiers;
    private Dictionary<GameObject, Queue<GameObject>> poolDictionary;

    void Awake()
    {
        poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();

        foreach (EnemyTier tier in enemyTiers)
        {
            foreach (GameObject prefab in tier.prefabs)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();

                for (int i = 0; i < tier.poolSizePerPrefab; i++)
                {
                    GameObject obj = Instantiate(prefab, transform);
                    obj.SetActive(false);
                    objectPool.Enqueue(obj);
                }

                poolDictionary.Add(prefab, objectPool);
            }
        }
    }

    // ---------------------------------------------------
    // Lógica de dificultad por tiempo
    // ---------------------------------------------------

    public GameObject GetEnemyByDifficulty()
    {
        float minutes = UIController.instance.gameTimer / 60f;
        float randomValue = Random.value;

        // 0 - 3 min → Easy
        if (minutes < 3f)
        {
            return GetRandomPooledObject(0); // Easy = Tier 0
        }

        // 3 - 7 min → Easy (50%) o Normal (50%)
        else if (minutes < 7f)
        {
            if (randomValue < 0.5f)
                return GetRandomPooledObject(0); // Easy
            else
                return GetRandomPooledObject(1); // Normal
        }

        // 7 - 12 min → Siempre Easy + (Normal 60% / Hard 40%)
        else if (minutes < 12f)
        {
            if (randomValue < 0.6f)
                return GetRandomPooledObject(1); // Normal
            else
                return GetRandomPooledObject(2); // Hard
        }

        // Más de 12 min → solo Hard 
        else
        {
            return GetRandomPooledObject(2);
        }
    }

    public GameObject GetExtraEasy()
    {
        return GetRandomPooledObject(0);
    }

    public GameObject GetRandomPooledObject(int tierIndex)
    {
        if (tierIndex < 0 || tierIndex >= enemyTiers.Count)
        {
            Debug.LogError("Índice de tier inválido: " + tierIndex);
            return null;
        }

        List<GameObject> prefabsInTier = enemyTiers[tierIndex].prefabs;

        if (prefabsInTier.Count == 0)
        {
            Debug.LogWarning("El tier " + tierIndex + " no tiene prefabs asignados.");
            return null;
        }

        GameObject randomPrefab = prefabsInTier[Random.Range(0, prefabsInTier.Count)];
        return GetSpecificPooledObject(randomPrefab);
    }

    private GameObject GetSpecificPooledObject(GameObject prefab)
    {
        if (!poolDictionary.ContainsKey(prefab))
        {
            Debug.LogError("No hay una piscina para el prefab: " + prefab.name);
            return null;
        }

        Queue<GameObject> pool = poolDictionary[prefab];
        GameObject objectToSpawn = null;

        // Buscar uno inactivo en la cola
        foreach (GameObject obj in pool)
        {
            if (!obj.activeSelf)
            {
                objectToSpawn = obj;
                break;
            }
        }

        if (objectToSpawn == null)
        {
            objectToSpawn = Instantiate(prefab, transform);
            pool.Enqueue(objectToSpawn);
            Debug.Log($"[Pooler] Creado nuevo objeto para {prefab.name} (pool lleno)");
        }

        return objectToSpawn;
    }
}