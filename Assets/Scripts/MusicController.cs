using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public static MusicController instance;
    public AudioSource[] tracks;
    private int currentTrackIndex = -1;
    private int aux=0;
    private Coroutine deathMusicCoroutine; // Para controlar la corrutina de retraso

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        StopAllTracks();
        PlayTrack(0);
    }

    void Update()
    {
        if (PlayerHealthController.instance.deathPlayer && aux==0)
        {
            aux=1; // Marca que la secuencia de muerte ha comenzado
            StopAllTracks();
            
            // 1. INICIA LA CORRUTINA PARA ESPERAR 3 SEGUNDOS
            if (deathMusicCoroutine != null) StopCoroutine(deathMusicCoroutine);
            deathMusicCoroutine = StartCoroutine(StartDeathMusicDelay(4.0f));
            return; // Salimos del Update para no seguir verificando el tiempo
        }
        
        // Si la corrutina de muerte está corriendo, ignora la lógica de tiempo normal
        if (deathMusicCoroutine != null) return;
            
        // Lógica de cambio de canción por tiempo
        if(aux==0){
            float gameTimer = UIController.instance.gameTimer;
            float minutes = gameTimer / 60f;

            // A los 10 minutos
            if (minutes >= 10f && currentTrackIndex < 2)
            {
                PlayTrack(2); // Toca la canción 3
            }
            // A los 5 minutos
            else if (minutes >= 5f && currentTrackIndex < 1)
            {
                PlayTrack(1); // Toca la canción 2
            }
        }
    }

    private IEnumerator StartDeathMusicDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay); 

        // 2. Toca la canción después del retraso
        PlayTrack(4);
        deathMusicCoroutine = null; // Marca la corrutina como terminada
    }


    public void PlayTrack(int trackIndex)
    {
        // Si ya estamos tocando esta canción
        if (trackIndex == currentTrackIndex) return;
        if (trackIndex < 0 || trackIndex >= tracks.Length) return;

        currentTrackIndex = trackIndex;
        
        // Detiene todas las canciones
        StopAllTracks();
        tracks[trackIndex].Play();
    }

    public void StopAllTracks()
    {
        foreach (AudioSource track in tracks)
        {
            track.Stop();
        }
    }
}