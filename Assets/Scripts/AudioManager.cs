using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable()]
public struct SoundParameters
{
    [Range(0, 1.0f)]
    public float Volume;
    [Range(-3.0f, 3.0f)]
    public float Tone;
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
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

}
