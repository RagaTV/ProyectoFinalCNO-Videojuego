using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public AudioSource[] tracks;
    private int currentTrackIndex = -1;
    private int aux=0;
   

    void Start()
    {
        // Al empezar, nos aseguramos de que todas las canciones
        // estén paradas para tener un inicio limpio.
        StopAllTracks();

        // Empezamos la primera canción (índice 0)
        PlayTrack(0);
    }

    void Update()
    {
        if (PlayerHealthController.instance.deathPlayer && aux==0)
        {
            aux=1;
            StopAllTracks();
            PlayTrack(4);
        }        
        
        float gameTimer = UIController.instance.gameTimer;
        float minutes = gameTimer / 60f;

        if(aux==0){
            // A los 15 minutos
            if (minutes >= 15f && currentTrackIndex < 3)
            {
                PlayTrack(3); // Toca la canción 4
            }
            // A los 10 minutos
            else if (minutes >= 10f && currentTrackIndex < 2)
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

    void PlayTrack(int trackIndex)
    {
        // Si ya estamos tocando esta canción
        if (trackIndex == currentTrackIndex) return;
        if (trackIndex < 0 || trackIndex >= tracks.Length) return;

        currentTrackIndex = trackIndex;
        
        // Detiene todas las canciones
        StopAllTracks();
        tracks[trackIndex].Play();
    }

    void StopAllTracks()
    {
        foreach (AudioSource track in tracks)
        {
            track.Stop();
        }
    }
}