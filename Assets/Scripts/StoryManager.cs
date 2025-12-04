using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryManager : MonoBehaviour
{
    public static StoryManager instance;

    private void Awake()
    {
        instance = this;
    }

    private bool startDialogueShown = false;

    private void Update()
    {
        if (UIController.instance == null) return;

        float minutes = UIController.instance.gameTimer / 60f;

        // Diálogo Inicial (5 segundos)
        if (minutes >= 0.08f && !startDialogueShown) // ~5 seg
        {
            startDialogueShown = true;
            StartCoroutine(PlayStartDialogue());
        }
    }

    public void OnBossDefeated(string bossName)
    {
        StartCoroutine(PlayBossDefeatedDialogue(bossName));
    }

    IEnumerator PlayStartDialogue()
    {
        Vector3 pos = PlayerController.instance.transform.position + Vector3.up * 2f;
        float speed = 0.5f;

        yield return new WaitForSeconds(1f);
        pos = PlayerController.instance.transform.position + Vector3.up * 2f;
        DamageNumberController.instance.SpawnFloatingText("¿Dónde estoy?...", pos, speed);
        
        yield return new WaitForSeconds(3f);
        pos = PlayerController.instance.transform.position + Vector3.up * 2f;
        DamageNumberController.instance.SpawnFloatingText("No recuerdo nada...", pos, speed);
        
        yield return new WaitForSeconds(3f);
        pos = PlayerController.instance.transform.position + Vector3.up * 2f;
        DamageNumberController.instance.SpawnFloatingText("Debo salir de aquí.", pos, speed);
    }

    IEnumerator PlayBossDefeatedDialogue(string bossName)
    {
        Vector3 pos = PlayerController.instance.transform.position + Vector3.up * 2f;
        float speed = 0.5f;

        yield return new WaitForSeconds(1f);

        if (bossName.Contains("Frogger") || bossName.Contains("Rana"))
        {
            pos = PlayerController.instance.transform.position + Vector3.up * 2f;
            DamageNumberController.instance.SpawnFloatingText("¿Qué era esa cosa?", pos, speed);
            
            yield return new WaitForSeconds(3f);
            pos = PlayerController.instance.transform.position + Vector3.up * 2f;
            DamageNumberController.instance.SpawnFloatingText("He encontrado una fotografía...", pos, speed);
            
            yield return new WaitForSeconds(3f);
            pos = PlayerController.instance.transform.position + Vector3.up * 2f;
            DamageNumberController.instance.SpawnFloatingText("Parece... mi familia.", pos, speed);
        }
        else if (bossName.Contains("Gollux") || bossName.Contains("Eye"))
        {
            pos = PlayerController.instance.transform.position + Vector3.up * 2f;
            DamageNumberController.instance.SpawnFloatingText("Otra pesadilla terminada.", pos, speed);
            
            yield return new WaitForSeconds(3f);
            pos = PlayerController.instance.transform.position + Vector3.up * 2f;
            DamageNumberController.instance.SpawnFloatingText("Hay una nota en el suelo...", pos, speed);
            
            yield return new WaitForSeconds(3f);
            pos = PlayerController.instance.transform.position + Vector3.up * 2f;
            DamageNumberController.instance.SpawnFloatingText("'Busca las píldoras de la verdad'", pos, speed);
        }
        else if (bossName.Contains("Apple") || bossName.Contains("Cat"))
        {
            pos = PlayerController.instance.transform.position + Vector3.up * 2f;
            DamageNumberController.instance.SpawnFloatingText("Todo se desvanece...", pos, speed);
            
            yield return new WaitForSeconds(3f);
            pos = PlayerController.instance.transform.position + Vector3.up * 2f;
            DamageNumberController.instance.SpawnFloatingText("Esas luces... ¿son de un hospital?", pos, speed);
            
            yield return new WaitForSeconds(3f);
            pos = PlayerController.instance.transform.position + Vector3.up * 2f;
            DamageNumberController.instance.SpawnFloatingText("Al fin... silencio.", pos, speed);
            yield return new WaitForSeconds(3f);

            // Secuencia Final
            if (CameraControl.instance != null)
            {
                yield return StartCoroutine(CameraControl.instance.FadeToWhite(2f));
                yield return new WaitForSeconds(2f);
            }
            else
            {
                yield return new WaitForSeconds(2f);
            }

            // Música Final (Track 4)
            if (MusicController.instance != null)
            {
                MusicController.instance.PlayTrack(4);
            }

            // Game Over
            if (PlayerStats.instance != null)
            {
                PlayerStats.instance.GameOver();
            }
        }
    }
}
