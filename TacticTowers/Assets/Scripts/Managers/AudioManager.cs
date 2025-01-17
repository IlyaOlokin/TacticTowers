using UnityEngine;
using UnityEngine.Audio;
using System;
using System.Collections.Generic;
using System.Linq;

public class AudioManager : MonoBehaviour
{
    public Sound[] Sounds;
    public Sound[] Music;
    public static AudioManager Instance;
        
    public Sound CurrentMusic;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        DontDestroyOnLoad(gameObject);

        foreach (var sound in Sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.outputAudioMixerGroup = sound.audioMixerGroup;
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
        }
        
        foreach (var music in Music)
        {
            music.source = gameObject.AddComponent<AudioSource>();
            music.source.outputAudioMixerGroup = music.audioMixerGroup;
            music.source.clip = music.clip;
            music.source.volume = music.volume;
            music.source.pitch = music.pitch;
            music.source.loop = music.loop;
        }
    }

    public void Play(string name)
    {
        Array.Find(Sounds, sound => sound.name == name).source.Play();
    }

    public void Stop(string name)
    {
        Array.Find(Sounds, sound => sound.name == name).source.Stop();
    }

    public void PlayMusic(string musicName)
    {
        Music
            .Where(music => music.source.isPlaying && music.name != musicName)
            .ToList()
            .ForEach(music => music.source.Stop());
        Array.Find(Music, music => music.name == musicName).source.Play();
    }

    public void PlayMusic()
    {
        if (!CurrentMusic.source.isPlaying)
            CurrentMusic.source.Play();
    }
    
    public void ChangeMusic(string musicName)
    {
        Music
            .Where(music => music.source.isPlaying && music.name != musicName)
            .ToList()
            .ForEach(music => music.source.Stop());
        CurrentMusic = Array.Find(Music, music => music.name == musicName);
    }

    public void StopMusic()
    {
        CurrentMusic.source.Stop();
    }
}

[Serializable]
public class Sound
{
    public string name;
    
    public AudioClip clip;

    [Range(0, 1)]
    public float volume;
    [Range(0, 3)]
    public float pitch;

    public bool loop;

    public AudioMixerGroup audioMixerGroup;
    
    [HideInInspector]
    public AudioSource source;
}
