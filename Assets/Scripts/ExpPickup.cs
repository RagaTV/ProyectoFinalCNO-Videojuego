using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpPickup : MonoBehaviour
{
    public int expValue;
    private bool movingToPlayer = false;
    public float moveSpeed;
    public float timeBetweenChecks = .2f;
    private float checkCounter;
    
    // Update is called once per frame
    void Update() 
    {
        if (movingToPlayer == true)
        {
            transform.position = Vector3.MoveTowards(transform.position, PlayerController.instance.transform.position, moveSpeed * Time.deltaTime);
        } else
        {
            checkCounter -= Time.deltaTime;
            if(checkCounter <= 0)
            {
                checkCounter = timeBetweenChecks;
                if(Vector3.Distance(transform.position, PlayerController.instance.transform.position) < PlayerStats.instance.pickupRange)
                {
                    movingToPlayer = true;
                    moveSpeed += PlayerStats.instance.moveSpeed + 1;
                }
            }
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            SFXManager.instance.PlaySFXPitched(SoundEffect.ExpPickup);

            ExperienceLevelController.instance.GetExp(expValue);

            Destroy(gameObject);
        }
    }
}
