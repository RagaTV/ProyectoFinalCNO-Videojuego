using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestController : MonoBehaviour
{
    public int valor;
    public Animator anim;
    private bool alreadyOpened=false;
    public GameObject indicator;
    public GameObject coinChest;

    void Start()
    {
        anim = GetComponent<Animator>();
        //coinChest.transform.position = transform.position;
       // coinChest.transform.position += new Vector3(0f, 1.5f, 0f);
    }

    // Update is called once per frame
    void Update()
    {

        
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //indicator.transform.position=transform.position+new Vector3(0, 1.5f, 0);
            if(Input.GetKeyDown(KeyCode.E) && alreadyOpened==false){//si puede abrirlo
                if(CoinController.instance.currentCoins >= valor){
                    CoinController.instance.currentCoins=CoinController.instance.currentCoins-valor;
                    SFXManager.instance.PlaySFX(SoundEffect.ChestSound);
                    anim.SetBool("Close", false);
                    alreadyOpened = true;
                    StartCoroutine(ShowSeconds(2.5f));

                }else{
                    Debug.Log("No abre");
                    StartCoroutine(ShowIndicatorForSeconds(1.0f));
                }
            }
        }
    }


    private IEnumerator ShowIndicatorForSeconds(float delay)
    {
        indicator.SetActive(true);
        yield return new WaitForSeconds(delay);
        indicator.SetActive(false);
    }

    private IEnumerator ShowSeconds(float delay)
    {
 
        yield return new WaitForSeconds(delay);
        UIController.instance.ShowLevelUpOptions();
    }
}
