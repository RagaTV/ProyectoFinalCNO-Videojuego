using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundEffect
{
    None,
    DeathSound,
    LevelUp,
    UIClick,
    ExpPickup,
    SwordsCircle,
    ShieldSound,
    BrightZone,
    CoinPickup,
    BananaSound,
    MouseSound,
    ChestSound
    // ...añadir todos los necesarios
}

[System.Serializable]
public class SoundClip
{
    public SoundEffect type;
    public AudioSource source;
}

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;
    public List<SoundClip> soundClips;
    private Dictionary<SoundEffect, AudioSource> sfxDictionary;

    private void Awake()
    {
        instance = this;
        sfxDictionary = new Dictionary<SoundEffect, AudioSource>();
        foreach (SoundClip clip in soundClips)
        {
            sfxDictionary[clip.type] = clip.source;
        }
    }

    public void PlaySFX(SoundEffect sfxToPlay)
    {
        if (sfxDictionary.ContainsKey(sfxToPlay))
        {
            sfxDictionary[sfxToPlay].pitch = 1f; 
            
            sfxDictionary[sfxToPlay].Stop();
            sfxDictionary[sfxToPlay].Play();
        }
        else
        {
            Debug.LogWarning("SFXManager: No se encontró el sonido: " + sfxToPlay);
        }
    }

    public void PlaySFXPitched(SoundEffect sfxToPlay)
    {
        if (sfxDictionary.ContainsKey(sfxToPlay))
        {
            sfxDictionary[sfxToPlay].pitch = Random.Range(.8f, 1.2f);
            sfxDictionary[sfxToPlay].Stop();
            sfxDictionary[sfxToPlay].Play();
        }
        else
        {
            Debug.LogWarning("SFXManager: No se encontró el sonido: " + sfxToPlay);
        }
    }
}