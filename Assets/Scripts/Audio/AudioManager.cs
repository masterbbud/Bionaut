using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    void Awake()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.audioClip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    private void Start()
    {
        Debug.Log("Youshould hear music.");
        Play("Theme");
    }


    public void Play(string name)
    {
        Sound currentSound = Array.Find(sounds, sound => sound.name == name);
        if (currentSound == null)
        {
            Debug.Log("Sound not available");
            return;
        }
        currentSound.source.Play();
    }
}