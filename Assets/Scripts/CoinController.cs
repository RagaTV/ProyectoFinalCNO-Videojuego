using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    public static CoinController instance;
    public int currentCoins;

    public CoinPickup pickup;

    private void Awake()
    {
        instance = this;
    }

    public void AddCoins(int coinsToAdd)
    {
        currentCoins += coinsToAdd;

        UIController.instance.UpdateCoinCount(currentCoins);
    }

    // Esta función es llamada por el Enemigo al morir
    public void SpawnCoin(Vector3 position, int coinValue)
    {
        // Instancia el prefab de la moneda
        Instantiate(pickup, position, Quaternion.identity).coinValue = coinValue;
    }

    public bool SpendCoins(int coinsToSpend)
    {
        // Revisa si el jugador tiene suficientes monedas
        if (currentCoins >= coinsToSpend)
        {
            // Sí tiene Gasta las monedas
            currentCoins -= coinsToSpend;
            UIController.instance.UpdateCoinCount(currentCoins);
            return true; // Devuelve 'true' (éxito)
        }
        else
        {
            // No tiene suficientes
            return false; // Devuelve 'false' (fallo)
        }
    }
}