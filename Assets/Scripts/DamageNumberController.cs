using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageNumberController : MonoBehaviour
{
    public static DamageNumberController instance;
    public void Awake()
    {
        instance = this;
    }

    public DamageNumber numberToSpawn;
    public Transform numberCanvas;
    private List<DamageNumber> numberPool = new List<DamageNumber>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SpawnDamage(float damageAmount, Vector3 location)
    {
        int rounded = Mathf.RoundToInt(damageAmount);

        DamageNumber newDamage = GetFromPool();

        newDamage.transform.position = location;
        newDamage.Setup(rounded);
        newDamage.gameObject.SetActive(true);
    }

    public void SpawnFloatingText(string text, Vector3 location, float speed = -1f)
    {
        DamageNumber newText = GetFromPool();
        newText.transform.position = location;
        newText.Setup(text, speed);
        newText.gameObject.SetActive(true);
    }

    public DamageNumber GetFromPool()
    {
        DamageNumber numberSpawn = null;
        if (numberPool.Count == 0)
        {
            numberSpawn = Instantiate(numberToSpawn, numberCanvas);
        } else
        {
            numberSpawn = numberPool[0];
            numberPool.RemoveAt(0);
        }

        return numberSpawn;
    }
    
    public void PlaceInPool(DamageNumber numberToPlace)
    {
        numberToPlace.gameObject.SetActive(false);

        numberPool.Add(numberToPlace);
    }
}
