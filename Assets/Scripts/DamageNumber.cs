using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageNumber : MonoBehaviour
{
    public TMP_Text damageText;
    public float lifeTime;
    private float lifeCounter;
    public float floatSpeed = 1f;

    // Update is called once per frame
    void Update()
    {
        if (lifeCounter > 0)
        {
            lifeCounter -= Time.deltaTime;

            if (lifeCounter <= 0)
            {
                //Destroy(gameObject);

                DamageNumberController.instance.PlaceInPool(this);
            }
        }

        transform.position += Vector3.up * floatSpeed * Time.deltaTime;
    }
    
    private float originalSpeed;

    private void Awake()
    {
        originalSpeed = floatSpeed;
    }

    public void Setup (int damageDisplay)
    {
        floatSpeed = originalSpeed; // Resetear velocidad normal
        lifeCounter = lifeTime;
        damageText.text = damageDisplay.ToString();
    }

    public void Setup(string textDisplay, float customSpeed = -1f)
    {
        lifeCounter = lifeTime;
        damageText.text = textDisplay;
        
        if (customSpeed >= 0)
        {
            floatSpeed = customSpeed;
        }
        else
        {
            floatSpeed = originalSpeed;
        }
    }
}
