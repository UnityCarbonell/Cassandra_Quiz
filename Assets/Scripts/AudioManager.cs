using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable()]
public struct SoundParameters
{
    [Range(0, 1.0f)]
    public float Volume;
    [Range(-3.0f, 3.0f)]
    public float Pitch;
    public bool Loop;
}
[System.Serializable()]
public class Sound
{
    [SerializeField] string name;
    public string Name { get { return name; } }

    [SerializeField] AudioClip clip;
    public AudioClip Clip { get { return clip; } }

    [SerializeField] SoundParameters parameters;
    public SoundParameters Parameters { get { return Parameters; } }

    [HideInInspector]
    public AudioSource Source;

    public void Play()
    {
        Source.clip = Clip;

        Source.volume = parameters.Volume;
        Source.pitch = parameters.Pitch;
        Source.loop = parameters.Loop;

        Source.Play();
    }

    public void Stop()
    {
        Source.Stop();
    }
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] Sound[] sounds;
    [SerializeField] AudioSource sourcePrefab;
    [SerializeField] string startUpTrack;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        InitSounds();
    }

    void Start()
    {
        if (string.IsNullOrEmpty(startUpTrack) != true)
        {
            SoundPlay(startUpTrack);
        }    
    }

    void InitSounds()
    {
        foreach (var sound in sounds)
        {
            AudioSource source = (AudioSource)Instantiate(sourcePrefab, gameObject.transform);
            source.name = sound.Name;
            sound.Source = source;
        }
    }

    public void SoundPlay(string name)
    {
        var sound = GetSound(name);
        if (sound != null)
        {
            sound.Play();
        }
        else
        {
            Debug.LogWarningFormat("No sound named {0} was found. We have a problem in AudioManager.SoundPlay().", name);
        }
    }

    public void SoundStop(string name)
    {
        var sound = GetSound(name);
        if (sound != null)
        {
            sound.Stop();
        }
        else
        {
            Debug.LogWarningFormat("No sound named {0} was found. We have a problem in AudioManager.SoundStop().", name);
        }
    }

    Sound GetSound(string name)
    {
        foreach (var sound in sounds)
        {
            if (sound.Name == name)
            {
                return sound;
            }
        }
        return null;
    }
}
