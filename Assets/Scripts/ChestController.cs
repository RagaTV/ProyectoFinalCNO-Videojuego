using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestController : MonoBehaviour
{
    public int valor;
    public Animator anim;
    private bool alreadyOpened=false;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetKeyDown(KeyCode.E) && alreadyOpened==false){//si puede abrirlo
            if(CoinController.instance.currentCoins >= valor){
                CoinController.instance.currentCoins=CoinController.instance.currentCoins-valor;
                Debug.Log("Abre");
                anim.SetBool("Close", false);
                alreadyOpened = true;

            }else{
                Debug.Log("No abre");
            }
        }
        
    }
}
