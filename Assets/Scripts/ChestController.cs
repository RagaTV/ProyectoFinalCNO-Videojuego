using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChestController : MonoBehaviour
{
    public int valor;
    public Animator anim;
    private bool alreadyOpened=false;
    public GameObject indicator;
    public GameObject coinChest;
    public GameObject imcoinChest;
    public TextMeshProUGUI coinText;
    private Coroutine mostrar;
    public ParticleSystem particulas;
    

    void Start()
    {
        anim = GetComponent<Animator>();
        coinChest.transform.position = transform.position;
        coinChest.transform.position += new Vector3(0.32f, 0.32f, 0f);
        imcoinChest.transform.position = transform.position;
        imcoinChest.transform.position += new Vector3(-0.2f, 0.32f, 0f);
        coinText.text ="- "+ valor;
        mostrar = StartCoroutine(mostrarCoins());    
    }

    // Update is called once per frame
    void Update()
    {
        if(alreadyOpened==true){
            StartCoroutine(destroyChest(10.0f));
        }
    }



    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if(Input.GetKeyDown(KeyCode.E) && alreadyOpened==false){//si puede abrirlo
                if(CoinController.instance.currentCoins >= valor){
                    //muestra monedas encima del cofre
                    StopCoroutine(mostrar);   
                    coinChest.SetActive(false);
                    imcoinChest.SetActive(false);
                    coinText.gameObject.SetActive(false);
                    //Se le resta lo que vale el cofre
                    CoinController.instance.currentCoins=CoinController.instance.currentCoins-valor; 
                    SFXManager.instance.PlaySFX(SoundEffect.ChestSound);
                    anim.SetBool("Close", false);
                    alreadyOpened = true;
                    StartCoroutine(ShowSeconds(0.7f));
                    

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
        yield return new WaitForSeconds(0.5f);
        particulas.Play();
        yield return new WaitForSeconds(delay);
        UIController.instance.ShowLevelUpOptions(true);
    }

    private IEnumerator mostrarCoins()
    {
        while(true){
            coinChest.SetActive(true);
            imcoinChest.SetActive(true);
            coinText.gameObject.SetActive(true); 
            yield return new WaitForSeconds(1.0f);
            coinChest.SetActive(false);
            imcoinChest.SetActive(false);
            coinText.gameObject.SetActive(false);
            yield return new WaitForSeconds(1.0f); 
        }
    }

    private IEnumerator destroyChest(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}


  