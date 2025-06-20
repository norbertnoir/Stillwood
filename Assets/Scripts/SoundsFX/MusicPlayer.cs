using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour
{
    AudioSource audioSourcePlayer;
    public AudioClip bg;
    void Start()
    {
        audioSourcePlayer = GetComponent<AudioSource>();
        audioSourcePlayer.clip = bg;
        audioSourcePlayer.loop = true;
        audioSourcePlayer.Play();
    }

    private void Update()
    {
        GameObject ddol = GameObject.FindGameObjectWithTag("DontDestroyOnLoad");
        if (ddol != null)
        {
            Settings settings = ddol.GetComponent<Settings>();
            if (settings != null)
            {
                audioSourcePlayer.volume = settings.musicVolume / 100f;
            }
        }
    }

}
