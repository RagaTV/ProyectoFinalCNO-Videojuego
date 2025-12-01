using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    public int coinValue;
    private bool movingToPlayer = false;
    public float moveSpeed;
    public float timeBetweenChecks = .2f;
    private float checkCounter;
    
    // Update is llamado cada frame
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
            SFXManager.instance.PlaySFXPitched(SoundEffect.CoinPickup);
            CoinController.instance.AddCoins(coinValue);

            Destroy(gameObject);
        }
    }
}