using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraControl : MonoBehaviour
{
    public static CameraControl instance; 

    private Transform target;
    private Camera cam; 
    
    [Header("Efecto de Muerte")]
    public Image fadePanel; 
    public float deathZoomDuration = 1f; 
    public float targetOrthographicSize = 2f; 
    public float targetFade = 0.7f; 

    private bool isPlayerDead = false;

    private void Awake()
    {
        instance = this; 
    }

    void Start()
    {
        target = FindObjectOfType<PlayerController>().transform;
        cam = GetComponent<Camera>(); 
    }

    void LateUpdate()
    {
        if (!isPlayerDead && target != null)
        {
            transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
        }
    }

    public void StartDeathSequence()
    {
        if (isPlayerDead) return; // Evita que se ejecute dos veces

        isPlayerDead = true;
        
        fadePanel.gameObject.SetActive(true);
        StartCoroutine(DeathZoomAndFade());
    }

    private IEnumerator DeathZoomAndFade()
    {
        float elapsedTime = 0f;
        float startSize = cam.orthographicSize;
        Color startColor = fadePanel.color;
        Color targetColor = new Color(0, 0, 0, targetFade); 

        while (elapsedTime < deathZoomDuration)
        {
            cam.orthographicSize = Mathf.Lerp(startSize, targetOrthographicSize, elapsedTime / deathZoomDuration);

            fadePanel.color = Color.Lerp(startColor, targetColor, elapsedTime / deathZoomDuration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cam.orthographicSize = targetOrthographicSize;
        fadePanel.color = targetColor;
    }
}
